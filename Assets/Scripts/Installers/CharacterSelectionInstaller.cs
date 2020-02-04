using Presentation.CharacterSelection;
using Presentation.Managers;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class CharacterSelectionInstaller : MonoInstaller
    {
        public SelectCharacterButton prefab;
        
        public override void InstallBindings()
        {
            Container.Bind<CharacterSelectionManager>().AsSingle();

            Container.BindFactory<SelectCharacterButton, SelectCharacterButton.Factory>().FromComponentInNewPrefab(prefab);
        }
    }
}