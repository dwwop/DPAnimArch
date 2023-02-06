using UnityEngine;
using TMPro;
using AnimArch.Visualization.Diagrams;

namespace AnimArch.Visualization.UI
{
    public class ClassPopUpManager : MonoBehaviour
    {
        public TMP_Text classTxt;

        public void OpenAttributePopUp()
        {
            UIEditorManager.Instance.attributePopUp.ActivateCreation(classTxt);
        }

        public void OpenMethodPopUp()
        {
            UIEditorManager.Instance.methodPopUp.ActivateCreation(classTxt);
        }

        public void OpenClassPopUp()
        {
            UIEditorManager.Instance.classPopUp.ActivateCreation(classTxt);
        }
    }
}