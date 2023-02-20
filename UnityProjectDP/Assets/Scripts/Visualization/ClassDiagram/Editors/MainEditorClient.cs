using Networking;
using Unity.Netcode;
using UnityEngine;

namespace AnimArch.Visualization.Diagrams
{
    public class MainEditorClient : MainEditor
    {
        public MainEditorClient(IVisualEditor visualEditor) : base(visualEditor)
        {
        }

        public override void DeleteNode(string className)
        {
            Spawner.Instance.DeleteClassServerRpc(className);
        }

        public override void DeleteRelation(GameObject relation)
        {
            var relationNetworkId = relation.GetComponent<NetworkObject>().NetworkObjectId;
            Spawner.Instance.DeleteRelationServerRpc(relationNetworkId);
        }
    }
}
