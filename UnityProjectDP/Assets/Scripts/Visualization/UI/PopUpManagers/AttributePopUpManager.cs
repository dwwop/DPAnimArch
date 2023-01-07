using System;
using AnimArch.Visualization.Diagrams;
using TMPro;
using UnityEngine;

namespace AnimArch.Visualization.UI
{
    public class AttributePopUpManager : MonoBehaviour
    {
        public TMP_Text classTxt;
        public TMP_Text attributeTxt;
        
        
        public void OpenAttributeEditPopUp()
        {
            ClassEditor.Instance.atrPopUp.ActivateCreation(classTxt, attributeTxt);
        }
    }
}