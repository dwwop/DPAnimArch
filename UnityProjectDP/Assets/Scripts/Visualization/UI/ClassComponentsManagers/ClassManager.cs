using TMPro;
using UnityEngine;

namespace Visualization.UI.ClassComponentsManagers
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
            UIEditorManager.Instance.confirmPopUp.ActivateCreation(delegate
            {
                UIEditorManager.Instance.mainEditor.DeleteNode(name);
            });
        }
    }
}
