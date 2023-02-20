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
            go.name = name;
            var visualEditor = new VisualEditor();
            visualEditor.UpdateNodeName(go);
        }

        [ServerRpc(RequireOwnership = false)]
        public void DeleteClassServerRpc(string className)
        {
            if (IsClient && !IsHost)
                return;
            UIEditorManager.Instance.mainEditor.DeleteNode(className);
        }

        [ClientRpc]
        public void AddAttributeClientRpc(string attributeName, string attributeText, ulong classNetworkId)
        {
            if (IsServer)
                return;
            var objects = NetworkManager.Singleton.SpawnManager.SpawnedObjects;
            var obj = objects[classNetworkId];
            var classNo = obj.GetComponent<NetworkObject>().gameObject;
            var visualEditor = new VisualEditorClient();
            visualEditor.AddAttribute(attributeName, attributeText, classNo);
        }

        [ClientRpc]
        public void UpdateAttributeClientRpc(string oldAttributeName, string newAttributeName, string attributeText, ulong classNetworkId)
        {
            if (IsServer)
                return;
            var objects = NetworkManager.Singleton.SpawnManager.SpawnedObjects;
            var obj = objects[classNetworkId];
            var classNo = obj.GetComponent<NetworkObject>().gameObject;
            var visualEditor = new VisualEditorClient();
            visualEditor.UpdateAttribute(oldAttributeName, newAttributeName, attributeText, classNo);
        }

        [ClientRpc]
        public void DeleteAttributeClientRpc(string attributeName, ulong classNetworkId)
        {
            if (IsServer)
                return;
            var objects = NetworkManager.Singleton.SpawnManager.SpawnedObjects;
            var obj = objects[classNetworkId];
            var classGo = obj.GetComponent<NetworkObject>().gameObject;
            var visualEditor = new VisualEditorClient();
            visualEditor.DeleteAttribute(attributeName, classGo);
        }

        [ClientRpc]
        public void AddMethodClientRpc(string methodName, string methodText, ulong classNetworkId)
        {
            if (IsServer)
                return;
            var objects = NetworkManager.Singleton.SpawnManager.SpawnedObjects;
            var obj = objects[classNetworkId];
            var classGo = obj.GetComponent<NetworkObject>().gameObject;
            var visualEditor = new VisualEditorClient();
            visualEditor.AddMethod(methodName, methodText, classGo);
        }

        [ClientRpc]
        public void UpdateMethodClientRpc(string oldMethodName, string newMethodName, string methodText, ulong classNetworkId)
        {
            if (IsServer)
                return;
            var objects = NetworkManager.Singleton.SpawnManager.SpawnedObjects;
            var obj = objects[classNetworkId];
            var classNo = obj.GetComponent<NetworkObject>().gameObject;
            var visualEditor = new VisualEditorClient();
            visualEditor.UpdateMethod(oldMethodName, newMethodName, methodText, classNo);
        }

        [ClientRpc]
        public void DeleteMethodClientRpc(string methodName, ulong classNetworkId)
        {
            if (IsServer)
                return;
            var objects = NetworkManager.Singleton.SpawnManager.SpawnedObjects;
            var obj = objects[classNetworkId];
            var classGo = obj.GetComponent<NetworkObject>().gameObject;
            var visualEditor = new VisualEditorClient();
            visualEditor.DeleteMethod(methodName, classGo);
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
