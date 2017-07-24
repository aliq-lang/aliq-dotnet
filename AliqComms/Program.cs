using System;
using Microsoft.ServiceBus.Messaging;

namespace AliqWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Endpoint=sb://aliqserver.servicebus.windows.net/;SharedAccessKeyName=ListeningRightAccessKey;SharedAccessKey=YuIr93zHJMKz9xRhXLyIQmQHxQvOs6LOuIrHfNfsbUE=;EntityPath=aliqqueue";
            var queueName = "aliqqueue";

            var client = QueueClient.CreateFromConnectionString(connectionString, queueName);

            client.OnMessage(message =>
            {
                Console.WriteLine(String.Format("Message body: {0}", message.GetBody<String>()));
                Console.WriteLine(String.Format("Message id: {0}", message.MessageId));
            });
            
            Console.WriteLine("Press ENTER to exit program");
            Console.ReadLine();
        }
    }
}