using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Graphene.ApiCommunication
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
                return HandleError<T>(e.ToString());
            }
        }
        public async Task<HttpResponse<T>> PostAsync<T, TBody>(string path, TBody body,bool clearCookies = false)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

                using (HttpResponseMessage response = await _apiClient.PostAsync(path,content))
                {
                    return await HandleResponse<T>(response, clearCookies);
                }
            }
            catch (Exception e)
            {
                return HandleError<T>(e.ToString());
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
                return HandleError<T>(e.ToString());
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
                return HandleError<T>(e.ToString());
            }
        }

        private HttpResponse<T> HandleError<T>(string e)
        {
            Debug.LogError(e);
            return new HttpResponse<T>(500, false);
        }

        private async Task<HttpResponse<T>> HandleResponse<T>(HttpResponseMessage response,bool clearCookies = false)
        {
            if (!response.IsSuccessStatusCode)
            {
                Debug.LogError(response.ReasonPhrase);
                return new HttpResponse<T>((int) response.StatusCode, false);
            }
            
            var json = await response.Content.ReadAsStringAsync();
            
            try
            {
                var res = JsonConvert.DeserializeObject<T>(json);

                if (clearCookies)
                {
                    _cacheManager.ClearCookies();
                }else if(_cookieCount < _cookieContainer.Count)
                {
                    _cacheManager.SaveCookies(_cookieContainer.GetCookies(_uri));
                }

                return new HttpResponse<T>(res, (int) response.StatusCode, true);
            }
            catch (Exception e)
            {
                return HandleError<T>(json + "\n" + e);
            }
        }

        public CookieContainer GetCookieContainer()
        {
            return _cookieContainer;
        }
    }
}