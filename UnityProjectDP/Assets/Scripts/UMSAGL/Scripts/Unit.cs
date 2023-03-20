using UnityEngine;

namespace UMSAGL.Scripts
{
	public abstract class Unit : MonoBehaviour {

		protected Graph _graph;

		public object UserData
		{
			get; set;
		}

		// Use this for initialization
		protected virtual void Awake ()
		{
			TryGetGraph();
		}

		protected virtual void Update()
		{
			if (!_graph)
				TryGetGraph();
		}

		private bool TryGetGraph()
		{
			var graph = GetComponentInParent<Graph>();
			if (!graph)
				return false;
			else
			{
				_graph = graph;
				return true;
			}
		}

		protected abstract void OnDestroy();
	}
}
