using System;
using Components;
using Controllers;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class PlayerBehaviour : NetworkBehaviour
    {
        private Camera _cam;
        private PositionData _position;
        private bool _destroied;
        private Material _material;

        private const string Handler = "UpdatePosition";

        [Inject] private CharactersController _charactersController;

        protected override void Awake()
        {
            base.Awake();

            _cam = Camera.main;

            _material = transform.GetComponentInChildren<Renderer>().material;
            
            transform.position = Vector3.zero;
            
            _position = new PositionData(transform.position);
        }

        private void Update()
        {
            //Todo: temporary
            if(_destroied) return;
            
            if (IsLocal)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    var mouse = Input.mousePosition;
                    var pos = _cam.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, 2));

                    _position = new PositionData(pos);
                    
                    NetworkController.SendToAll(Handler, Id, _position);

                    transform.position = pos;
                }
            }
            else
            {
                SyncPosition();
            }
        }

        private void SyncPosition()
        {
            var pos = _position.GetPosition();
            
            transform.position = pos;
        }

        protected override void Setup()
        {
            base.Setup();

            _material.color = _charactersController.GetCharacter(Client.SelectedCharacter).GetColor();
            
            if (IsLocal)
            {
                NetworkController.SendToAll(Handler, Id, _position);
            }
            else
            {
                NetworkController.RegisterHandler<PositionData>(Handler, Id, ReceivePosition);
                
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            //Todo: temporary
            _destroied = true;
            
            NetworkController.UnregisterHandler(Handler);
        }

        private void ReceivePosition(PositionData pos)
        {
            _position = pos;
        }
    }
}