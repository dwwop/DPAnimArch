using TMPro;
using UnityEngine;

namespace Visualization.UI.ClassComponentsManagers
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
            UIEditorManager.Instance.methodPopUp.panel.SetActive(false);
            UIEditorManager.Instance.confirmPopUp.ActivateCreation(delegate
            {
                UIEditorManager.Instance.methodPopUp.RemoveArg(name);
                UIEditorManager.Instance.methodPopUp.panel.SetActive(true);
            });
            UIEditorManager.Instance.confirmPopUp.cancelButton.onClick.AddListener(delegate
            {
                UIEditorManager.Instance.methodPopUp.panel.SetActive(true);
            });
            
            UIEditorManager.Instance.confirmPopUp.exitButton.onClick.AddListener(delegate
            {
                UIEditorManager.Instance.methodPopUp.panel.SetActive(true);
            });
        }
    }
}
