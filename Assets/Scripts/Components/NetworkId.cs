using System;
using UnityEngine;
using Zenject;

namespace Components
{
    public class NetworkId : MonoBehaviour
    {
        private Guid _id;

        public Guid Id
        {
            get => _id;
            private set => _id = value;
        }

        public bool isLocal { get; private set; }

        private bool init;


        private void GenerateId()
        {
            if (init) return;

            init = true;
            isLocal = true;
            Id = Guid.NewGuid();
        }

        public void Initialize()
        {
            if (init) return;
            
            GenerateId();
        }

        public void Initialize(Guid id)
        {
            Id = id;
            isLocal = false;
            init = true;

            Initialize();
        }
    }
}