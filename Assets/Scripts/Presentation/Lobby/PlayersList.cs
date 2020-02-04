using System;
using System.Collections.Generic;
using Graphene.SharedModels.Network;
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
        private List<PlayerInfo> _players;

        private void Awake()
        {
            _manager.OnPlayerConnected += AddPlayer;
            _manager.OnPlayerDisconnected += RemovePlayer;

            _players = new List<PlayerInfo>();

            foreach (var player in _manager.GetAvailablePlayers())
            {
                AddPlayer(player);
            }
        }

        private void OnDestroy()
        {
            _manager.OnPlayerConnected -= AddPlayer;
            _manager.OnPlayerDisconnected -= RemovePlayer;
        }

        private void AddPlayer(NetworkClient player)
        {
            var pl = _playerFactory.Create(root);
            pl.Setup(player);
            _players.Add(pl);
        }

        private void RemovePlayer(string userName)
        {
            var i = _players.FindIndex(x => x.Player.userName == userName);

            Destroy(_players[i].gameObject);

            _players.RemoveAt(i);
        }
    }
}