using System;
using Components;
using Controllers;
using Graphene.ApiCommunication;
using Graphene.ApiCommunication.Installer;
using UnityEngine;
using Zenject;

namespace Installers
{
    [CreateAssetMenu(fileName = "ApiIntegration", menuName = "Installers/ApiIntegrationInstaller")]
    public class ApiIntegrationInstaller: ScriptableObjectInstaller<ApiInstaller>
    {
        public NetworkingSettings settings;
        
        public override void InstallBindings()
        {
            Container.BindInstance(settings);
            
            Container.Bind<AuthenticationController>().AsSingle();
            Container.Bind<CharactersController>().AsSingle();

//            Container.BindIFactory<NetworkFactory>().FromComponentInNewPrefab(settings.playerPrefab);
//            Container.Bind<NetworkBehaviour>().FromFactory<NetworkFactory>();
            Container.BindFactory<NetworkBehaviour, NetworkBehaviour.Factory>().FromComponentInNewPrefab(settings.playerPrefab);
        }
    }
}