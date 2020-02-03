using System.Collections.Generic;

namespace Graphene.SharedModels.Network
{
    public class NetworkClient
    {
        public readonly string userName;
        private readonly List<string> connectionId;

        public NetworkClient(string userName, List<string> connectionId)
        {
            this.userName = userName;
            this.connectionId = connectionId;
        }

        public int Count => connectionId.Count;

        public void Remove(string id)
        {
            connectionId.Remove(id);
        }

        public void Add(string id)
        {
            connectionId.Add(id);
        }
    }
}