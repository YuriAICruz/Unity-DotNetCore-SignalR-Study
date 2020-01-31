using Graphene.ApiCommunication;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "ApiInstaller", menuName = "Installers/ApiInstaller")]
public class ApiInstaller : ScriptableObjectInstaller<ApiInstaller>
{
    public bool isSsl;
    public string domain;
    public string port;
    
    public override void InstallBindings()
    {
        Container.Bind<Http>().AsSingle().WithArguments($"{(isSsl?"https":"http")}://{domain}:{port}/");
    }
}