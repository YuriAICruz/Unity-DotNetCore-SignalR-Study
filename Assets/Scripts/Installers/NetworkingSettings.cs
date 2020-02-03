using Components;
using UnityEngine;

namespace Installers
{
    [CreateAssetMenu(fileName = "NetworkingSettings", menuName = "Graphene/Networking/NetworkingSettings")]
    public class NetworkingSettings : ScriptableObject
    {
        public NetworkBehaviour playerPrefab;
    }
}