using AnimArch.Visualization.UI;
using Networking;
using System.Collections.Generic;
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

        public override void AddAttribute(string targetClass, Attribute attribute)
        {
            Spawner.Instance.AddAttributeServerRpc(targetClass, attribute.Name, attribute.Type);
        }

        public override void DeleteAttribute(string className, string attributeName)
        {
            Spawner.Instance.DeleteAttributeServerRpc(className, attributeName);
        }

        public override void AddMethod(string targetClass, Method method)
        {
            string arguments = string.Join(",", method.arguments);
            Spawner.Instance.AddMethodServerRpc(targetClass, method.Name, method.ReturnValue, arguments);
        }

        public override void DeleteMethod(string className, string methodName)
        {
            Spawner.Instance.DeleteMethodServerRpc(className, methodName);
        }
    }
}
