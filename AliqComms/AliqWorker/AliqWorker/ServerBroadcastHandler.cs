using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AliqWorker
{
    class ServerBroadcastHandler
    {
        private static SubscriptionClient ServerBroadcastListener = 
            SubscriptionClient.CreateFromConnectionString(ConnectionLinksContainer.ListenerConnectionString, ConnectionLinksContainer.SubscriptionTopicName, "aliqserverbroadcast", ReceiveMode.PeekLock);
        public void Run()
        {
            while (true)
            {
                // listen to messages from the aliq server
                ServerBroadcastListener.OnMessageAsync(serverMessage =>
                {
                    Console.WriteLine(String.Format("Message body: {0}", serverMessage.GetBody<String>()));
                    Console.WriteLine(String.Format("Message id: {0}", serverMessage.MessageId));
                    if ((String)serverMessage.Properties["processingMode"] == "processLocalData")
                    {
                        return Task.Factory.StartNew(() => (new LocalDataProcessingHandler()).Run(serverMessage));
                    }
                    if ((String)serverMessage.Properties["processingMode"] == "processExternalData")
                    {
                        if ((String)serverMessage.Properties["processingNode"] == Environment.MachineName)
                        {
                            return Task.Factory.StartNew(() => (new ExternalDataProcessingHandler()).Run(serverMessage));
                        }
                        else
                        {
                            return Task.Factory.StartNew(() => (new DataUploadHandler()).Run(serverMessage));
                        }
                    }
                    return Task.Factory.StartNew(()=> {
                        // a do-nothing task to avoid runtime exceptions?
                    });
                });
            }
        }
    }

    internal class LocalDataProcessingHandler
    {
        public void Run(BrokeredMessage message)
        {
            // do some local data processing


            // processing done, let's notify the server
            var serverNotification = new BrokeredMessage("Processed local data notification.");
            serverNotification.Properties["processedMessageId"] = message.MessageId;
            serverNotification.Properties["aliqcomms"] = "aliqworkernotification";
            MessageBroadcaster.Broadcast(serverNotification);
        }
    }

    internal class ExternalDataProcessingHandler
    {
        private Object PadLock = new Object();
        int DataResponses = 0;
        public void Run(BrokeredMessage message)
        {
            DataResponses++;
            var workerDataResponseListener = SubscriptionClient.CreateFromConnectionString(ConnectionLinksContainer.ListenerConnectionString, ConnectionLinksContainer.SubscriptionTopicName, "aliqworkersdataresponse", ReceiveMode.PeekLock);
            workerDataResponseListener.OnMessageAsync((workerDataResponseMessage) =>
            {
                return Task.Factory.StartNew(() =>
                {
                    if ((String)workerDataResponseMessage.Properties["requestMessageId"] != message.MessageId)
                    {
                        return;
                    }
                    lock (PadLock)
                    {
                        DataResponses++;
                    }
                   
                    var downloadLink = (String)workerDataResponseMessage.Properties["downloadLink"];
                    if(string.IsNullOrEmpty(downloadLink))
                    {
                        return;
                    }
                    // now download from this link, unzip if necessary and then delete the blob
                    // do some processing with this data


                    // finally, if data from all nodes has been consumed, notify the server
                    if (DataResponses == ConnectionLinksContainer.NumberOfActiveWorkerNodes)
                    {
                        var serverNotification = new BrokeredMessage("Processed external data notification.");
                        serverNotification.Properties["processedMessageId"] = message.MessageId;
                        serverNotification.Properties["aliqcomms"] = "aliqworkernotification";
                    }
                });
                
            });
        }
    }

    internal class DataUploadHandler
    {
        public void Run(BrokeredMessage message)
        {
            Console.WriteLine(String.Format("Message body: {0}", message.GetBody<String>()));
            Console.WriteLine(String.Format("Message id: {0}", message.MessageId));
            var fileList = (String)message.Properties["fileList"];
            var downloadLink = string.Empty;
            // check if requested files are available

            // upload if available and set the downloadLink to uploaded spot
            var uploadNotification = new BrokeredMessage("Data uploaded notification to requesting worker");
            uploadNotification.Properties["requestMessageId"] = message.MessageId;
            uploadNotification.Properties["downloadLink"] = downloadLink;
            MessageBroadcaster.Broadcast(uploadNotification);
        }

        private String UploadFiles(String fileList)
        {
            // return the location where we uploaded the files
            return string.Empty;
        }

    }
}
