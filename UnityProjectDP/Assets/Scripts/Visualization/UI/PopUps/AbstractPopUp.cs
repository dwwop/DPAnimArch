using System.Linq;
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

        private static void SetButtonsActive(bool active)
        {
            if (DiagramPool.Instance.ClassDiagram.graph != null)
                DiagramPool.Instance.ClassDiagram.graph.GetComponentsInChildren<GraphicRaycaster>()
                    .ForEach(x => x.enabled = active);

            var canvas = GameObject.Find("Canvas").transform;
            canvas.Find("RightMenu").GetComponentsInChildren<Button>()
                .Where(x => x.interactable)
                .ForEach(x => x.enabled = active);
            
            canvas.Find("TopMenu").GetComponentsInChildren<Button>()
                .Where(x => x.interactable)
                .ForEach(x => x.enabled = active);
            
            canvas.Find("TopMenu").GetComponentsInChildren<EventTrigger>()
                .ForEach(x => x.enabled = active);
            
            ToolManager.Instance.SetActive(active);
        }

        public void OnEnable()
        {
            SetButtonsActive(false);
        }

        public void OnDisable()
        {
            SetButtonsActive(true);
        }

        public virtual void ActivateCreation()
        {
            panel.SetActive(true);
        }

        public virtual void Confirmation()
        {
        }

        public virtual void Deactivate()
        {
            panel.SetActive(false);
        }
    }
}
