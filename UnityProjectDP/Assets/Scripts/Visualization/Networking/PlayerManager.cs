using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Networking
{
    public class PlayerManager : NetworkSingleton<PlayerManager>
    {
        static string playerName = "Enter name";

        private void Start()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += (id) => { };
        }
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                playerName = GUILayout.TextField(playerName, 25);
                StartButtons();
            }
            else
            {
                StatusLabels();
            }

            GUILayout.EndArea();
        }

        static void StartButtons()
        {
            if (GUILayout.Button("Host"))
            {
                NetworkManager.Singleton.StartHost();
                SceneManager.LoadScene("AnimArch");
            }
            if (GUILayout.Button("Client"))
            {
                NetworkManager.Singleton.StartClient();
            }

            if (GUILayout.Button("Server"))
            {
                NetworkManager.Singleton.StartServer();
            }
        }

        static void StatusLabels()
        {
            var mode = NetworkManager.Singleton.IsHost ?
                "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";
            GUILayout.Label("Transport: " +
                NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);
        }
    }
}
