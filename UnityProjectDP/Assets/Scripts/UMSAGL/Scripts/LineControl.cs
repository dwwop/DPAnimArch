using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineControl : MonoBehaviour
{
    void OnMouseOver()
    {
        if ( ToolManager.Instance.SelectedTool == "Highlighter")
        {
            Debug.Log("MouseOver");
        }
        if (Input.GetMouseButtonDown(1) && ToolManager.Instance.SelectedTool == "Highlighter")
        {
            //triggerUnhighlighAction.Invoke(gameObject);
        }
    }
}
