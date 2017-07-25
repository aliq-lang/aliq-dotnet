namespace RemoteBackEnd
{
    public sealed class Server
    {
        public Server(DataBinding dataBinding)
        {
            DataBinding = dataBinding;
        }

        private DataBinding DataBinding { get; }
    }
}
