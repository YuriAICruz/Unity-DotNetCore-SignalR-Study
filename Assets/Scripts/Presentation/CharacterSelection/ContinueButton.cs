using Presentation.Managers;
using UnityEngine;
using Zenject;

namespace Presentation.CharacterSelection
{
    public class ContinueButton : ButtonView
    {
        [Inject] private CharacterSelectionManager _manager;
        
        protected override void OnClick()
        {
            _manager.Continue();
        }
    }
}