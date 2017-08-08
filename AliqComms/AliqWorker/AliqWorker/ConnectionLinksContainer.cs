using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AliqWorker
{
    public static class ConnectionLinksContainer
    {
        private static string _listenerConnectionString;
        public static string ListenerConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_listenerConnectionString))
                {
                    _listenerConnectionString = (string)JObject.Parse(File.ReadAllText(@"C:\Users\deshank\Documents\hackathon2017\secrets\secrets.json"))["broadcastListenerLink"];
                }
                return _listenerConnectionString;
            }
        }




        public static string SenderConnectionString { get { return (string)JObject.Parse(File.ReadAllText(@"C:\Users\deshank\Documents\hackathon2017\secrets\secrets.json"))["broadcastSenderLink"]; } }

        public static string StorageConnectionString { get { return (string)JObject.Parse(File.ReadAllText(@"C:\Users\deshank\Documents\hackathon2017\secrets\secrets.json"))["storageConnectionLink"]; } }

        // the topic server broadcasts to for workers to listen
        public static string SubscriptionTopicName { get { return "aliqcomms"; } }

        public static int NumberOfActiveWorkerNodes { get { return 1; } }

    }
}
