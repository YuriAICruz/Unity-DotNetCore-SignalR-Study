using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using Graphene.SharedModels.ModelView;
using Graphene.SharedModels.Network;
using Graphene.SignalR;
using UnityEngine.SceneManagement;

namespace Presentation.Managers
{
    public class LobbyManager:IDisposable
    {
        public event Action<NetworkClient> OnPlayerUpdate;
        public event Action<NetworkClient> OnPlayerConnected;
        public event Action<string> OnPlayerDisconnected;

        private readonly NetworkClientManager _network;
        private readonly CharactersController _charactersController;

        public LobbyManager(NetworkClientManager network, CharactersController charactersController)
        {
            _network = network;
            _charactersController = charactersController;
            _network.OnClientUpdate += PlayerUpdate;
            _network.OnClientConnected += PlayerConnected;
            _network.OnClientDisconnected += PlayerDisconnected;
        }

        public void Dispose()
        {
            _network.OnClientUpdate -= PlayerUpdate;
            _network.OnClientConnected -= PlayerConnected;
            _network.OnClientDisconnected -= PlayerDisconnected;
        }

        private void PlayerUpdate(NetworkClient player)
        {
            OnPlayerUpdate?.Invoke(player);

            if (_network.Clients.Count(x => x.Status == ClientStatus.Waiting) <= 0)
            {
                Continue();
            }
        }

        private void Continue()
        {
            if(SceneManager.GetActiveScene().name == "Lobby")
                SceneManager.LoadScene(3);
        }

        private void PlayerConnected(NetworkClient player)
        {
            OnPlayerConnected?.Invoke(player);
        }

        private void PlayerDisconnected(string player)
        {
            OnPlayerDisconnected?.Invoke(player);
        }

        public void IsReady()
        {
            _network.Self.SetStatus(ClientStatus.Ready);
            _network.Sync();
        }

        public void CancelReady()
        {
            _network.Self.SetStatus(ClientStatus.Waiting);
            _network.Sync();
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