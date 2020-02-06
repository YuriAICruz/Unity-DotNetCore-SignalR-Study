using System;
using System.Threading.Tasks;
using Controllers;
using Graphene.ApiCommunication;
using Graphene.SharedModels.ModelView;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Presentation
{
    public class SignUpWindow : BaseWindow
    {
        public InputField username;
        public InputField email;
        public InputField password;
        
        public Button signIn;

        [Inject] private Http _http;
        [Inject] private AuthenticationController _authentication;

        protected override void Awake()
        {
            base.Awake();

            Hide();
            _authentication.UserLoggedIn += Hide;
            _authentication.UserSignedUp += Hide;
            
            _authentication.ShowSignIn += Hide;
            _authentication.ShowSignUp += Show;
            
            signIn.onClick.AddListener(ShowSignIn);
        }

        public void ShowSignIn()
        {
            _authentication.SignIn();
        }

        private void OnDestroy()
        {
            _authentication.UserLoggedIn -= Hide;
            _authentication.UserSignedUp -= Hide;
            
            _authentication.ShowSignIn -= Show;
            _authentication.ShowSignUp -= Hide;
        }

        protected override void OnSubmit()
        {
            submit.interactable = false;
            _authentication.SignUp(new RegisterModelView(username.text, email.text, password.text), EnableSubmitButton);
        }
    }
}