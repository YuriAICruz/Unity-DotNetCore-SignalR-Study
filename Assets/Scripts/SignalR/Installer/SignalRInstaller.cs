using System.ComponentModel;
using Graphene.ApiCommunication;
using Graphene.ApiCommunication.Installer;
using Graphene.SignalR;
using UnityEngine;
using Zenject;

namespace Graphene.Installer
{
    [CreateAssetMenu(fileName = "SignalRInstaller", menuName = "Installers/SignalRInstaller")]
    public class SignalRInstaller : ScriptableObjectInstaller<ApiInstaller>
    {
        public HttpCommunicationSettings settings;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<NetworkClientManager>().AsSingle().WithArguments(settings.GetUrl(), settings.socketPath, settings.timeout);
        }
    }
}