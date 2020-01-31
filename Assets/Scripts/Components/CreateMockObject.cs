using System;
using UnityEngine;

namespace Components
{
    public class CreateMockObject : MonoBehaviour
    {
        public SyncTransformOverNetwork reference;

        private void Start()
        {
            CreateMock();
        }

        private void CreateMock()
        {
            var obj = new GameObject("Mock", new[]
            {
                typeof(SyncTransformOverNetwork)
            });

            var sync = obj.GetComponent<SyncTransformOverNetwork>();
            sync._networkController = reference._networkController;
            sync.Initialize(reference.GetId());

            for (int i = 0, n = transform.childCount; i < n; i++)
            {
                var child = transform.GetChild(0);
                child.SetParent(obj.transform);
                child.localPosition = Vector3.zero;
            }
        }
    }
}