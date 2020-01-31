using Graphene.ApiCommunication;
using UnityEngine;
using Zenject;

namespace Graphene.ApiCommunication.Installer
{
    [CreateAssetMenu(fileName = "ApiInstaller", menuName = "Installers/ApiInstaller")]
    public class ApiInstaller : ScriptableObjectInstaller<ApiInstaller>
    {
        public HttpCommunicationSettings settings;
        
        public override void InstallBindings()
        {
            Container.Bind<INotificationService>().To<DirectNotificationService>().AsSingle();

            Container.BindInterfacesAndSelfTo<Http>().AsSingle().WithArguments(settings.GetUrl());
        }
    }
}