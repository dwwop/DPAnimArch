using AnimArch.Visualization.Diagrams;
using AnimArch.Visualization.UI;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace AnimArch.Visualization
{
    [Serializable]
    public class GameObjectEvent : UnityEvent<GameObject>
    {
    };

    public class Clickable : MonoBehaviour
    {
        public GameObjectEvent triggerHighlighAction;
        public GameObjectEvent triggerUnhighlighAction;
        private Vector3 _screenPoint;
        private Vector3 _offset;

        private bool _selectedElement;
        private bool _changedPos;


        private void OnMouseDown()
        {
            var temp = ToolManager.Instance.SelectedTool;
            if (temp == "DiagramMovement")
                OnClassSelected();
        }

        private void OnClassSelected()
        {
            _selectedElement = true;
            var position = gameObject.transform.position;
            if (Camera.main == null) return;
            _screenPoint = Camera.main.WorldToScreenPoint(position);
            _offset = position -
                     Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                         _screenPoint.z));
        }

        private void OnMouseUp()
        {
            if (!_changedPos && MenuManager.Instance.isSelectingNode)
            {
                UIEditorManager.Instance.SelectNode(gameObject);
            }

            _changedPos = false;
            _selectedElement = false;
        }

        private void OnMouseDrag()
        {
            if (_selectedElement == false ||
                (ToolManager.Instance.SelectedTool != "DiagramMovement" && !MenuManager.Instance.isSelectingNode)
                || IsMouseOverUI())
                return;

            var cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z);
            if (Camera.main == null) return;
            var cursorPosition = Camera.main.ScreenToWorldPoint(cursorPoint) + _offset;
            cursorPosition.z = transform.position.z;
            if (transform.position == cursorPosition) return;
            _changedPos = true;
            transform.position = cursorPosition;

            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(name);
            classInDiagram.ParsedClass = ParsedEditor.UpdateNodeGeometry(classInDiagram.ParsedClass, classInDiagram.VisualObject);
        }

        private void OnMouseOver()
        {
            if (Input.GetMouseButtonDown(0) && ToolManager.Instance.SelectedTool == "Highlighter" && !IsMouseOverUI())
            {
                triggerHighlighAction.Invoke(gameObject);
            }

            if (Input.GetMouseButtonDown(1) && ToolManager.Instance.SelectedTool == "Highlighter" && !IsMouseOverUI())
            {
                triggerUnhighlighAction.Invoke(gameObject);
            }

            if (Input.GetMouseButtonDown(0) && MenuManager.Instance.isCreating)
            {
                MenuManager.Instance.SelectClass(gameObject.name);
            }

            if (Input.GetMouseButtonDown(0) && MenuManager.Instance.isPlaying)
            {
                MenuManager.Instance.SelectPlayClass(gameObject.name);
            }
        }

        private static bool IsMouseOverUI()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
    }
}