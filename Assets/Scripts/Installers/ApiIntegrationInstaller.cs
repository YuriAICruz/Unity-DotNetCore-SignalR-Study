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
        public override void InstallBindings()
        {
            Container.Bind<AuthenticationController>().AsSingle();
        }
    }
}