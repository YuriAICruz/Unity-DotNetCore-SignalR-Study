using System;
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

        private void Initialize()
        {
            GetNetId();

            NetworkId.Initialize();
        }

        private void Initialize(Guid id)
        {
            GetNetId();

            NetworkId.Initialize(id);
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
                var behaviour = base.Create();
                Setup(behaviour);
                behaviour.Initialize();

                return behaviour;
            }

            public NetworkBehaviour Create(Guid id)
            {
                var behaviour = base.Create();
                Setup(behaviour);
                behaviour.Initialize(id);

                return behaviour;
            }
        }
    }
}