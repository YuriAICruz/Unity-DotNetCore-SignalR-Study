using System;
using Graphene.SharedModels.Network;
using Presentation.Managers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Presentation.Lobby
{
    public class PlayerInfo : MonoBehaviour
    {
        [Inject] private LobbyManager _manager;

        public Text userName;
        public Text status;
        public Image characterColor;
        public NetworkClient Player { get; private set; }

        public void Setup(NetworkClient player)
        {
            Player = player;
            _manager.OnPlayerUpdate += UpdateData;

            UpdateData(player);
        }

        private void OnDestroy()
        {
            _manager.OnPlayerUpdate -= UpdateData;
        }

        private void UpdateData(NetworkClient player)
        {
            if (Player.userName != player.userName) return;

            userName.text = player.userName;
            status.text = player.Status.ToString();

            var character = _manager.GetCharacter(player.SelectedCharacter);
            
            if (character == null) return;
            
            characterColor.color = character.GetColor();
        }

        public class Factory : PlaceholderFactory<PlayerInfo>
        {
            public PlayerInfo Create(Transform transform)
            {
                var instance = base.Create();

                instance.transform.SetParent(transform);
                instance.transform.localScale = Vector3.one;

                return instance;
            }
        }
    }
}