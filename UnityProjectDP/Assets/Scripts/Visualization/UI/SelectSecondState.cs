using UnityEngine;

namespace Visualization.UI
{
    public class SelectSecondState : State
    {
        private GameObject _firstNode;

        public override void Select(GameObject selectedNode)
        {
            if (selectedNode.name == UIEditorManager.Instance.relation.SourceModelName)
            {
                Animation.Animation.Instance.HighlightClass(UIEditorManager.Instance.relation.SourceModelName, false);
                UIEditorManager.Instance.relation.SourceModelName = null;
                UIEditorManager.Instance.state = new SelectFirstState();
                return;
            }

            UIEditorManager.Instance.relation.TargetModelName = selectedNode.name;
            UIEditorManager.Instance.AddRelation();
        }
    }
}
