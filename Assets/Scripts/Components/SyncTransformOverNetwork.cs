using Controllers;
using Graphene.SignalR;
using UnityEngine;
using Zenject;

namespace Components
{
    
    public class SyncTransformOverNetwork : NetworkBehaviour
    {

        private Transform _transform;
        private TransformData _transformData;

        private const string Handler = "TransformUpdate";

        protected override void Awake()
        {
            base.Awake();
            _transformData = new TransformData();
        }

        protected override void Start()
        {
            base.Start();

            _transform = transform;

            if (!IsLocal)
            {
                NetworkController.RegisterHandler<TransformData>(Handler, NetworkId.Id, UpdateTransform);
            }
            else
            {
                Sync();
            }
        }

        private void Update()
        {
            if (IsLocal)
            {
                Sync();
            }
            else
            {
                UpdateTransform();
            }
        }

        private void Sync()
        {
            if (_transformData.Changed(_transform))
            {
                _transformData.SetTransform(_transform);

                NetworkController.SendToAll(Handler, NetworkId.Id, _transformData);
            }
        }

        private void UpdateTransform(TransformData transformData)
        {
            _transformData = transformData;
        }

        private void UpdateTransform()
        {
            _transform.position = _transformData.GetPosition();
            _transform.rotation = _transformData.GetRotation();
            _transform.localScale = _transformData.GetScale();
        }
    }
}