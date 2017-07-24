using System;

namespace AliqServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var client = new ServerBusClient();
            client.SendMessage();
        }
    }
}
