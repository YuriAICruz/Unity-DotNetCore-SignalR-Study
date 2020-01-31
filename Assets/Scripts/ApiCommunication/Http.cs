using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

namespace Graphene.ApiCommunication
{
    public class Http : ITickable
    {
        public delegate void NetworkAction();

        private readonly INotificationService _notificationService;

        private Queue<Action> _mainThreadPool;

        private HttpClient _apiClient;
        private Uri _uri;
        private HttpClientHandler _apiClientHandler;
        private CookieContainer _cookieContainer;

        private int _cookieCount;
        private CacheManager _cacheManager;

        public Http(string url, INotificationService notificationService)
        {
            _mainThreadPool = new Queue<Action>();

            _notificationService = notificationService;
            _uri = new Uri(url);
            _cacheManager = new CacheManager();

            _cookieContainer = _cacheManager.LoadCookies(_uri);

            _cookieCount = _cookieContainer.Count;

            _apiClientHandler = new HttpClientHandler();
            _apiClientHandler.CookieContainer = _cookieContainer;
            _apiClientHandler.UseCookies = true;

            _apiClient = new HttpClient(_apiClientHandler);
            _apiClient.Timeout = new TimeSpan(0, 0, 0, 2);
            _apiClient.BaseAddress = _uri;
            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }


        #region Http Methods

        public async Task<HttpResponse<T>> GetAsync<T>(string path, Action<HttpResponse<T>> mainThreadCallback = null)
        {
            try
            {
                using (HttpResponseMessage response = await _apiClient.GetAsync(path))
                {
                    return await HandleResponse<T>(response, mainThreadCallback);
                }
            }
            catch (Exception e)
            {
                return HandleError<T>(e.ToString(), mainThreadCallback);
            }
        }

        public async Task<HttpResponse<T>> PostAsync<T, TBody>(string path, TBody body, Action<HttpResponse<T>> mainThreadCallback = null, bool clearCookies = false)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

                using (HttpResponseMessage response = await _apiClient.PostAsync(path, content))
                {
                    return await HandleResponse<T>(response, mainThreadCallback, clearCookies);
                }
            }
            catch (Exception e)
            {
                return HandleError<T>(e.ToString(), mainThreadCallback);
            }
        }

        public async Task<HttpResponse<T>> PutAsync<T, TBody>(string path, TBody body, Action<HttpResponse<T>> mainThreadCallback = null)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

                using (HttpResponseMessage response = await _apiClient.PutAsync(path, content))
                {
                    return await HandleResponse<T>(response, mainThreadCallback);
                }
            }
            catch (Exception e)
            {
                return HandleError<T>(e.ToString(), mainThreadCallback);
            }
        }

        public async Task<HttpResponse<T>> DeleteAsync<T, TBody>(string path, TBody body, Action<HttpResponse<T>> mainThreadCallback = null)
        {
            try
            {
                using (HttpResponseMessage response = await _apiClient.DeleteAsync(path))
                {
                    return await HandleResponse<T>(response, mainThreadCallback);
                }
            }
            catch (Exception e)
            {
                return HandleError<T>(e.ToString(), mainThreadCallback);
            }
        }

        #endregion

        private HttpResponse<T> HandleError<T>(string e, Action<HttpResponse<T>> mainThreadCallback)
        {
            Debug.LogError(e);

            var res = new HttpResponse<T>(500, false);
            _mainThreadPool.Enqueue(() => mainThreadCallback?.Invoke(res));
            return res;
        }

        private async Task<HttpResponse<T>> HandleResponse<T>(HttpResponseMessage response,
            Action<HttpResponse<T>> mainThreadCallback, bool clearCookies = false)
        {
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    _notificationService.RequestNotAuthorized();
                else
                    _notificationService.RequestFailed();

                Debug.LogError(response.ReasonPhrase);

                var res = new HttpResponse<T>((int) response.StatusCode, false);
                _mainThreadPool.Enqueue(() => mainThreadCallback?.Invoke(res));
                return res;
            }

            var json = await response.Content.ReadAsStringAsync();

            try
            {
                var objRes = JsonConvert.DeserializeObject<T>(json);

                if (clearCookies)
                {
                    _cacheManager.ClearCookies();
                }
                else if (_cookieCount < _cookieContainer.Count)
                {
                    _cacheManager.SaveCookies(_cookieContainer.GetCookies(_uri));
                }

                _notificationService.RequestSuccess();

                var res = new HttpResponse<T>(objRes, (int) response.StatusCode, true);
                _mainThreadPool.Enqueue(() => mainThreadCallback?.Invoke(res));
                return res;
            }
            catch (Exception e)
            {
                _notificationService.RequestFailed();

                var res = HandleError<T>(json + "\n" + e, mainThreadCallback);
                return res;
            }
        }

        public CookieContainer GetCookieContainer()
        {
            return _cookieContainer;
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