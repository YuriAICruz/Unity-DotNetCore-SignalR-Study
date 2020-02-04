using System;
using System.Collections.Generic;
using Graphene.SharedModels.ModelView;
using Presentation.Managers;
using UnityEngine;
using Zenject;

namespace Presentation.CharacterSelection
{
    public class ListCharacters : MonoBehaviour
    {
        [Inject] private CharacterSelectionManager _manager;
        [Inject] private SelectCharacterButton.Factory _buttonFactory;
        
        private List<CharactersModelView> _characters;

        private void Awake()
        {
            _manager.GetCharacters((res) =>
            {
                _characters = res;
                Setup();
            });
        }

        private void Setup()
        {
            for (int i = 0; i < _characters.Count; i++)
            {
                var bt = _buttonFactory.Create(transform);
                bt.Setup(_characters[i]);
            }
        }
    }
}