using AnimArch.Extensions;
using AnimArch.Visualization.Diagrams;
using AnimArch.Visualization.UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

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
        public void SetNetworkObjectNameClientRpc(string name, ulong networkId)
        {
            if (IsServer)
                return;
            var objects = NetworkManager.Singleton.SpawnManager.SpawnedObjects;
            objects[networkId].name = name;
        }

        [ClientRpc]
        public void SetClassNameClientRpc(string name, ulong networkId)
        {
            if (IsServer)
                return;
            var objects = NetworkManager.Singleton.SpawnManager.SpawnedObjects;
            var obj = objects[networkId];
            var go = obj.GetComponent<NetworkObject>().gameObject;
            var visualEditor = new VisualEditor();
            visualEditor.UpdateNodeName(go);
        }

        [ClientRpc]
        public void AddAttributeClientRpc(string attributeName, string attributeText, ulong parentClassNetworkId)
        {
            if (IsServer)
                return;
            var objects = NetworkManager.Singleton.SpawnManager.SpawnedObjects;
            var obj = objects[parentClassNetworkId];
            var parentClass = obj.GetComponent<NetworkObject>().gameObject;
            var visualEditor = new VisualEditorClient();
            visualEditor.AddAttribute(attributeName, attributeText, parentClass);
        }

        [ClientRpc]
        public void AddMethodClientRpc(string methodName, string methodText, ulong parentClassNetworkId)
        {
            if (IsServer)
                return;
            var objects = NetworkManager.Singleton.SpawnManager.SpawnedObjects;
            var obj = objects[parentClassNetworkId];
            var parentClass = obj.GetComponent<NetworkObject>().gameObject;
            var visualEditor = new VisualEditorClient();
            visualEditor.AddMethod(methodName, methodText, parentClass);
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
            DiagramPool.Instance.ClassDiagram.graph.enabled = false;
        }

        [ClientRpc]
        public void SetLinePointsClientRpc(ulong networkId, Vector2[] points)
        {
            if (IsServer)
                return;
            var objects = NetworkManager.Singleton.SpawnManager.SpawnedObjects;
            Debug.Assert(objects[networkId].GetComponent<UILineRenderer>());
            var lineRenderer = objects[networkId].GetComponent<UILineRenderer>();
            lineRenderer.Points = points;
        }

        [ClientRpc]
        public void SetLineResolutionClientRpc(ulong networkId, float resolution)
        {
            if (IsServer)
                return;
            var objects = NetworkManager.Singleton.SpawnManager.SpawnedObjects;
            Debug.Assert(objects[networkId].GetComponent<UILineRenderer>());
            var lineRenderer = objects[networkId].GetComponent<UILineRenderer>();
            lineRenderer.Resoloution = resolution;
        }
    }
}
