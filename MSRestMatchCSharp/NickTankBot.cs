using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSRestMatchCSharp
{
    public class NickTankBot
    {
        private TcpClient client;
        private Thread clientReceiveThread;
        string secretToken;
        public bool botQuit = false;
      
        List<GameObjectState> objectState;
        GameObjectState ownState;
        GameObjectState currentClosest;

        public NickTankBot(string ip, int port, string name, string secretCode, string colour, float startX, float startZ, float angle)
        {

            ConnectToTcpServer();


            Thread.Sleep(5000);


            SendMessage(MessageFactory.CreateTankMessage(name, secretCode, colour));
          
            secretToken = secretCode;

            //despawn first in case we're re-running
            //helper.DespawnTank(secretToken

            //BasicMovementTest();




        }

        private void ConnectToTcpServer()
        {
            try
            {
                clientReceiveThread = new Thread(new ThreadStart(ListenForData));
                clientReceiveThread.IsBackground = true;
                clientReceiveThread.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("On client connect exception " + e);
            }
        }

        private void ListenForData()
        {
            try
            {
                client = new TcpClient("localhost", 8052);
                Byte[] bytes = new Byte[1024];
                while (true)
                {
                    // Get a stream object for reading 				
                    using (NetworkStream stream = client.GetStream())
                    {
                        int length;
                        // Read incomming stream into byte arrary. 					
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            DecodeMessage((NetworkMessageType)bytes[0], bytes, length);
                        }
                    }
                }
            }
            catch (SocketException socketException)
            {
                Console.WriteLine("Socket exception: " + socketException);
            }
        }


        private void DecodeMessage(NetworkMessageType messageType, byte[] bytes, int length)
        {

            var incomingData = new byte[length];
            Array.Copy(bytes, 1, incomingData, 0, length);
            string clientMessage = Encoding.ASCII.GetString(incomingData);


            var strings = clientMessage.Split(':');
            var token = strings[0];

            Console.WriteLine(messageType.ToString() + " -- " + clientMessage);

        }


        private void SendMessage(byte[] message)
        {
            if (client == null)
            {
                return;
            }
            try
            {
                // Get a stream object for writing. 			
                NetworkStream stream = client.GetStream();
                if (stream.CanWrite)
                {
                    stream.Write(message, 0, message.Length);

                }
            }
            catch (SocketException socketException)
            {
                Console.WriteLine("Socket exception: " + socketException);
            }
        }

        public void Update()
        {


            RefreshOwnState();

            RefreshViewObjectState();

            //Should do the following:
            //1. Identify the closest target.
            //2. Swivel the barrel to keep aim at it.
            //3. Move within range.
            //4. Fire

            var newClosest = FindClosestTank();

            if (newClosest != null)
            {
                if (currentClosest != null)
                    if (newClosest.Name != currentClosest.Name)
                        Console.WriteLine("New closest enemy: " + newClosest.Name);
                currentClosest = newClosest;
            }

            if (currentClosest != null)
            {
               
                float targetHeading = GetHeading(ownState.X, ownState.Y, currentClosest.X, currentClosest.Y);

                //TurnTurretBasedOnHeading(targetHeading);


                if (GetTurnDir(ownState.TurretHeading, targetHeading))
                {
                    //if (helper.tracker.LastCalledInterval(RequestType.TurretRight) > 100)
                    //    helper.TurretRight(secretToken);


                }
                else
                {
                    //if (helper.tracker.LastCalledInterval(RequestType.TurretLeft) > 100)
                    //    helper.TurretLeft(secretToken);
                }


            }

        }

        double HeadingDiff(double h1, double h2)
        { 
            // angle between two headings
            double diff = h1 - h2 + 3600 % 360;
            return diff <= 180 ? diff : 360 - diff;
        }
    
        /// Left is false
        bool GetTurnDir(double hdg, double newHdg)
        { // should a new heading turn left or right?
            if (newHdg > hdg)
                return newHdg - hdg > 180;
            return hdg - newHdg > 180;
        }

        private void TurnTurretBasedOnHeading(float heading)
        {
            if (Math.Abs(heading - ownState.TurretHeading) < 10)
            {
                //if (helper.tracker.LastCalledInterval(RequestType.StopTurret) > 100)
                //    helper.StopTurret(secretToken);
            }


            if (ownState.TurretHeading > heading)
            {

                //if (helper.tracker.LastCalledInterval(RequestType.TurretRight) > 100)
                //    helper.TurretRight(secretToken);

            }
            else
            {

                //if (helper.tracker.LastCalledInterval(RequestType.TurretLeft) > 100)
                //    helper.TurretLeft(secretToken);

            }





        }

        private void RefreshOwnState()
        {
            //if (helper.tracker.LastCalledInterval(RequestType.GetOwnState) > 200)
            //    ownState = helper.GetTankState(secretToken);

        }

        private void RefreshViewObjectState()
        {
            //make sure we refresh our knowledge of the world every 500 milliseconds
            //if (helper.tracker.LastCalledInterval(RequestType.GetObjectsInView) > 500)
            //    objectState = helper.GetObjectsInView(secretToken);



        }

        private GameObjectState FindClosestTank()
        {
            if (ownState == null)
                return null;

            float closestDistance = float.MaxValue;
            GameObjectState closest = null;
            foreach (GameObjectState s in objectState)
            {
                if (s.Type != "Tank")
                    continue;


                //get distance between us and the object.
                float ownX = ownState.X;
                float ownY = ownState.Y;

                float otherX = s.X;
                float otherY = s.Y;


                float dist = CalculateDistance(ownX, ownY, otherX, otherY);

                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closest = s;
                }

            }

            return closest;
        }

        private float CalculateDistance(float ownX, float ownY, float otherX, float otherY)
        {
            float headingX = otherX - ownX;
            float headingY = otherY - ownY;
            return (float)Math.Sqrt((headingX * headingX) + (headingY * headingY));
        }

        private float GetHeading(float x1, float y1, float x2, float y2)
        {
            float heading = (float)Math.Atan2(y2 - y1, x2 - x1);
            heading = (float)RadianToDegree(heading);
            heading = (heading + 360) % 360;
            return heading;

        }

        private double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }


        private float VectorDot(float x1, float y1, float x2, float y2)
        {
            return (x1 * x2) + (y1 * y2);
        }


        private void BasicMovementTest()
        {
            Thread.Sleep(1000);

            SendMessage(MessageFactory.CreateMessage(NetworkMessageType.forward, secretToken));
            Thread.Sleep(1000);

            SendMessage(MessageFactory.CreateMessage(NetworkMessageType.reverse, secretToken));
            Thread.Sleep(1000);

            SendMessage(MessageFactory.CreateMessage(NetworkMessageType.stop, secretToken));
            Thread.Sleep(1000);

            SendMessage(MessageFactory.CreateMessage(NetworkMessageType.left, secretToken));
            Thread.Sleep(1000);


            SendMessage(MessageFactory.CreateMessage(NetworkMessageType.right, secretToken));
            Thread.Sleep(1000);

            SendMessage(MessageFactory.CreateMessage(NetworkMessageType.stop, secretToken));
            Thread.Sleep(1000);

            SendMessage(MessageFactory.CreateMessage(NetworkMessageType.turretLeft, secretToken));
            Thread.Sleep(1000);

            SendMessage(MessageFactory.CreateMessage(NetworkMessageType.turretRight, secretToken));
            Thread.Sleep(1000);

            SendMessage(MessageFactory.CreateMessage(NetworkMessageType.stopTurret, secretToken));
            Thread.Sleep(1000);

            SendMessage(MessageFactory.CreateMessage(NetworkMessageType.fire, secretToken));

        }

        private void PrintState()
        {
            Console.WriteLine("POS: " + ownState.X + ":" + ownState.Y);
            Console.WriteLine("HEADING: " + ownState.Heading);
            Console.WriteLine("TURRET: " + ownState.TurretHeading);
        }

    }

    public enum RequestType
    {
        GetObjectsInView,
        GetOwnState,
        TurretLeft,
        TurretRight,
        Left,
        Right,
        Forward,
        Backward,
        Fire,
        Stop,
        StopTurret
    }





}
