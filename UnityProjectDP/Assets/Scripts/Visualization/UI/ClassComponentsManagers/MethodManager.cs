using TMPro;
using UnityEngine;

namespace Visualization.UI.ClassComponentsManagers
{
    public class MethodManager : MonoBehaviour
    {
        public TMP_Text classTxt;
        public TMP_Text methodTxt;

        public void OpenMethodEditPopUp()
        {
            UIEditorManager.Instance.methodPopUp.ActivateCreation(classTxt, methodTxt);
        }

        public void DeleteMethod()
        {
            UIEditorManager.Instance.confirmPopUp.ActivateCreation(delegate
            {
                UIEditorManager.Instance.mainEditor.DeleteMethod(classTxt.text, name);
            });
        }
    }
}
