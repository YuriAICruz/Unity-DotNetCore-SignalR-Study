using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Graphene.ApiCommunication;
using Graphene.SharedModels.ModelView;
using Graphene.SignalR;
using UnityEngine;
using Zenject;

namespace Controllers
{
    public class AuthenticationController
    {
        private readonly Http _http;
        private readonly INotificationService _notification;
        private readonly NetworkClientManager _network;

        public event Action NetworkOn;
        public event Action NetworkOff;

        public event Action UserLoggedIn;
        public event Action UserSignedUp;
        public event Action UserSignedOut;
        public event Action ShowSignUp;
        public event Action ShowSignIn;

        public UserModelView CurrentUserModelView { get; private set; }
        public bool IsLoggedIn { get; private set; }

        private Queue<Action> _workingThreads;

        private bool _isBusy;

        public AuthenticationController(Http http, INotificationService notification, NetworkClientManager network)
        {
            _http = http;
            _notification = notification;
            _network = network;

            _network.OnConnected += NetworkConnected;
            _network.OnDisconnected += NetworkDisconnected;

            _workingThreads = new Queue<Action>();

            _notification.OnRequestNotAuthorized += NotAuthorized;

            IsLoggedIn = false;
            _isBusy = true;
            _http.GetAsync<UserModelView>("auth", (res) =>
            {
                WorkerFree();
                if (res.Success)
                {
                    OnLoggedIn(res.Response);
                    Debug.Log(CurrentUserModelView);
                    return;
                }

                Debug.LogError(res.StatusCode);
            }).ContinueWith(CallError);
        }

        private void NetworkDisconnected()
        {
            NetworkOff?.Invoke();
        }

        private void NetworkConnected()
        {
            NetworkOn?.Invoke();
        }

        void OnLoggedIn(UserModelView userModelView)
        {
            IsLoggedIn = true;
            UserLoggedIn?.Invoke();
            CurrentUserModelView = userModelView;

            _network?.Connect(CurrentUserModelView.UserName);
        }

        void OnLoggedIn(RegisterModelView userModelView)
        {
            IsLoggedIn = true;
            UserSignedUp?.Invoke();
            CurrentUserModelView = new UserModelView(userModelView.UserName, userModelView.Email);

            _network?.Connect(CurrentUserModelView.UserName);
        }

        private void CallError<T>(Task<HttpResponse<T>> task)
        {
            if (task.IsCompleted) return;

            Debug.LogError(task.Exception);
        }

        private void NotAuthorized()
        {
            UserSignedOut?.Invoke();
        }

        private void WorkerFree()
        {
            _isBusy = false;

            if (_workingThreads.Count > 0)
            {
                _workingThreads.Dequeue()();
            }
        }

        #region Navigation

        public void SignIn()
        {
            ShowSignIn?.Invoke();
        }

        public void SignUp()
        {
            ShowSignUp?.Invoke();
        }

        #endregion

        #region Actions

        public void Login(LoginModelView loginModelView, Action<bool> onResponse)
        {
            if (IsLoggedIn)
            {
                onResponse(true);
                return;
            }

            if (_isBusy)
            {
                _workingThreads.Enqueue(() => Login(loginModelView, onResponse));
                return;
            }

            _isBusy = true;
            _http.PostAsync<UserModelView, LoginModelView>("auth/signIn", loginModelView,
                    (res) => OnLoginResult(res, onResponse))
                .ContinueWith(CallError);
        }

        public void SignUp(RegisterModelView registerModelView, Action<bool> onResponse)
        {
            if (IsLoggedIn)
            {
                onResponse(true);
                return;
            }

            if (_isBusy)
            {
                _workingThreads.Enqueue(() => SignUp(registerModelView, onResponse));
                return;
            }

            _isBusy = true;
            _http.PostAsync<RegisterModelView, RegisterModelView>("auth/signUp", registerModelView,
                    (res) => OnSignUpResult(res, onResponse))
                .ContinueWith(CallError);
        }


        public void SignOut(Action<bool> onResponse)
        {
            if (!IsLoggedIn)
            {
                onResponse(true);
                return;
            }

            if (_isBusy)
            {
                _workingThreads.Enqueue(() => SignOut(onResponse));
                return;
            }

            _isBusy = true;
            _http.PostAsync<string, string>("auth/signOut", "", (res) => OnSignOutResult(res, onResponse))
                .ContinueWith(CallError);
        }

        #endregion

        #region Callbacks

        private void OnLoginResult(HttpResponse<UserModelView> result, Action<bool> onResponse)
        {
            _isBusy = false;

            if (result.Success)
            {
                OnLoggedIn(result.Response);
                onResponse?.Invoke(true);
                return;
            }

            onResponse?.Invoke(false);
            Debug.LogError(result.StatusCode);
        }

        private void OnSignUpResult(HttpResponse<RegisterModelView> result, Action<bool> onResponse)
        {
            _isBusy = false;

            if (result.Success)
            {
                OnLoggedIn(result.Response);
                onResponse?.Invoke(true);
                return;
            }

            onResponse?.Invoke(false);
            Debug.LogError(result.StatusCode);
        }

        private void OnSignOutResult(HttpResponse<string> result, Action<bool> onResponse)
        {
            _isBusy = false;

            if (result.Success)
            {
                UserSignedOut?.Invoke();
                IsLoggedIn = false;
                onResponse?.Invoke(true);
                return;
            }

            onResponse?.Invoke(false);
            Debug.LogError(result.StatusCode);
        }

        #endregion
    }
}