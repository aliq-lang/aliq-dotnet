namespace RemoteBackEnd
{
    /// <summary>
    /// TODO: create two interfaces (INode and IRemoteNode), the first one operates with 
    /// Bag[T] and the second operates with bag ids (for marshalling). Remove DataBinding 
    /// from Node and Server classes and move it to RemoteNode and RemoteServer.
    /// </summary>
    public interface INode
    {
        void Create(string bagId);
    }
}
