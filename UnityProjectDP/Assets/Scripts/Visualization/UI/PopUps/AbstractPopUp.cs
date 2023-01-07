using AnimArch.Visualization.Diagrams;
using TMPro;
using UnityEngine;

namespace AnimArch.Visualization.UI
{
    public abstract class AbstractPopUp : MonoBehaviour
    {
        public GameObject panel;
        public TMP_InputField inp;
        protected TMP_Text className;

        public virtual void ActivateCreation(TMP_Text classTxt)
        {
            panel.SetActive(true);
            className = classTxt;
        }

        public abstract void Confirmation();
        
        public virtual void Deactivate()
        {
            panel.SetActive(false);
            inp.text = "";
        }
    }
}