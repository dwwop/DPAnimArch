using AnimArch.Visualization.Diagrams;
using TMPro;
using UnityEngine;

namespace AnimArch.Visualization.UI
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
            MainEditor.DeleteMethod(classTxt.text, name);
        }
    }
}
