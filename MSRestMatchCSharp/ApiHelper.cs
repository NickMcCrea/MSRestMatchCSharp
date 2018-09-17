using System;
using System.Collections.Generic;
using RestSharp;
using Newtonsoft.Json;
using System.Text;

namespace MSRestMatchCSharp
{

    public class GameObjectState
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float ForwardX { get; set; }
        public float ForwardY { get; set; }
        public float Heading { get; set; }
        public float TurretHeading { get; set; }
        public float TurretForwardX { get; set; }
        public float TurretForwardY { get; set; }
        public int Health { get; set; }
        public int Ammo { get; set; }
    }

    class LastRequestTracker
    {
        private Dictionary<RequestType, DateTime> lastRequestTracker;

        public LastRequestTracker()
        {
            lastRequestTracker = new Dictionary<RequestType, DateTime>();
            lastRequestTracker.Add(RequestType.GetObjectsInView, DateTime.Now);
            lastRequestTracker.Add(RequestType.GetOwnState, DateTime.Now);
            lastRequestTracker.Add(RequestType.Backward, DateTime.Now);
            lastRequestTracker.Add(RequestType.Fire, DateTime.Now);
            lastRequestTracker.Add(RequestType.Forward, DateTime.Now);
            lastRequestTracker.Add(RequestType.Left, DateTime.Now);
            lastRequestTracker.Add(RequestType.Right, DateTime.Now);
            lastRequestTracker.Add(RequestType.TurretLeft, DateTime.Now);
            lastRequestTracker.Add(RequestType.TurretRight, DateTime.Now);
            lastRequestTracker.Add(RequestType.StopTurret, DateTime.Now);
        }

        internal double LastCalledInterval(RequestType type)
        {
            return (DateTime.Now - lastRequestTracker[type]).TotalMilliseconds;
        }

        internal void UpdateLastCall(RequestType type)
        {
            lastRequestTracker[type] = DateTime.Now;
        }
    }

    public static class MessageFactory
    {

        public static byte[] CreateTankMessage(string name, string token, string color)
        {
            string message = token + ":" + name + ":" + color;
            byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(message);
            return AddByteStartOfToArray(clientMessageAsByteArray, 1);
        }

        public static byte[] AddByteStartOfToArray(byte[] bArray, byte newByte)
        {
            byte[] newArray = new byte[bArray.Length + 1];
            bArray.CopyTo(newArray, 1);
            newArray[0] = newByte;
            return newArray;
        }

        public static byte[] CreateMessage(NetworkMessageType type, string token)
        {
            string message = token;
            byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(message);
            return AddByteStartOfToArray(clientMessageAsByteArray, (byte)type);
        }


    }

    public enum NetworkMessageType
    {
        test = 0,
        createTank = 1,
        despawnTank = 2,
        fire = 3,
        forward = 4,
        reverse = 5,
        left = 6,
        right = 7,
        stop = 8,
        turretLeft = 9,
        turretRight = 10,
        stopTurret = 11,
        objectUpdate = 12
    }

}


