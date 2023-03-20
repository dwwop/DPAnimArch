using System.Collections;
using UnityEngine;

namespace UMSAGL.Scripts
{
	public class SampleGraph : MonoBehaviour {

		public GameObject graphPrefab;
		private Graph graph;
		public GameObject assPrefab;

		private IEnumerator LayoutTest()
		{
			//relayout test after 10s
			yield return new WaitForSecondsRealtime(10);
			graph.Layout();
		}

		void Start () {
			//Instantiate graph
			var go = GameObject.Instantiate(graphPrefab);
			graph = go.GetComponent<Graph>();

			//Generate
			Generate();

			//Relayout after 10s, for testing purposes only
			StartCoroutine(LayoutTest());
		}

		void Generate()
		{
			//Demonstration how to create graph using provided API
			var a = graph.AddNode();
        
			var b = graph.AddNode();
			/*
		int count = 3;

		for (int i = 0; i < count; i++)
		{
			var n = graph.AddNode();
			graph.AddEdge(a, n);
			n = graph.AddNode();
			graph.AddEdge(b, n);
		}*/
			var c = graph.AddNode();
			var d = graph.AddNode();

			graph.AddEdge(a, b);
			graph.AddEdge(d, b,assPrefab);

			graph.Layout();
		}
	}
}
