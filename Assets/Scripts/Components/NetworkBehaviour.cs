using System;
using Graphene.SharedModels.Network;
using Graphene.SignalR;
using UnityEngine;
using Zenject;

namespace Components
{
    [RequireComponent(typeof(NetworkId))]
    public class NetworkBehaviour : MonoBehaviour
    {
        [Inject] protected NetworkClientManager NetworkController;
        protected NetworkId NetworkId;
        public NetworkClient Client { get; private set; }

        private const string CreateHandler = "CreatedBehaviour";
        private const string DestroyHandler = "DestriedBehaviour";

        public bool IsLocal
        {
            get => NetworkId.isLocal;
        }

        public Guid Id
        {
            get => NetworkId.Id;
        }


        protected virtual void Awake()
        {
            GetNetId();
        }

        protected virtual void Start()
        {
            NetworkId.Initialize();
        }

        protected virtual void OnDestroy()
        {
            NetworkController.SendToAll(DestroyHandler, Id, Client.userName);
        }

        private void GetNetId()
        {
            if (NetworkId == null)
            {
                NetworkId = GetComponent<NetworkId>();

                if (NetworkId == null)
                {
                    NetworkId = gameObject.AddComponent<NetworkId>();
                }
            }
        }

        private void Initialize(NetworkClient client)
        {
            GetNetId();

            Client = client;

            NetworkId.Initialize();

            SyncNetwork();

            Setup();
        }

        private void Initialize(NetworkClient client, Guid id)
        {
            GetNetId();

            Client = client;

            NetworkId.Initialize(id);

            Setup();
        }

        public void SyncNetwork()
        {
            NetworkController.SendToAll(CreateHandler, Id, Client.userName);
        }

        protected virtual void Setup()
        {
        }

        public class Factory : PlaceholderFactory<NetworkBehaviour>
        {
            private void Setup(NetworkBehaviour behaviour)
            {
                behaviour.name = "NetworkBehaviourClone";
                behaviour.transform.SetParent(null, false);
            }

            public override NetworkBehaviour Create()
            {
                throw new NotImplementedException();
            }

            public NetworkBehaviour Create(NetworkClient client)
            {
                var behaviour = base.Create();
                Setup(behaviour);
                behaviour.Initialize(client);

                return behaviour;
            }

            public NetworkBehaviour Create(NetworkClient client, Guid id)
            {
                var behaviour = base.Create();
                Setup(behaviour);
                behaviour.Initialize(client, id);

                return behaviour;
            }
        }
    }
}