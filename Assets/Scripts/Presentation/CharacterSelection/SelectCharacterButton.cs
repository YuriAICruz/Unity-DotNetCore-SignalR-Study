using Graphene.SharedModels.ModelView;
using Presentation.Managers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Presentation.CharacterSelection
{
    public class SelectCharacterButton : ButtonView
    {
        [Inject] private CharacterSelectionManager _manager;

        public Text characterName;
        public Image characterColor;

        public Color selected, normal, disbled;
        private CharactersModelView _character;

        protected override void Awake()
        {
            base.Awake();

            _button.interactable = _character != null;

            _button.image.color = GetColor;
            _manager.OnCharacterSelected += ChangeColor;
        }

        private void ChangeColor(int index)
        {
            _button.image.color = GetColor;
        }

        protected override void OnClick()
        {
            _manager.SelectCharacter(_character.Id);
        }

        private Color GetColor => _character == null ? disbled : _manager.SelectedCharacter ==  _character.Id ? selected : normal;

        public void Setup(CharactersModelView character)
        {
            _character = character;

            _button.interactable = true;
            
            characterName.text = character.Name;
            characterColor.color = character.GetColor();
            
            _button.image.color = GetColor;
        }

        public class Factory : PlaceholderFactory<SelectCharacterButton>
        {
            public SelectCharacterButton Create(Transform transform)
            {
                var instance = base.Create();

                instance.transform.SetParent(transform);
                
                instance.transform.localScale = Vector3.one;
                
                return instance;
            }
        }
    }
}