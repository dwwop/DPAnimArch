using Networking;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace AnimArch.Visualization.Diagrams
{
    public class ClassDiagramBuilderServer : ClassDiagramBuilder
    {
        public override void MakeNetworkedGraph()
        {
            var graphGo = DiagramPool.Instance.ClassDiagram.graph.gameObject;
            Debug.Assert(graphGo);

            var graphNo = graphGo.GetComponent<NetworkObject>();
            graphNo.Spawn();

            var unitsGo = GameObject.Find("Units");
            var unitsNo = unitsGo.GetComponent<NetworkObject>();
            unitsNo.Spawn();

            if (!unitsNo.TrySetParent(graphGo))
            {
                throw new InvalidParentException(unitsGo.name);
            }
            unitsGo.GetComponent<Transform>().localScale = new Vector3(1, 1, 1);
        }

    }
}
