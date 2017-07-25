using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AliqWorker
{

    public static class MessageBroadcaster
    {
        private static TopicClient Broadcaster = TopicClient.CreateFromConnectionString(ConnectionLinksContainer.SenderConnectionString, ConnectionLinksContainer.SubscriptionTopicName);
        private static Object padlock = new Object();
        public static void Broadcast(BrokeredMessage message)
        {
            lock (padlock)
            {

                Broadcaster.Send(message);
            }
        }
    }
}
