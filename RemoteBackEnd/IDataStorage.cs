namespace RemoteBackEnd
{
    interface IDataStorage
    {
        bool Exist(string id);

        IDataWriter<T> Create<T>(string id);

        IDataReader<T> Open<T>(string id);
    }
}
