using System.Collections.Generic;

namespace Graphene.SharedModels.Network
{
    public class NetworkClients
    {
        private List<NetworkClient> _connections = new List<NetworkClient>();

        public int Count => _connections.Count;

        public int FindIndex(string userName)
        {
            return _connections.FindIndex(x => x.userName == userName);
        }

        public void RemoveAt(int i)
        {
            _connections.RemoveAt(i);
        }
        
        public void Remove(string userName)
        {
            var conn = _connections.Find(x => x.userName == userName);
            _connections.Remove(conn);
        }
        
        public void Remove(string userName, string id)
        {
            var i = _connections.FindIndex(x => x.userName == userName);
            _connections[i].Remove(id);
            if(_connections[i].Count <= 0)
                Remove(userName);
        }

        public int Add(string userName, string id)
        {
            var i = _connections.FindIndex(x => x.userName == userName);

            if (i >= 0)
            {
                _connections[i].Add(id);
            }
            else
            {
                _connections.Add(new NetworkClient(userName, new List<string>(){id}));

                i = _connections.Count;
            }
            
            return i;
        }

        public NetworkClient this[int i] => _connections[i];
    }
}