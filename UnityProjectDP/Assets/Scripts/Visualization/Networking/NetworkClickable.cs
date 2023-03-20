using Unity.Netcode;
using UnityEngine;
using Visualization.ClassDiagram;
using Visualization.ClassDiagram.Editors;
using Visualization.UI;

namespace Visualization.Networking
{
    public class NetworkClickable : Clickable
    {

        protected override void OnMouseDrag()
        {
            if (_selectedElement == false ||
                (ToolManager.Instance.SelectedTool != ToolManager.Tool.DiagramMovement && !MenuManager.Instance.isSelectingNode)
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

            if (NetworkManager.Singleton.IsServer)
            {
                var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(name);
                classInDiagram.ParsedClass = ParsedEditor.UpdateNodeGeometry(classInDiagram.ParsedClass, classInDiagram.VisualObject);
            }
            else
            {
                var clGetComponent = GetComponent<NetworkClass>();
                clGetComponent.UpdatePostionServerRpc(cursorPosition);
            }
        }
    }
}
