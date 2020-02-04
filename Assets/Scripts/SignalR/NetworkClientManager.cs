using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Graphene.ApiCommunication;
using Graphene.SharedModels.Network;
using JetBrains.Annotations;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Graphene.SignalR
{
    public class NetworkClientManager : IDisposable, ITickable
    {
        public event Action OnConnected, OnDisconnected;

        public event Action<NetworkClient> OnClientConnected;
        public event Action<string> OnClientDisconnected;
        public event Action<NetworkClient> OnClientUpdate;

        private readonly int _timeout;
        private readonly Http _http;

        private readonly HubConnection _connection;
        private bool _isDisposed;
        private bool _connected;
        public bool Connected => _connected;

        private string _userName;

        private NetworkClients _connections;

        public NetworkClient Self => _connections.Self;
        public IReadOnlyList<NetworkClient> Clients => _connections.Clients;

        private readonly Queue<Action> _mainThreadPool;

        public NetworkClientManager(string baseUrl, string socketPath, int timeout, Http http)
        {
            _http = http;
            _timeout = timeout;

            _mainThreadPool = new Queue<Action>();

            _connection = new HubConnectionBuilder()
                .WithUrl($"{baseUrl}{socketPath}",
                    options => { options.Cookies = _http.GetCookieContainer(); })
                .Build();

            _connection.On<string, string>("OnConnected", ClientConnected);
            _connection.On<string, string>("OnDisconnected", ClientDisconnected);

            _connection.On<NetworkClient>("OnClientUpdate", ClientUpdated);

            _connection.Closed += ReConnect;

            _isDisposed = false;
        }

        public void Dispose()
        {
            _connection.DisposeAsync();
            _isDisposed = true;
        }


        public async Task Sync()
        {
            if (!_connected || !Self.IsDirty()) return;

            try
            {
                await _connection.InvokeAsync("UpdatePlayer", Self);
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

        private void ClientUpdated(NetworkClient client)
        {
            Debug.Log(client);
            _connections.Update(client);

            _mainThreadPool.Enqueue(() => OnClientUpdate?.Invoke(client));
        }


        public async Task Connect(string userName)
        {
            _connections = new NetworkClients(userName);
            _userName = userName;

            if (_isDisposed)
                return;
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
                return;
#endif

            try
            {
                await _connection.StartAsync();

                OnConnectedToServer();
            }
            catch (System.Exception ex)
            {
                ReConnect(ex);
            }
        }

        private async Task ReConnect(Exception error)
        {
            if (!_isDisposed) return;
            
            Debug.LogError(error);

            if (_connected)
                OnDisconnectedToServer();

            await Task.Delay(_timeout);
            await Connect(_userName);
        }


        private void OnDisconnectedToServer()
        {
            _connected = false;
            _mainThreadPool.Enqueue(() => OnDisconnected?.Invoke());
            Debug.Log("Disconnected");
        }

        private void OnConnectedToServer()
        {
            Debug.Log("Connection init");
        }


        private void ClientConnected(string userName, string id)
        {
            if (_userName == userName)
            {
                _connections.AddSelf(userName, id);
                _connected = true;
                _mainThreadPool.Enqueue(() => OnConnected?.Invoke());
                Debug.Log("Connection started");
                return;
            }

            var i = _connections.Add(userName, id);

            Debug.Log($"OnConnected: {userName} {userName == _userName}");

            _mainThreadPool.Enqueue(() => OnClientConnected?.Invoke(_connections[i]));
        }

        private void ClientDisconnected(string userName, string id)
        {
            if (_userName == userName)
            {
                return;
            }

            var i = _connections.FindIndex(userName);

            if (i >= 0)
            {
                var username = _connections[i].userName;
                _mainThreadPool.Enqueue(() => OnClientDisconnected?.Invoke(username));
                _connections.RemoveAt(i);
            }
            else
            {
                Debug.LogError("Client not found on disconnection");
            }
        }

        #region Handlers

        public void RegisterHandler<T>(string handlerName, Guid guid, [NotNull] Action<T> response)
        {
            _connection.On<Guid, string>(handlerName, (id, json) =>
            {
                if (guid != id) return;

                try
                {
                    var res = JsonConvert.DeserializeObject<T>(json);

                    response(res);
                    return;
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to deserialize message\n" + e);
                }
            });
        }

        public void RegisterHandler<T>(string handlerName, [NotNull] Action<Guid, T> response)
        {
            _connection.On<Guid, string>(handlerName, (id, json) =>
            {
                try
                {
                    var res = JsonConvert.DeserializeObject<T>(json);

                    response(id, res);
                    return;
                }
                catch (Exception e)
                {
                    Debug.LogError("Failed to deserialize message\n" + e);
                }
            });
        }

        public void RegisterHandler(string handlerName, Guid guid, [NotNull] Action<float> response)
        {
            _connection.On<Guid, float>(handlerName, (id, value) =>
            {
                if (guid != id) return;

                response(value);
            });
        }

        public void RegisterHandler(string handlerName, [NotNull] Action<int> response)
        {
            throw new NotImplementedException();
            _connection.On<string, string>(handlerName, (name, value) =>
            {
                if (int.TryParse(value, out var res))
                {
                    response(res);
                    return;
                }

                Debug.LogError("Failed to deserialize value");
            });
        }

        public void UnregisterHandler(string handlerName)
        {
            Debug.LogError("NotImplementedException");
        }

        #endregion

        #region Sender

        public async void SendToAll<T>(string handler, Guid id, T data)
        {
            if (!_connected) return;

            try
            {
                await _connection.InvokeAsync("SendToAllFromId", handler, id, JsonConvert.SerializeObject(data));
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

        public async void SendToAll<T>(string handler, T data)
        {
            if (!_connected) return;

            try
            {
                await _connection.InvokeAsync("SendToAll", handler, Self.userName, JsonConvert.SerializeObject(data));
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

        #endregion

        public void Tick()
        {
            if (_mainThreadPool.Count > 0)
            {
                _mainThreadPool.Dequeue()();
            }
        }

        public NetworkClient GetClient(string userName)
        {
            return _connections.Find(userName);
        }
    }
}