using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using Graphene.SharedModels.ModelView;
using Graphene.SharedModels.Network;
using Graphene.SignalR;

namespace Presentation.Managers
{
    public class LobbyManager
    {
        public event Action<NetworkClient> OnPlayerUpdate;
        
        private readonly NetworkClientManager _network;
        private readonly CharactersController _charactersController;

        public LobbyManager(NetworkClientManager network, CharactersController charactersController)
        {
            _network = network;
            _charactersController = charactersController;
            _network.OnClientUpdate += OnPlayerUpdate;
        }
        
        public void IsReady()
        {
            _network.Self.SetStatus(ClientStatus.Ready);            
        }
        
        public void CancelReady()
        {
            _network.Self.SetStatus(ClientStatus.Waiting);            
        }

        public IReadOnlyList<NetworkClient> GetAvailablePlayers()
        {
            return _network.Clients;
        }

        public CharactersModelView GetCharacter(int id)
        {
            return _charactersController.GetCharacter(id);
        }
    }
}