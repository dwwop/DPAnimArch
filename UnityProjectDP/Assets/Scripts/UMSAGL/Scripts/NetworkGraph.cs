using System.Collections.Generic;
using Microsoft.Msagl.Core.Geometry.Curves;
using Unity.Netcode;
using UnityEngine;
using Visualization.Networking;

namespace UMSAGL.Scripts
{
    public class NetworkGraph : Graph
    {
        protected override void RedrawEdges()
        {
            foreach (var edge in _graph.Edges)
            {
                var vertices = new List<Vector2>();
                var go = (GameObject)edge.UserData;

                switch (edge.Curve)
                {
                    case Curve curve:
                    {
                        var p = curve[curve.ParStart];
                        vertices.Add(new Vector3(ToUnitySpace(p.X), ToUnitySpace(p.Y), 0));
                        foreach (var seg in curve.Segments)
                        {
                            p = seg[seg.ParEnd];
                            vertices.Add(new Vector3(ToUnitySpace(p.X), ToUnitySpace(p.Y), 0));
                        }

                        break;
                    }
                    case LineSegment ls:
                    {
                        var p = ls.Start;
                        vertices.Add(new Vector3(ToUnitySpace(p.X), ToUnitySpace(p.Y)));
                        p = ls.End;
                        vertices.Add(new Vector3(ToUnitySpace(p.X), ToUnitySpace(p.Y)));
                        break;
                    }
                }

                var verticesArray = vertices.ToArray();
                go.GetComponent<UEdge>().Points = verticesArray;
                var edgeNo = go.GetComponent<NetworkObject>();
                Spawner.Instance.SetLinePointsClientRpc(edgeNo.NetworkObjectId, verticesArray);
                Spawner.Instance.SetLineResolutionClientRpc(edgeNo.NetworkObjectId, go.GetComponent<UnityEngine.UI.Extensions.UILineRenderer>().Resoloution);
            }
        }

        public override void RemoveNode(GameObject node)
        {
            if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost) // Only server can remove components
                return;
            var graphNode = node.GetComponent<UNode>().GraphNode;
            foreach (var edge in graphNode.Edges)
            {
                Destroy((GameObject)edge.UserData);
                //in MSAGL edges are automatically removed, only UnityObjects have to be removed
            }

            _graph.Nodes.Remove(graphNode);
            Destroy(node);
        }
    }
}
