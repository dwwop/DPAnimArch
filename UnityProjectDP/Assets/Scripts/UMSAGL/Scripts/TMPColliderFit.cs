using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class TMPColliderFit : MonoBehaviour {

	public float zSize = 5;

	private BoxCollider bc;
	private RectTransform rt;

	// Use this for initialization
	void Awake () {
		bc = GetComponent<BoxCollider>();
		rt = GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
		var size = rt.rect.size;
		bc.size = new Vector3(size.x, size.y, zSize);
	}
}
