using Microsoft.Msagl.Core.Layout;
using UnityEngine;

namespace UMSAGL.Scripts
{
	public class UNode : Unit
	{
		public Node GraphNode { get; set; }

		public Rect Size {
			get
			{
				return GetComponent<RectTransform>().rect;
			}
		}

		private Rect oldSize;

		protected override void OnDestroy()
		{
			_graph.RemoveNode(gameObject);
		}

		private void Start()
		{
			oldSize = GetComponent<RectTransform>().rect;
		}

		protected override void Update()
		{
			base.Update();
			var size = GetComponent<RectTransform>().rect;
			if (transform.hasChanged || oldSize != size)
			{
				_graph.UpdateGraph();
				transform.hasChanged = false;
				oldSize = size;
			}
		}
	}
}
