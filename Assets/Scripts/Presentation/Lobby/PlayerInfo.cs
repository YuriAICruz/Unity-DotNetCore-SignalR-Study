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

        public void Setup(NetworkClient player)
        {
            _manager.OnPlayerUpdate += UpdateData;

            UpdateData(player);
        }

        private void UpdateData(NetworkClient player)
        {
            userName.text = player.userName;
            status.text = player.Status.ToString();

            characterColor.color = _manager.GetCharacter(player.SelectedCharacter).GetColor();
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