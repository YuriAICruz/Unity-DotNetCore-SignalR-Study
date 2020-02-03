using Presentation.Managers;
using UnityEngine;
using Zenject;

namespace Presentation.CharacterSelection
{
    public class SelectCharacterButton : ButtonView
    {
        [Inject] private CharacterSelectionManager _manager;

        public Color selected, normal;

        protected override void Awake()
        {
            base.Awake();

            _button.image.color = GetColor;
            _manager.OnCharacterSelected += ChangeColor;
        }

        private void ChangeColor(int index)
        {
            _button.image.color = GetColor;
        }

        protected override void OnClick()
        {
            _manager.SelectCharacter(transform.GetSiblingIndex());
        }

        private Color GetColor => _manager.SelectedCharacter == transform.GetSiblingIndex() ? selected : normal;
    }
}