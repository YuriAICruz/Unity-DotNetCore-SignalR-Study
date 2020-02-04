using System;
using System.Collections.Generic;

namespace Graphene.SharedModels.Network
{
    public class NetworkClients
    {
        private readonly string _userName;
        private readonly List<NetworkClient> _connections;

        public IReadOnlyList<NetworkClient> Clients => _connections;

        public NetworkClient Self => _ownerId >= 0 ? _connections[_ownerId] : null;

        private int _ownerId = -1;

        public NetworkClients(string userName)
        {
            _connections = new List<NetworkClient>();
            _userName = userName;
        }

        public int Count => _connections.Count;


        public NetworkClient Find(string userName)
        {
            return _connections.Find(x => x.userName == userName);
        }
        
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

            if (i < 0)
                throw new NullReferenceException();

            _connections[i].Remove(id);
            if (_connections[i].GetCount() <= 0)
                Remove(userName);
        }


        public void AddSelf(string userName, string id)
        {
            _ownerId = Add(userName, id);
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
                _connections.Add(new NetworkClient(userName, new List<string>() {id}));

                i = _connections.Count - 1;
            }

            return i;
        }

        private void Add(NetworkClient client)
        {
            if (_connections.Contains(client)) return;

            _connections.Add(client);
        }

        public NetworkClient this[int i] => _connections[i];

        public void Update(NetworkClient client)
        {
            var player = _connections.Find(x => x.userName == client.userName);

            if (player != null)
                player.Update(client);
            else
                Add(client);
        }
    }
}