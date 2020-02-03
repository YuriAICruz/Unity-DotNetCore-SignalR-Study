using System;
using UnityEngine;
using Zenject;

namespace Components
{
    public class CreateMockObject : MonoBehaviour
    {
        public SyncTransformOverNetwork reference;

        [Inject] private NetworkBehaviour.Factory _factory;

        private void Start()
        {
            CreateMock();
        }

        private void CreateMock()
        {
            var sync = _factory.Create(reference.Id);
            
            //var sync = obj.GetComponent<SyncTransformOverNetwork>();
            //sync.NetworkController = reference.NetworkController;

            for (int i = 0, n = transform.childCount; i < n; i++)
            {
                var child = transform.GetChild(0);
                child.SetParent(sync.transform);
                child.localPosition = Vector3.zero;
            }
        }
    }
}