using Microsoft.Msagl.Core.Layout;
using UnityEngine;
using UnityEngine.UI.Extensions;
using System.Linq;

public class UEdge : Unit
{

	public GameObject startCap = null;
	public GameObject endCap = null;
	public bool dashed = false;
	public float segmentLength = 10f;

	private UILineRenderer lineRenderer;
	private bool _dashed = false;
	private float _segmentLength = 0f;

	public Edge GraphEdge { get; set; }

	public float SegmentLength
	{
		get
		{
			return _segmentLength;
		}
		set
		{
			_segmentLength = segmentLength = Mathf.Max(value, 1.0f);
			Dashed = dashed;
		}
	}

	public float Width
	{
		get
		{
			return Mathf.Max(lineRenderer.lineThickness, 10f);
		}
	}

	public bool Dashed
	{
		get
		{
			return _dashed;
		}
		set
		{
			_dashed = dashed = value;
			Dash(value);
		}
	}

	public Vector2[] Points
	{
		get
		{
			return lineRenderer.Points;
		}
		set
		{
			lineRenderer.Points = value;
			Dashed = dashed;
			UpdateCaps();
		}
	}

	protected override void OnDestroy()
	{
		graph.RemoveEdge(gameObject);
	}

	private void Dash(bool active)
	{
		if (active && Points.Length > 1)
		{
			lineRenderer.LineList = true;
			lineRenderer.ImproveResolution = ResolutionMode.PerLine;
			var prev = Points.First();
			var totalDistance = 0f;
			foreach (var next in Points.Skip(1))
			{
				totalDistance += Vector2.Distance(prev, next);
				prev = next;
			}
			lineRenderer.Resoloution = totalDistance / segmentLength;
		}
		else
		{
			lineRenderer.LineList = false;
			lineRenderer.ImproveResolution = ResolutionMode.None;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		lineRenderer = GetComponent<UILineRenderer>();
		Points = lineRenderer.Points;
		SegmentLength = segmentLength;
		UpdateCaps();
	}

	private float CapAngle(Vector2 p1, Vector2 p2)
	{
		float eulerAngle = 0f;
		if (Mathf.Abs(p1.x - p2.x) > Mathf.Abs(p1.y - p2.y))
		{
			eulerAngle = p1.x > p2.x ? 180f : 0f;
		}
		else
		{
			eulerAngle = p1.y > p2.y ? 270f : 90f;
		}
		return eulerAngle;
	}

	private void UpdateCap(GameObject capPrefab, string capName, Vector2 targetPoint, Vector2 dirPoint)
	{
		if (Points.Length > 1)
		{
			var capTransform = transform.Find(capName);
			if (capPrefab != null)
			{
				if (capTransform == null)
				{
					capTransform = GameObject.Instantiate(capPrefab, transform).transform;
					capTransform.name = capName;
					capTransform.GetComponentInChildren<UIPolygon>()?.SetAllDirty();
					Canvas.ForceUpdateCanvases();
				}
				var angle = CapAngle(dirPoint, targetPoint);
				capTransform.localEulerAngles = new Vector3(0, 0, angle);
				capTransform.localPosition = new Vector3(targetPoint.x, targetPoint.y, 0);
			}
			else if (capTransform != null)
			{
				GameObject.Destroy(capTransform.gameObject);
				Canvas.ForceUpdateCanvases();
			}
		}
	}

	private void UpdateCaps()
	{
		if (Points.Length > 1)
		{
			UpdateCap(startCap, "StartCap", Points[0], Points[1]);
			UpdateCap(endCap, "EndCap", Points[Points.Length - 1], Points[Points.Length - 2]);
		}
	}

	private void Update()
	{
        
		if (SegmentLength != segmentLength)
		{
			SegmentLength = segmentLength;
		}
		else if (Dashed != dashed)
		{
			Dashed = dashed;
        }


    }
    public void ChangeColor(Color c)
    {
        lineRenderer.color = c;
        if (endCap != null || startCap!=null)
        {
            UIPolygon[] p = GetComponentsInChildren<UIPolygon>();
            if (p != null)
            {
                foreach (UIPolygon pol in p)
                {
                    pol.color = c;
                }
            }
            UILineRenderer[] lr= GetComponentsInChildren<UILineRenderer>();
            if (lr != null)
            {
                foreach (UILineRenderer l in lr)
                {
                    l.color = c;
                }
            }
        }
    }

}
