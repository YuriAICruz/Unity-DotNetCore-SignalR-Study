using Presentation.Managers;
using UnityEngine;
using Zenject;

public class IntroInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IntroManager>().AsSingle().NonLazy();
    }
}