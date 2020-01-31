using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Graphene.ApiCommunication;
using Graphene.ApiCommunication.Models;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SignalRStudy
{
    public class HttpTester : MonoBehaviour
    {
        public Text response;
        public InputField idField;
        public InputField mailField;
        public InputField nameField;
        public Button postButton;
        
        [Inject]private Http _http;

        private Queue<Action> _parallelStack = new Queue<Action>();

        private void Awake()
        {
            response.text = "loading . . .";

            _http.GetAsync<Player>("player/2").ContinueWith(EnqueueShowPlayer);
            
            postButton.onClick.AddListener(Post);
        }

        private void Start()
        {
            //http.PostAsync<string, string>("auth/signout", "", true).ContinueWith(EnqueueShowGeneric);
        }

        private void EnqueueShowUser(Task<HttpResponse<RegisterModelView>> user)
        {
            _parallelStack.Enqueue(() => ShowUser(user));
        }
        
        private void ShowUser(Task<HttpResponse<RegisterModelView>> user)
        {
            if (user.Result.Success)
            {
                Debug.Log(user.Result.Response);

                response.text = user.Result.Response.ToString();
            }
            else
            {
                Debug.LogError(user.Exception);
                
                response.text = user.Exception?.ToString();
            }
        }

        private void Update()
        {
            if (_parallelStack.Count > 0)
            {
                _parallelStack.Dequeue()();
            }
        }

        private void Post()
        {
            //http.PostAsync<RegisterModelView, RegisterModelView>("auth/signUp", new RegisterModelView((idField.text),  mailField.text, nameField.text)).ContinueWith(EnqueueShowUser);
            _http.PostAsync<LoginModelView, LoginModelView>("auth/signin", new LoginModelView((idField.text),  nameField.text)).ContinueWith(EnqueueShowGeneric);
            
            //http.PostAsync<Player, Player>("player", new Player(int.Parse(idField.text), nameField.text)).ContinueWith(EnqueueShowPlayer);
        }

        private void EnqueueShowGeneric<T>(Task<HttpResponse<T>> obj)
        {
            _parallelStack.Enqueue(() => ShowGeneric(obj));
        }

        private void ShowGeneric<T>(Task<HttpResponse<T>> obj)
        {
            if (obj.Result.Success)
            {
                Debug.Log(obj.Result.Response);

                response.text = obj.Result.Response.ToString();
            }
            else
            {
                Debug.LogError(obj.Exception);
                
                response.text = obj.Exception?.ToString();
            }
        }

        private void EnqueueShowPlayer(Task<HttpResponse<Player>> player)
        {
            _parallelStack.Enqueue(() => ShowPlayer(player));
        }

        private void ShowPlayer(Task<HttpResponse<Player>> player)
        {
            if (player.Result.Success)
            {
                Debug.Log(player.Result.Response);

                response.text = player.Result.Response.ToString();
            }
            else
            {
                Debug.LogError(player.Exception);
                
                response.text = player.Exception?.ToString();
            }
        }
    }
}