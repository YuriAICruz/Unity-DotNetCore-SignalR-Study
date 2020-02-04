using System;
using System.Collections.Generic;
using Graphene.ApiCommunication;
using Graphene.SharedModels.ModelView;
using Graphene.SignalR;
using UnityEngine;
using Random = System.Random;

namespace Controllers
{
    public class CharactersController
    {
        private readonly Http _http;
        private readonly NetworkClientManager _network;
        private List<CharactersModelView> _characters;

        public CharactersController(Http http, NetworkClientManager network)
        {
            _http = http;
            _network = network;
        }

        public int SelectedCharacter { get; private set; }

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

                _characters = res.Response;
                callback?.Invoke(_characters);
            });
        }

        public CharactersModelView GetCharacter(int id)
        {
            return _characters.Find(x => x.Id == id);
        }

        public void SelectCharacter(int id)
        {
            SelectedCharacter = id;

            _network.Self.SelectCharacter(id);
            _network.Sync();
        }
    }
}