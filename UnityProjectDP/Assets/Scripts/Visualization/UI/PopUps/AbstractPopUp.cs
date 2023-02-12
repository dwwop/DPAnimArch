using AnimArch.Extensions;
using AnimArch.Visualization.Diagrams;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AnimArch.Visualization.UI
{
    public abstract class AbstractPopUp : MonoBehaviour
    {
        public GameObject panel;

        protected void ChangeStateOfButtons(bool state)
        {
            DiagramPool.Instance.ClassDiagram.graph.GetComponentsInChildren<GraphicRaycaster>()
                .ForEach(x => x.enabled = state);
            var parent = transform.parent.parent;
            parent.Find("RightMenu").GetComponentsInChildren<Button>()
                .ForEach(x => x.enabled = state);
            parent.Find("TopMenu").GetComponentsInChildren<Button>()
                .ForEach(x => x.enabled = state);
            parent.Find("TopMenu").GetComponentsInChildren<EventTrigger>()
                .ForEach(x => x.enabled = state);
        }

        public virtual void ActivateCreation()
        {
            ChangeStateOfButtons(false);
            panel.SetActive(true);
        }

        public abstract void Confirmation();

        public virtual void Deactivate()
        {
            ChangeStateOfButtons(true);
            panel.SetActive(false);
        }
    }
}