using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSRestMatchCSharp
{
    public class NickTankBot
    {

        string secretToken;
        public bool botQuit = false;
        RestMatchApiHelper helper;
        List<GameObjectState> objectState;
        GameObjectState ownState;
        GameObjectState currentClosest;

        public NickTankBot(string url, int port, string name, string secretCode, string colour, float startX, float startZ, float angle)
        {


            helper = new RestMatchApiHelper(url, port);
            secretToken = secretCode;

            //despawn first in case we're re-running
            //helper.DespawnTank(secretToken);


            Thread.Sleep(1000);

            helper.CreateTank(name, secretToken, colour, startX, startZ, 0);

            //BasicMovementTest();




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
                    if (helper.tracker.LastCalledInterval(RequestType.TurretRight) > 100)
                        helper.TurretRight(secretToken);


                }
                else
                {
                    if (helper.tracker.LastCalledInterval(RequestType.TurretLeft) > 100)
                        helper.TurretLeft(secretToken);
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
                if (helper.tracker.LastCalledInterval(RequestType.StopTurret) > 100)
                    helper.StopTurret(secretToken);
            }


            if (ownState.TurretHeading > heading)
            {

                if (helper.tracker.LastCalledInterval(RequestType.TurretRight) > 100)
                    helper.TurretRight(secretToken);

            }
            else
            {

                if (helper.tracker.LastCalledInterval(RequestType.TurretLeft) > 100)
                    helper.TurretLeft(secretToken);

            }





        }

        private void RefreshOwnState()
        {
            if (helper.tracker.LastCalledInterval(RequestType.GetOwnState) > 200)
                ownState = helper.GetTankState(secretToken);

        }

        private void RefreshViewObjectState()
        {
            //make sure we refresh our knowledge of the world every 500 milliseconds
            if (helper.tracker.LastCalledInterval(RequestType.GetObjectsInView) > 500)
                objectState = helper.GetObjectsInView(secretToken);



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

            helper.Forward(secretToken);
            Thread.Sleep(1000);

            helper.Reverse(secretToken);
            Thread.Sleep(1000);

            helper.Stop(secretToken);

            helper.Left(secretToken);
            Thread.Sleep(1000);

            RefreshOwnState();
            PrintState();
            helper.Right(secretToken);
            Thread.Sleep(1000);

            helper.Stop(secretToken);

            helper.TurretLeft(secretToken);
            Thread.Sleep(1000);
            RefreshOwnState();
            PrintState();
            helper.TurretRight(secretToken);
            Thread.Sleep(1000);

            helper.StopTurret(secretToken);
            RefreshOwnState();
            PrintState();
            helper.Fire(secretToken);
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
