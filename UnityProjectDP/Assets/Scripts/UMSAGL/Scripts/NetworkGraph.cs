using UnityEngine;
using Microsoft.Msagl.Core.Geometry.Curves;
using System.Collections.Generic;
using Networking;
using Unity.Netcode;

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
}
