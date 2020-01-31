using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Graphene.ApiCommunication;
using Models;
using UnityEngine;
using Zenject;

namespace Controllers
{
    public class AuthenticationController
    {
        private readonly Http _http;
        private readonly INotificationService _notification;

        public event Action UserLoggedIn;
        public event Action UserSignedIn;
        public event Action UserSignedOut;
        public event Action ShowSignUp;
        public event Action ShowSignIn;

        public User CurrentUser { get; private set; }
        public bool IsLoggedIn { get; private set; }

        private Queue<Action> _workingThreads;

        private bool _isBusy;


        public AuthenticationController(Http http, INotificationService notification)
        {
            _http = http;
            _notification = notification;
            _workingThreads = new Queue<Action>();

            _notification.OnRequestNotAuthorized += NotAuthorized;

            IsLoggedIn = false;
            _isBusy = true;
            _http.GetAsync<User>("auth", (res) =>
            {
                WorkerFree();
                if (res.Success)
                {
                    IsLoggedIn = true;
                    UserLoggedIn?.Invoke();
                    CurrentUser = res.Response;
                    Debug.Log(CurrentUser);
                    return;
                }

                Debug.LogError(res.StatusCode);
            }).ContinueWith(CallError);
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
            _http.PostAsync<LoginModelView, LoginModelView>("auth/signIn", loginModelView,
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

        private void OnLoginResult(HttpResponse<LoginModelView> result, Action<bool> onResponse)
        {
            _isBusy = false;

            if (result.Success)
            {
                IsLoggedIn = true;
                UserLoggedIn?.Invoke();
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
                IsLoggedIn = true;
                UserSignedIn?.Invoke();
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