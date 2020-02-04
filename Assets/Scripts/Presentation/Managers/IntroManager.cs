using System;
using Controllers;
using UnityEngine.SceneManagement;

namespace Presentation.Managers
{
    public class IntroManager : IDisposable
    {
        private readonly AuthenticationController _auth;

        private bool _userOn, _networkOn;

        public IntroManager(AuthenticationController auth)
        {
            _auth = auth;

            _auth.UserLoggedIn += UserOn;
            _auth.UserSignedIn += UserOn;

            _auth.NetworkOn += NetworkOn;

            _auth.NetworkOff += NetworkOff;
            _auth.UserSignedOut += UserOff;
        }

        public void Dispose()
        {
            _auth.UserLoggedIn -= UserOn;
            _auth.UserSignedIn -= UserOn;

            _auth.NetworkOn -= NetworkOn;

            _auth.NetworkOff -= ReturnToLogin;
            _auth.UserSignedOut -= ReturnToLogin;
        }

        private void UserOff()
        {
            _userOn = false;
            ReturnToLogin();
        }

        private void NetworkOff()
        {
            _networkOn = false;
            ReturnToLogin();
        }

        private void ReturnToLogin()
        {
            if (!_userOn || !_networkOn)
                SceneManager.LoadScene(0);
        }

        private void UserOn()
        {
            _userOn = true;
            NextWindow();
        }

        private void NetworkOn()
        {
            _networkOn = true;
            NextWindow();
        }

        private void NextWindow()
        {
            if (_userOn && _networkOn)
                SceneManager.LoadScene(1);
        }
    }
}