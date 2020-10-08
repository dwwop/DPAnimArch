using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


[Serializable]
public class GameObjectEvent : UnityEvent<GameObject> { };
public class Clickable : MonoBehaviour
{
    public GameObjectEvent triggerHighlighAction;
    public GameObjectEvent triggerUnhighlighAction;
    private Vector3 screenPoint;
    private Vector3 offset;

    private bool selectedElement = false;


    private void OnMouseDown()
    {
        string temp = ToolManager.Instance.SelectedTool;
        if (temp == "DiagramMovement")
            OnClassSelected();
    }

    private void OnClassSelected()
    {
        selectedElement = true;
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    void OnMouseUp()
    {
        selectedElement = false;
    }

    void OnMouseDrag()
    {
        if (selectedElement == false || ToolManager.Instance.SelectedTool != "DiagramMovement"|| IsMouseOverUI())
        {
            return;
        }

        Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorPoint) + offset;
        cursorPosition.z = transform.position.z;
        transform.position = cursorPosition;
    }
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && ToolManager.Instance.SelectedTool == "Highlighter" && !IsMouseOverUI())
        {
            triggerHighlighAction.Invoke(gameObject);
        }
        if (Input.GetMouseButtonDown(1) && ToolManager.Instance.SelectedTool == "Highlighter" && !IsMouseOverUI())
        {
            triggerUnhighlighAction.Invoke(gameObject);
        }
        if (Input.GetMouseButtonDown(0)&&MenuManager.Instance.isCreating==true)
        {
            MenuManager.Instance.SelectClass(this.gameObject.name);
        }
    }
    private bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}