using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;

namespace MSRestMatchCSharp
{
    class RestMatchApiHelper
    {

        private RestClient client;
        public LastRequestTracker tracker;

        public RestMatchApiHelper(string baseUrl, int port)
        {
            client = new RestClient(baseUrl + ":" + port + "/");
            tracker = new LastRequestTracker();


        }

        internal void CreateTank(string displayName, string secretToken, string hexColour, float x, float y, float angle)
        {

            RestRequest r = new RestRequest("tank/createtest/", Method.POST);
            r.RequestFormat = DataFormat.Json;
            r.AddBody(new { Name = displayName, Token = secretToken, Color = hexColour, X = x, Y = y, Angle = angle });
            var response = client.Execute(r);
        }

        internal void DespawnTank(string token)
        {
            RestRequest r = new RestRequest("tank/" + token + "/despawn/", Method.POST);
            r.RequestFormat = DataFormat.Json;
            r.AddBody(new { });
            var response = client.Execute(r);
        }

        internal void Fire(string token)
        {
            tracker.UpdateLastCall(RequestType.Fire);
            RestRequest r = new RestRequest("tank/" + token + "/fire/", Method.POST);
            r.RequestFormat = DataFormat.Json;
            r.AddBody(new { });
            var response = client.Execute(r);
        }

        internal void Forward(string token)
        {
            tracker.UpdateLastCall(RequestType.Forward);
            RestRequest r = new RestRequest("tank/" + token + "/forward/", Method.POST);
            r.RequestFormat = DataFormat.Json;
            r.AddBody(new { });
            var response = client.Execute(r);
        }

        internal void Reverse(string token)
        {
            tracker.UpdateLastCall(RequestType.Backward);
            RestRequest r = new RestRequest("tank/" + token + "/reverse/", Method.POST);
            r.RequestFormat = DataFormat.Json;
            r.AddBody(new { });
            var response = client.Execute(r);
        }

        internal void Left(string token)
        {
            tracker.UpdateLastCall(RequestType.Left);
            RestRequest r = new RestRequest("tank/" + token + "/left/", Method.POST);
            r.RequestFormat = DataFormat.Json;
            r.AddBody(new { });
            var response = client.Execute(r);
        }

        internal void Right(string token)
        {
            tracker.UpdateLastCall(RequestType.Right);
            RestRequest r = new RestRequest("tank/" + token + "/right/", Method.POST);
            r.RequestFormat = DataFormat.Json;
            r.AddBody(new { });
            var response = client.Execute(r);
        }

        internal void Stop(string token)
        {
            tracker.UpdateLastCall(RequestType.StopTurret);
            RestRequest r = new RestRequest("tank/" + token + "/stop/", Method.POST);
            r.RequestFormat = DataFormat.Json;
            r.AddBody(new { });
            var response = client.Execute(r);
        }

        internal void TurretLeft(string token)
        {
            tracker.UpdateLastCall(RequestType.TurretLeft);
            RestRequest r = new RestRequest("tank/" + token + "/turretleft/", Method.POST);
            r.RequestFormat = DataFormat.Json;
            r.AddBody(new { });
            var response = client.Execute(r);
        }

        internal void TurretRight(string token)
        {
            tracker.UpdateLastCall(RequestType.TurretRight);
            RestRequest r = new RestRequest("tank/" + token + "/turretright/", Method.POST);
            r.RequestFormat = DataFormat.Json;
            r.AddBody(new { });
            var response = client.Execute(r);
        }

        internal void StopTurret(string token)
        {
            tracker.UpdateLastCall(RequestType.StopTurret);
            RestRequest r = new RestRequest("tank/" + token + "/stopturret/", Method.POST);
            r.RequestFormat = DataFormat.Json;
            r.AddBody(new { });
            var response = client.Execute(r);
        }

        internal GameObjectState GetTankState(string token)
        {
            Console.WriteLine("GetTankState");
            tracker.UpdateLastCall(RequestType.GetOwnState);
            RestRequest r = new RestRequest("tank/" + token + "/state/", Method.GET);
            r.RequestFormat = DataFormat.Json;
            var response = client.Execute(r);
            return JsonConvert.DeserializeObject<GameObjectState>(response.Content);
        }

        internal List<GameObjectState> GetObjectsInView(string token)
        {
            Console.WriteLine("GetObjectsInView");
            tracker.UpdateLastCall(RequestType.GetObjectsInView);
            RestRequest r = new RestRequest("tank/" + token + "/fieldofview/", Method.GET);
            r.RequestFormat = DataFormat.Json;
            var response = client.Execute(r);
            return JsonConvert.DeserializeObject<List<GameObjectState>>(response.Content);

        }

    }

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
}
