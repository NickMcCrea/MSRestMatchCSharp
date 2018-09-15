using MSRestMatchCSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;


public class TCPTestClient
{
    #region private members 	
    private TcpClient client;
    private Thread clientReceiveThread;
    private Timer messageTimer;
    private int number;
    #endregion
    // Use this for initialization 	
    public TCPTestClient()
    {
        number = new Random().Next(0, 100);

        ConnectToTcpServer();

        
     
       // messageTimer = new Timer(x => { SendMessage(); }, null, 0, 1000);


        SendMessage(APIHelper.CreateTank("NickTank", "secretToken", "#EA9414"));

    }
    // Update is called once per frame
    void Update()
    {

      

    }
    /// <summary> 	
    /// Setup socket connection. 	
    /// </summary> 	
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
    /// <summary> 	
    /// Runs in background clientReceiveThread; Listens for incomming data. 	
    /// </summary>     
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
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        // Convert byte array to string message. 						
                        string serverMessage = Encoding.ASCII.GetString(incommingData);
                        Console.WriteLine("server message received as: " + serverMessage);
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Console.WriteLine("Socket exception: " + socketException);
        }
    }
    /// <summary> 	
    /// Send message to server using socket connection. 	
    /// </summary> 	
    private void SendMessage()
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
                string clientMessage = "This is a message from " + number;
                // Convert string message to byte array.                 
                byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
                // Write byte array to socketConnection stream.     

                var newArray = APIHelper.AddByteStartOfToArray(clientMessageAsByteArray, 0);

                stream.Write(newArray, 0, newArray.Length);
                
            }
        }
        catch (SocketException socketException)
        {
            Console.WriteLine("Socket exception: " + socketException);
        }
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

   
}