using Unity.Netcode;
using UnityEngine;

namespace Visualization.Networking
{
    class Player : NetworkBehaviour
    {
        public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();

        public string Name;

        [SerializeField]
        public Color Color;
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                Color = Color.blue;
            }
            if (IsClient)
            {
                Color = Color.green;
            }
        }
    }
}