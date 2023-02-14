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

        protected void SetButtonsActive(bool active)
        {
            DiagramPool.Instance.ClassDiagram.graph.GetComponentsInChildren<GraphicRaycaster>()
                .ForEach(x => x.enabled = active);
            var parent = transform.parent.parent;
            parent.Find("RightMenu").GetComponentsInChildren<Button>()
                .ForEach(x => x.enabled = active);
            parent.Find("TopMenu").GetComponentsInChildren<Button>()
                .ForEach(x => x.enabled = active);
            parent.Find("TopMenu").GetComponentsInChildren<EventTrigger>()
                .ForEach(x => x.enabled = active);
        }

        public virtual void ActivateCreation()
        {
            SetButtonsActive(false);
            panel.SetActive(true);
        }

        public abstract void Confirmation();

        public virtual void Deactivate()
        {
            SetButtonsActive(true);
            panel.SetActive(false);
        }
    }
}
