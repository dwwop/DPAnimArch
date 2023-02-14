using AnimArch.Extensions;
using AnimArch.Visualization.Diagrams;
using AnimArch.Visualization.UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Networking
{
    [RequireComponent(typeof(NetworkObject))]
    public class Spawner : NetworkSingleton<Spawner>
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        [ClientRpc]
        public void SetNetworkObjectNameClientRpc(string name, ulong id)
        {
            if (IsServer)
                return;
            var objects = NetworkManager.Singleton.SpawnManager.SpawnedObjects;
            objects[id].name = name;
        }

        [ClientRpc]
        public void SetClassNameClientRpc(string name, ulong id)
        {
            if (IsServer)
                return;
            var objects = NetworkManager.Singleton.SpawnManager.SpawnedObjects;
            var obj = objects[id];
            var go = obj.GetComponent<NetworkObject>().gameObject;
            VisualEditor.UpdateNodeName(go);
        }

        [ClientRpc]
        public void SetButtonsActiveClientRpc(bool active = true)
        {
            if (IsServer)
                return;
            DiagramPool.Instance.ClassDiagram.graph.GetComponentsInChildren<GraphicRaycaster>()
                .ForEach(x => x.enabled = active);
        }

        [ServerRpc(RequireOwnership = false)]
        public void CreateGraphServerRpc()
        {
            if (IsClient && !IsHost)
                return;
            UIEditorManager.Instance.InitializeCreation();
            GraphCreatedClientRpc();
        }


        [ClientRpc]
        public void GraphCreatedClientRpc()
        {
            if (IsServer)
                return;
            DiagramPool.Instance.ClassDiagram.graph = GameObject.Find("Graph").GetComponent<Graph>();
        }
    }
}
