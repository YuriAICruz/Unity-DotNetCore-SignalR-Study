using System;
using System.Collections.Generic;
using Graphene.ApiCommunication;
using Graphene.SharedModels.ModelView;
using Graphene.SignalR;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Presentation.Managers
{
    public class CharacterSelectionManager
    {
        private readonly Http _http;
        private readonly NetworkClientManager _network;
        public event Action<int> OnCharacterSelected; 
        private int _selectedCharacter = 0;

        public int SelectedCharacter => _selectedCharacter;
        
        public CharacterSelectionManager(Http http, NetworkClientManager network)
        {
            _http = http;
            _network = network;
        }

        public void SelectCharacter(int id)
        {
            _selectedCharacter = id;
            OnCharacterSelected?.Invoke(_selectedCharacter);
            
            _network.Self.SelectCharacter(id);
            _network.Sync();
        }

        public void Continue()
        {
            SceneManager.LoadScene(2);
        }

        public void GetCharacters(Action<List<CharactersModelView>> callback)
        {
            _http.GetAsync<List<CharactersModelView>>("characters", (res) =>
            {
                if (!res.Success)
                {
                    Debug.LogError(res.StatusCode);
                    callback?.Invoke(new List<CharactersModelView>());
                    
                    return;
                }
                callback?.Invoke(res.Response);
            });
        }
    }
}