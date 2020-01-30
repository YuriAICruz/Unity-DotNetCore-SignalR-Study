using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace DefaultNamespace
{
    public class Http
    {
        //Obs async methods do not return on the main thread, be aware with thread safety.
        
        
        private HttpClient _apiClient;
        private Uri _uri;
        private HttpClientHandler _apiClientHandler;
        private CookieContainer _cookieContainer;

        private int _cookieCount;
        private CacheManager _cacheManager;

        public Http(string url)
        {
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

        public async Task<HttpResponse<T>> GetAsync<T>(string path)
        {
            try
            {
                using (HttpResponseMessage response = await _apiClient.GetAsync(path))
                {
                    return await HandleResponse<T>(response);
                }
            }
            catch (Exception e)
            {
                return HandleError<T>(e);
            }
        }
        
        public async Task<HttpResponse<T>> PostAsync<T, TBody>(string path, TBody body)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

                using (HttpResponseMessage response = await _apiClient.PostAsync(path,content))
                {
                    return await HandleResponse<T>(response);
                }
            }
            catch (Exception e)
            {
                return HandleError<T>(e);
            }
        }
        
        public async Task<HttpResponse<T>> PutAsync<T, TBody>(string path, TBody body)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                
                using (HttpResponseMessage response = await _apiClient.PutAsync(path,content))
                {
                    return await HandleResponse<T>(response);
                }
            }
            catch (Exception e)
            {
                return HandleError<T>(e);
            }
        }
        
        public async Task<HttpResponse<T>> DeleteAsync<T, TBody>(string path, TBody body)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                
                using (HttpResponseMessage response = await _apiClient.DeleteAsync(path))
                {
                    return await HandleResponse<T>(response);
                }
            }
            catch (Exception e)
            {
                return HandleError<T>(e);
            }
        }

        private HttpResponse<T> HandleError<T>(Exception e)
        {
            Debug.LogError(e);
            return new HttpResponse<T>(500, false);
        }

        private async Task<HttpResponse<T>> HandleResponse<T>(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                Debug.LogError(response.ReasonPhrase);
                return new HttpResponse<T>((int) response.StatusCode, false);
            }
            
            try
            {
                var json = await response.Content.ReadAsStringAsync();
                Debug.Log(json);
                var res = JsonConvert.DeserializeObject<T>(json);
                
                Debug.Log(_cookieContainer.Count);
                if(_cookieCount < _cookieContainer.Count)
                {
                    SaveCookies();
                }

                return new HttpResponse<T>(res, (int) response.StatusCode, true);
            }
            catch (Exception e)
            {
                return HandleError<T>(e);
            }
        }

        private void SaveCookies()
        {
            _cacheManager.SaveCookies(_cookieContainer.GetCookies(_uri));
        }
    }
}