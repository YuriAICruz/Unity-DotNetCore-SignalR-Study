using Presentation.Managers;
using Zenject;

namespace Installers
{
    public class IntroInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IntroManager>().AsSingle().NonLazy();
        }
    }
}