using TMPro;
using UnityEngine;

namespace AnimArch.Visualization.UI
{
    public class ParameterManager : MonoBehaviour
    {
        public TMP_Text parameterTxt;

        public void OpenParameterEditPopUp()
        {
            UIEditorManager.Instance.methodPopUp.panel.SetActive(false);
            UIEditorManager.Instance.parameterPopUp.ActivateCreation(parameterTxt);
        }

        public void DeleteParameter()
        {
            UIEditorManager.Instance.methodPopUp.RemoveArg(name);
        }
    }
}
