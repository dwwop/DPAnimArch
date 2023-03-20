using UnityEngine;

namespace UMSAGL.Scripts
{
	public class CanvasFitter : MonoBehaviour {

		// Use this for initialization
		void Start () {
		
		}
	
		// Update is called once per frame
		void Update () {
			var graph = GetComponent<Graph>();
			GetComponent<RectTransform>().sizeDelta = graph.Rect;
			graph.Center();
		}
	}
}
