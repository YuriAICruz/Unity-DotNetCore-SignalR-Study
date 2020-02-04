using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Graphene.SharedModels.Network
{
    public enum ClientStatus
    {
        Waiting = 0,
        Ready = 1
    }

    public class NetworkClient
    {
        public readonly string userName;

        [JsonProperty(TypeNameHandling = TypeNameHandling.None)]
        private readonly List<string> connectionId;

        private bool _changed;
        
        [JsonProperty(TypeNameHandling = TypeNameHandling.None)]
        public ClientStatus Status { get; private set; }

        public int SelectedCharacter { get; private set; }

        public NetworkClient()
        {
        }

        public NetworkClient(string userName, List<string> connectionId)
        {
            this.userName = userName;
            this.connectionId = connectionId;
        }

        [JsonConstructor]
        public NetworkClient(string userName, List<string> connectionId, int selectedCharacter, ClientStatus status)
        {
            this.userName = userName;
            this.connectionId = connectionId;
            SelectedCharacter = selectedCharacter;
            Status = status;
        }

        public int GetCount()
        {
            return connectionId.Count;
        }

        public bool IsDirty()
        {
            return _changed;
        }

        public void Remove(string id)
        {
            connectionId.Remove(id);
        }

        public void Add(string id)
        {
            connectionId.Add(id);
        }

        public void SelectCharacter(int id)
        {
            SelectedCharacter = id;
            _changed = true;
        }

        public void SetStatus(ClientStatus status)
        {
            Status = status;
            _changed = true;
        }

        public void Update(NetworkClient client)
        {
            SelectedCharacter = client.SelectedCharacter;
            Status = client.Status;

            for (int i = 0; i < client.connectionId.Count; i++)
            {
                if (connectionId.Count <= i)
                {
                    connectionId.Add(client.connectionId[i]);
                    continue;
                }

                connectionId[i] = client.connectionId[i];
            }

            _changed = false;
        }

        public override string ToString()
        {
            return $"user: {userName}, ids: {connectionId.Count}, character: {SelectedCharacter} - {Status}";
        }
    }
}