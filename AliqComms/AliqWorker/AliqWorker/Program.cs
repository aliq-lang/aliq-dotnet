using System;
using Microsoft.ServiceBus.Messaging;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json.Linq;
using System.IO;

namespace AliqWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataProcessorRole = Task.Factory.StartNew(() =>
            {
                (new ServerBroadcastHandler()).Run();
            });
            Task.WaitAll(dataProcessorRole);
        }
    }
}