using UnityEngine;

namespace Graphene.ApiCommunication
{
    [CreateAssetMenu(fileName = "HttpCommunicationSettings", menuName = "Graphene/Api/HttpCommunicationSettings")]
    public class HttpCommunicationSettings : ScriptableObject
    {
        public bool isSsl = false;
        public string domain = "127.0.0.1";
        public string port = "8080";
        public string socketPath = "hub";
        public int timeout = 1000;

        public string GetUrl()
        {
            return $"{(isSsl ? "https" : "http")}://{domain}:{port}/";
        }
    }
}