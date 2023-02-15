using AnimArch.Extensions;
using AnimArch.Visualization.UI;
using Networking;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace AnimArch.Visualization.Diagrams
{
    public class VisualEditorServer : VisualEditor
    {
        public override GameObject CreateNode(Class newClass)
        {
            var nodeGo = base.CreateNode(newClass);

            var nodeNo = nodeGo.GetComponent<NetworkObject>();
            nodeNo.Spawn();
            Spawner.Instance.SetNetworkObjectNameClientRpc(nodeNo.name, nodeNo.NetworkObjectId);

            var graphTransform = DiagramPool.Instance.ClassDiagram.graph.gameObject.GetComponent<Transform>();
            var graphUnits = graphTransform.Find("Units");

            var utr = graphUnits.GetComponent<Transform>();
            if (!nodeNo.TrySetParent(utr))
            {
                throw new InvalidParentException(utr.name);
            }
            Spawner.Instance.SetClassNameClientRpc(nodeNo.name, nodeNo.NetworkObjectId);

            return nodeGo;
        }
    }
}
