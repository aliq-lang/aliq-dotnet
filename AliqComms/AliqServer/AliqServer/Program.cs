using System;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

using System.IO;
using Newtonsoft.Json.Linq;

namespace AliqServer
{
    public class Program
    {
        private static readonly string ListenerConnectionString = (string)JObject.Parse(File.ReadAllText(@"C:\Users\deshank\Documents\hackathon2017\secrets\secrets.json"))["broadcastListenerLink"];
        private static readonly string SenderConnectionString = (string)JObject.Parse(File.ReadAllText(@"C:\Users\deshank\Documents\hackathon2017\secrets\secrets.json"))["broadcastSenderLink"];
        private static readonly string TopicName = "aliqcomms";

        public static void Main(string[] args)
        {
            
            var client = TopicClient.CreateFromConnectionString(SenderConnectionString, TopicName);

            var message = new BrokeredMessage("Aliq Server to Aliq Worker, over!");
            message.Properties["aliqcomms"] = "aliqserverbroadcast";
            
            client.Send(message);
            Console.WriteLine(String.Format("Message body: {0}", message.GetBody<String>()));
            Console.WriteLine(String.Format("Message id: {0}", message.MessageId));

            Console.ReadKey();

            // listen to messages from the aliq server
            var workerBroadcastListener = SubscriptionClient.CreateFromConnectionString(ListenerConnectionString, TopicName, "aliqworkernotifications", ReceiveMode.ReceiveAndDelete);
            workerBroadcastListener.OnMessage((serverMessage) =>
            {
                    Console.WriteLine(String.Format("Message body: {0}", serverMessage.GetBody<String>()));
                    Console.WriteLine(String.Format("Message id: {0}", serverMessage.MessageId));
            });
            


            Console.WriteLine("Message successfully sent! Press ENTER to exit program");
            Console.ReadLine();
        }
    }
}
