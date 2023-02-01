using AnimArch.Visualization.Diagrams;
using TMPro;
using UnityEngine;

namespace AnimArch.Visualization.UI
{
    public class ParameterPopUpManager : MonoBehaviour
    {
        public TMP_Text parameterTxt;

        public void OpenParameterEditPopUp()
        {
            UIEditorManager.Instance.methodPopUp.panel.SetActive(false);
            UIEditorManager.Instance.parameterPopUp.ActivateCreation(parameterTxt);
        }
    }
}
