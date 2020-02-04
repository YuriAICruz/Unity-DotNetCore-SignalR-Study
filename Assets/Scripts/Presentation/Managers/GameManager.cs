using System;
using System.Collections.Generic;
using Components;
using Graphene.SharedModels.Network;
using Graphene.SignalR;
using Installers;
using Zenject;
using Object = UnityEngine.Object;

namespace Presentation.Managers
{
    public class GameManager : ITickable
    {
        private readonly NetworkBehaviour.Factory _factory;
        private readonly NetworkClientManager _network;

        private const string CreateHandler = "CreatedBehaviour";
        private const string DestroyHandler = "DestriedBehaviour";
        private const string Handler = "UpdatePosition";

        private readonly List<NetworkBehaviour> _players;

        private readonly Queue<Action> _mainThreadPool;
        private ClientStatus _selfStatus;

        public GameManager(NetworkBehaviour.Factory factory, NetworkClientManager network)
        {
            _factory = factory;
            _network = network;
            _players = new List<NetworkBehaviour>();
            _mainThreadPool = new Queue<Action>();

            //_network.OnClientConnected += AddPlayer;
            _network.OnClientDisconnected += RemovePlayer;
            _network.OnClientUpdate += AddPlayer;
            
            _network.RegisterHandler<string>(CreateHandler, (id, userName) =>
            {
                if (!_players.Exists(x => x.Id == id))
                {
                    _mainThreadPool.Enqueue(() =>
                    {
                        _players.Add(_factory.Create(_network.GetClient(userName), id));
                        CallAllBehaviours();
                    });
                    
                }
            });
            
            _network.RegisterHandler<string>(DestroyHandler, (id, userName) =>
            {
                var i = _players.FindIndex(x => x.Id == id);
                if (i >= 0)
                {
                    if (!_players[i].IsLocal)
                    {
                        _mainThreadPool.Enqueue(() => { DestroyPlayer(i); });
                    }
                }
            });

            SetupScene();
        }


        private void AddPlayer(NetworkClient client)
        {
            var self = _network.Self;

            if (client == self && _selfStatus == ClientStatus.Waiting && self.Status == ClientStatus.Ready)
            {
                if (!_players.Exists(x => x.Client.userName == self.userName))
                {
                    _players.Add(_factory.Create(client));
                    
                    CallAllBehaviours();
                }
            }

            _selfStatus = self.Status;
        }

        private void CallAllBehaviours()
        {
            foreach (var networkBehaviour in _players)
            {
                networkBehaviour.SyncNetwork();
            }
        }

        private void RemovePlayer(string userName)
        {
            var i = _players.FindIndex(x => x.Client.userName == userName);

            if (i < 0) return; //|| !_players[i].IsLocal

            DestroyPlayer(i);
        }

        private void DestroyPlayer(int i)
        {
            Object.Destroy(_players[i].gameObject);
            _players.RemoveAt(i);
        }


        private void SetupScene()
        {
            var self = _network.Self;

            foreach (var client in _network.Clients)
            {
                if (client != self) continue;

                _selfStatus = self.Status;
                _players.Add(_factory.Create(client));
                CallAllBehaviours();
                break;
            }
        }

        public void Tick()
        {
            if (_mainThreadPool.Count > 0)
            {
                _mainThreadPool.Dequeue()();
            }
        }
    }
}