using System;
using System.Collections.Generic;
using Controllers;
using Graphene.SharedModels.ModelView;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Presentation.Managers
{
    public class CharacterSelectionManager
    {
        private readonly CharactersController _controller;
        public event Action<int> OnCharacterSelected;

        public int SelectedCharacter => _controller.SelectedCharacter;

        public CharacterSelectionManager(CharactersController controller)
        {
            _controller = controller;
        }

        public void SelectCharacter(int id)
        {
            _controller.SelectCharacter(id);
            OnCharacterSelected?.Invoke(_controller.SelectedCharacter);
        }

        public void Continue()
        {
            SceneManager.LoadScene(2);
        }

        public void GetCharacters(Action<List<CharactersModelView>> callback)
        {
            _controller.GetCharacters((characters) =>
            {
                SelectCharacter(Random.Range(0, characters.Count));
                callback?.Invoke(characters);
            });
        }

        public CharactersModelView GetCharacter(int id)
        {
            return _controller.GetCharacter(id);
        }
    }
}