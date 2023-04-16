using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Visualization.ClassDiagram;
using Visualization.ClassDiagram.Editors;
using Visualization.UI;

namespace Visualization
{
    [Serializable]
    public class GameObjectEvent : UnityEvent<GameObject>
    {
    };

    public class Clickable : MonoBehaviour
    {
        public GameObjectEvent triggerHighlighAction;
        public GameObjectEvent triggerUnhighlighAction;
        protected Vector3 _screenPoint;
        protected Vector3 _offset;

        protected bool _selectedElement;
        protected bool _changedPos;


        private void OnMouseDown()
        {
            if (ToolManager.Instance.SelectedTool == ToolManager.Tool.DiagramMovement)
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

        protected virtual void OnMouseDrag()
        {
            if (_selectedElement == false ||
                (ToolManager.Instance.SelectedTool != ToolManager.Tool.DiagramMovement &&
                 !MenuManager.Instance.isSelectingNode)
                || IsMouseOverUI())
                return;

            var cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z);
            if (Camera.main == null)
                return;
            var cursorPosition = Camera.main.ScreenToWorldPoint(cursorPoint) + _offset;
            cursorPosition.z = transform.position.z;
            if (transform.position == cursorPosition)
                return;
            _changedPos = true;
            transform.position = cursorPosition;
            
            UIEditorManager.Instance.mainEditor.UpdateNodeGeometry(name);
        }

        private void OnMouseOver()
        {
            if (Input.GetMouseButtonDown(0) && ToolManager.Instance.SelectedTool == ToolManager.Tool.Highlighter &&
                !IsMouseOverUI())
            {
                triggerHighlighAction.Invoke(gameObject);
            }

            if (Input.GetMouseButtonDown(1) && ToolManager.Instance.SelectedTool == ToolManager.Tool.Highlighter &&
                !IsMouseOverUI())
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

        protected static bool IsMouseOverUI()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
    }
}
