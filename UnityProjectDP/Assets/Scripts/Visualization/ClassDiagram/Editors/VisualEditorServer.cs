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
        private Transform GetGraphUnits()
        {
            var graphTransform = DiagramPool.Instance.ClassDiagram.graph.gameObject.GetComponent<Transform>();
            var graphUnits = graphTransform.Find("Units");
            return graphUnits.GetComponent<Transform>();
        }

        public override void AddAttribute(ClassInDiagram classInDiagram, Attribute attribute)
        {
            base.AddAttribute(classInDiagram, attribute);

            var parentClass = classInDiagram.VisualObject.GetComponent<NetworkObject>();
            Spawner.Instance.AddAttributeClientRpc(attribute.Name, GetStringFromAttribute(attribute), parentClass.NetworkObjectId);
        }

        public override void AddMethod(ClassInDiagram classInDiagram, Method method)
        {
            base.AddMethod(classInDiagram, method);

            var parentClass = classInDiagram.VisualObject.GetComponent<NetworkObject>();
            Spawner.Instance.AddMethodClientRpc(method.Name, GetStringFromMethod(method), parentClass.NetworkObjectId);
        }

        public override GameObject CreateNode(Class newClass)
        {
            var nodeGo = base.CreateNode(newClass);

            var nodeNo = nodeGo.GetComponent<NetworkObject>();
            nodeNo.Spawn();
            Spawner.Instance.SetNetworkObjectNameClientRpc(nodeNo.name, nodeNo.NetworkObjectId);

            if (!nodeNo.TrySetParent(GetGraphUnits(), false))
            {
                throw new InvalidParentException(nodeNo.name);
            }
            Spawner.Instance.SetClassNameClientRpc(nodeNo.name, nodeNo.NetworkObjectId);

            return nodeGo;
        }

        public override GameObject CreateRelation(Relation relation)
        {
            var prefab = relation.PropertiesEaType switch
            {
                "Association" => relation.PropertiesDirection switch
                {
                    "Source -> Destination" => DiagramPool.Instance.networkAssociationSDPrefab,
                    "Destination -> Source" => DiagramPool.Instance.networkAssociationDSPrefab,
                    "Bi-Directional" => DiagramPool.Instance.networkAssociationFullPrefab,
                    _ => DiagramPool.Instance.networkAssociationNonePrefab
                },
                "Generalization" => DiagramPool.Instance.networkGeneralizationPrefab,
                "Dependency" => DiagramPool.Instance.networkDependsPrefab,
                "Realisation" => DiagramPool.Instance.networkRealisationPrefab,
                _ => DiagramPool.Instance.networkAssociationNonePrefab
            };

            var sourceClassGo = DiagramPool.Instance.ClassDiagram.FindClassByName(relation.FromClass).VisualObject;
            var destinationClassGo = DiagramPool.Instance.ClassDiagram.FindClassByName(relation.ToClass).VisualObject;

            var edge = DiagramPool.Instance.ClassDiagram.graph.AddEdge(sourceClassGo, destinationClassGo, prefab);

            var edgeNo = edge.GetComponent<NetworkObject>();


            if (edge.gameObject.transform.childCount > 0)
                DiagramPool.Instance.ClassDiagram.StartCoroutine(QuickFix(edge.transform.GetChild(0).gameObject));

            edgeNo.Spawn();

            if (!edgeNo.TrySetParent(GetGraphUnits(), false))
            {
                throw new InvalidParentException(edgeNo.name);
            }

            return edge;
        }
    }
}
