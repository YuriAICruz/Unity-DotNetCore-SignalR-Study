using System;
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

    [Serializable]
    internal class TransformData
    {
        public float[] data;

        public TransformData()
        {
            data = new float[10];
        }

        public Vector3 GetPosition()
        {
            return new Vector3(data[0], data[1], data[2]);
        }

        public Quaternion GetRotation()
        {
            return new Quaternion(data[3], data[4], data[5], data[6]);
        }

        public Vector3 GetScale()
        {
            return new Vector3(data[7], data[8], data[9]);
        }

        public void SetTransform(Transform transform)
        {
            var pos = transform.position;
            data[0] = pos.x;
            data[1] = pos.y;
            data[2] = pos.z;
            var rot = transform.rotation;
            data[3] = rot.x;
            data[4] = rot.y;
            data[5] = rot.z;
            data[6] = rot.w;
            var scl = transform.localScale;
            data[7] = scl.x;
            data[8] = scl.y;
            data[9] = scl.z;
        }

        public bool Changed(Transform transform)
        {
            var sum = 0f;
            var pos = transform.position;
            sum += data[0] - pos.x;
            sum += data[1] - pos.y;
            sum += data[2] - pos.z;
            var rot = transform.rotation;
            sum += data[3] - rot.x;
            sum += data[4] - rot.y;
            sum += data[5] - rot.z;
            sum += data[6] - rot.w;
            var scl = transform.localScale;
            sum += data[7] - scl.x;
            sum += data[8] - scl.y;
            sum += data[9] - scl.z;

            return Math.Abs(sum) > 0.1f;
        }
    }
}