﻿using System.Linq;
using AnimArch.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Visualization.ClassDiagram;

namespace Visualization.UI.PopUps
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

        public virtual void OnEnable()
        {
            SetButtonsActive(false);
        }

        public virtual void OnDisable()
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
