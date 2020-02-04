using Presentation.CharacterSelection;
using Presentation.Lobby;
using Presentation.Managers;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class LobbyInstaller : MonoInstaller
    {
        public PlayerInfo playerInfoPrefab;
        
        public override void InstallBindings()
        {
            Container.Bind<LobbyManager>().AsSingle();

            Container.BindFactory<PlayerInfo, PlayerInfo.Factory>().FromComponentInNewPrefab(playerInfoPrefab);
        }
    }
}