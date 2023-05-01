using UnityEngine;

namespace Visualization.UI
{
    public class SelectFirstState : State
    {
        public override void Select(GameObject selectedNode)
        {
            UIEditorManager.Instance.relation.SourceModelName = selectedNode.name;
            Animation.Animation.Instance.HighlightClass(selectedNode.name, true);
            UIEditorManager.Instance.state = new SelectSecondState();
        }
    }
}
