using Zenject;

namespace Presentation.Managers
{
    public class CharacterSelectionInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<CharacterSelectionManager>().AsSingle().NonLazy();
        }
    }
}