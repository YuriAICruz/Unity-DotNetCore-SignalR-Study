using System;
using Presentation.Managers;
using UnityEngine;
using Zenject;

namespace Presentation.Lobby
{
    public class PlayersList : MonoBehaviour
    {
        public Transform root;

        [Inject] private LobbyManager _manager;
        [Inject] private PlayerInfo.Factory _playerFactory;
        
        private void Awake()
        {
            foreach (var player in _manager.GetAvailablePlayers())
            {
                var pl = _playerFactory.Create(root);
                pl.Setup(player);
            }
        }
    }
}