using AnimArch.Visualization.Diagrams;
using TMPro;
using UnityEngine;

namespace AnimArch.Visualization.UI
{
    public class ClassManager : MonoBehaviour
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

        public void DeleteClass()
        {
            MainEditor.DeleteNode(name);
        }
    }
}
