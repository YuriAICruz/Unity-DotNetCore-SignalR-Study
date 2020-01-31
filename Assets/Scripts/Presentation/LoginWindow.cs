using System;
using System.Threading.Tasks;
using Controllers;
using Graphene.ApiCommunication;
using Models;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Presentation
{
    public class LoginWindow : BaseWindow
    {
        public InputField username;
        public InputField password;

        public Button signUp;

        [Inject] private AuthenticationController _authentication;

        protected override void Awake()
        {
            base.Awake();

            _authentication.UserLoggedIn += Hide;
            _authentication.UserSignedIn += Hide;
            
            _authentication.ShowSignIn += Show;
            _authentication.ShowSignUp += Hide;
            
            signUp.onClick.AddListener(ShowSignUp);
            
            if(_authentication.IsLoggedIn)
                Hide();
        }

        private void OnDestroy()
        {
            _authentication.UserLoggedIn -= Hide;
            _authentication.UserSignedIn -= Hide;
            
            _authentication.ShowSignIn -= Show;
            _authentication.ShowSignUp -= Hide;
        }

        private void ShowSignUp()
        {
            _authentication.SignUp();
        }

        protected override void OnSubmit()
        {
            submit.interactable = false;
            _authentication.Login(new LoginModelView(username.text, password.text), EnableSubmitButton);
        }
    }
}