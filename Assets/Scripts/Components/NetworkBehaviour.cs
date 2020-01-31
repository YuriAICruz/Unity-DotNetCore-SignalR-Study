using System;
using UnityEngine;

namespace Components
{
    public class NetworkBehaviour : MonoBehaviour
    {
        protected Guid _id;
        protected bool isLocal;
        private bool init;

        protected virtual void Start()
        {
            GenerateId();
        }

        private void GenerateId()
        {
            if (init) return;

            init = true;
            isLocal = true;
            _id = Guid.NewGuid();
        }

        public void Initialize(Guid id)
        {
            _id = id;
            isLocal = false;
            init = true;
        }

        public Guid GetId()
        {
            GenerateId();
            
            return _id;
        }
    }
}