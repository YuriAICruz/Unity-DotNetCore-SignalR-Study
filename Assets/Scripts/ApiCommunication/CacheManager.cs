using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using UnityEngine;

namespace Graphene.ApiCommunication
{
    public class CacheManager
    {
        public class CookieData
        {
            public string Value;
            public string Path;
            public string Name;
            public string Domain;
            public bool Discard;
            public bool Expired;

            public CookieData()
            {
            }

            public CookieData(Cookie cookie)
            {
                Value = cookie.Value;
                Path = cookie.Path;
                Name = cookie.Name;
                Domain = cookie.Domain;
                Expired = cookie.Expired;
                Discard = cookie.Discard;
            }
        }

        private string _path;

        public CacheManager()
        {
#if UNITY_EDITOR
            _path = Application.dataPath + "/../Http";
#else
            _path = Application.persistentDataPath + "/Http";
#endif

            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);
        }

        public void Save()
        {
        }

        public void SaveCookies(CookieCollection collection)
        {
            var cookies = new List<CookieData>();
            foreach (Cookie cookie in collection)
            {
                cookies.Add(new CookieData(cookie));
            }

            File.WriteAllText($"{_path}/cookie.db", JsonConvert.SerializeObject(cookies));
        }

        public CookieContainer LoadCookies(Uri uri)
        {
            var file = $"{_path}/cookie.db";

            var container = new CookieContainer();

            if (!File.Exists(file))
            {
                return container;
            }

            var json = File.ReadAllText(file);
            var collection = JsonConvert.DeserializeObject<List<CookieData>>(json);

            foreach (CookieData cookie in collection)
            {
                if (cookie.Expired) continue;

                container.SetCookies(uri, $"{cookie.Name}={cookie.Value}; path=/; domain={cookie.Domain};");
            }

            return container;
        }

        public void ClearCookies()
        {
            var file = $"{_path}/cookie.db";
            
            if (!File.Exists(file))
            {
                return;
            }

            File.Delete(file);
        }
    }
}