using UnityEngine;
using Microsoft.Msagl.Miscellaneous;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Core.Routing;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Msagl.Core.DataStructures;
using UnityEngine.UI.Extensions;
using System;

public class Graph : MonoBehaviour
{

	public GameObject nodePrefab;
	public GameObject edgePrefab;
	public float factor = 0.2f;
	public Vector2 margins;

	private GeometryGraph graph;
	private LayoutAlgorithmSettings settings;

	private Task graphTask = null;
	private bool reroute = false;
	private bool redraw = false;
	private bool reposition = false;
	private bool relayout = false;

	private Transform units;

	public Vector2 Rect
	{
		get
		{
			graph.UpdateBoundingBox();
			var size = graph.BoundingBox.Size;
			return new Vector2(ToUnitySpace((float)size.Width) + margins.x, ToUnitySpace((float)size.Height) + margins.y);
		}
	}

	public void Center()
	{
		graph.UpdateBoundingBox();
		units.localPosition = new Vector3(ToUnitySpace(graph.BoundingBox.Center.X), ToUnitySpace(graph.BoundingBox.Center.Y)) * -1.0f;
	}

	public GameObject AddNode()
	{
		var go = GameObject.Instantiate(nodePrefab, units);

		//Following step required otherwise Size will return wrong rect
		Canvas.ForceUpdateCanvases();

		var unode = go.GetComponent<UNode>();
		double w = ToGraphSpace(unode.Size.width);
		double h = ToGraphSpace(unode.Size.height);

		Node node = new Node(CurveFactory.CreateRectangle(w, h, new Point()));
		node.UserData = go;
		unode.GraphNode = node;
		graph.Nodes.Add(node);

		return go;
	}

	public void RemoveNode(GameObject node)
	{
		var graphNode = node.GetComponent<UNode>().GraphNode;
		foreach (var edge in graphNode.Edges)
		{
			GameObject.Destroy((GameObject)edge.UserData);
			//in MSAGL edges are automatically removed, only UnityObjects have to be removed
		}
		graph.Nodes.Remove(graphNode);
		GameObject.Destroy(node);
	}

	public GameObject AddEdge(GameObject from, GameObject to)
	{
		return AddEdge(from, to, edgePrefab);
	}

	public GameObject AddEdge(GameObject from, GameObject to, GameObject prefab)
	{
		var go = GameObject.Instantiate(prefab, units);
		var uEdge = go.GetComponent<UEdge>();

		Edge edge = new Edge(from.GetComponent<UNode>().GraphNode, to.GetComponent<UNode>().GraphNode);
		edge.LineWidth = ToGraphSpace(uEdge.Width);
		edge.UserData = go;
		uEdge.GraphEdge = edge;
		graph.Edges.Add(edge);

		return go;
	}

	public void RemoveEdge(GameObject edge)
	{
		graph.Edges.Remove(edge.GetComponent<UEdge>().GraphEdge);
		GameObject.Destroy(edge);
	}

	double ToGraphSpace(float x)
	{
		return x / factor;
	}

	float ToUnitySpace(double x)
	{
		return (float)x * factor;
	}

	protected virtual void Awake()
	{
		graph = new GeometryGraph();
		units = transform.Find("Units"); //extra object to center graph
		settings = new SugiyamaLayoutSettings();
		settings.EdgeRoutingSettings.EdgeRoutingMode = EdgeRoutingMode.RectilinearToCenter;
	}

	void PositionNodes()
	{
		foreach (var node in graph.Nodes)
		{
			var go = (GameObject)node.UserData;
			go.transform.localPosition = new Vector3(ToUnitySpace(node.Center.X), ToUnitySpace(node.Center.Y), 0.0f);
		}
	}

	void UpdateNodes()
	{
		Canvas.ForceUpdateCanvases();
		foreach (var node in graph.Nodes)
		{
			var go = (GameObject)node.UserData;
			node.Center = new Point(ToGraphSpace(go.transform.localPosition.x), ToGraphSpace(go.transform.localPosition.y));
			var unode = go.GetComponent<UNode>();
			node.BoundingBox = new Rectangle(new Size(ToGraphSpace(unode.Size.width), ToGraphSpace(unode.Size.height)), node.Center);
		}
	}

	void RedrawEdges()
	{
		foreach (var edge in graph.Edges)
		{
			List<Vector2> vertices = new List<Vector2>();
			GameObject go = (GameObject)edge.UserData;

			Curve curve = edge.Curve as Curve;
			if (curve != null)
			{
				Point p = curve[curve.ParStart];
				vertices.Add(new Vector3(ToUnitySpace(p.X), ToUnitySpace(p.Y), 0));
				foreach (ICurve seg in curve.Segments)
				{
					p = seg[seg.ParEnd];
					vertices.Add(new Vector3(ToUnitySpace(p.X), ToUnitySpace(p.Y), 0));
				}
			}
			else
			{
				LineSegment ls = edge.Curve as LineSegment;
				if (ls != null)
				{
					Point p = ls.Start;
					vertices.Add(new Vector3(ToUnitySpace(p.X), ToUnitySpace(p.Y)));
					p = ls.End;
					vertices.Add(new Vector3(ToUnitySpace(p.X), ToUnitySpace(p.Y)));
				}
			}

			go.GetComponent<UEdge>().Points = vertices.ToArray();
		}
	}

	private async void Forget(Task t, Action f = null)
	{
		try
		{
			await t;
		}
		catch (System.Exception e)
		{
			Debug.LogWarning(e);
		}
		f?.Invoke();
	}

	public void UpdateGraph()
	{
		reroute = true;
	}

	public void Layout()
	{
		relayout = true;
	}

	private void Update()
	{
		if (reposition)
		{
			PositionNodes();
			Center();
			reposition = false;
		}
		if (redraw)
		{
			RedrawEdges();
			redraw = false;
		}
		if (relayout)
		{
			if (graphTask == null)
			{
				UpdateNodes();
				graphTask = Task.Run(() =>
				{
					LayoutHelpers.CalculateLayout(graph, settings, null);
					LayoutHelpers.RouteAndLabelEdges(graph, settings, graph.Edges);
				});
				Forget(graphTask, () =>
				{
					graphTask = null;
					reposition = true;
					redraw = true;
				});
				relayout = false;
			}
		}
		if (reroute)
		{
			if (graphTask == null)
			{
				UpdateNodes();
				graphTask = Task.Run(() => LayoutHelpers.RouteAndLabelEdges(graph, settings, graph.Edges));
				Forget(graphTask, () =>
				{
					graphTask = null;
					redraw = true;
				});
				reroute = false;
			}
		}
	}
}
