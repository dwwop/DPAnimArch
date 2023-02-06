using TMPro;
using UnityEngine;

namespace AnimArch.Visualization.UI
{
    public class SelectionPopUp : MonoBehaviour
    {
        public GameObject panel;
        public TMP_Dropdown dropdown;

        public void ActivateCreation()
        {
            panel.SetActive(true);
        }

        public void Confirmation()
        {
            UIEditorManager.Instance.StartSelection(dropdown.options[dropdown.value].text);
        }

        public void Deactivate()
        {
            panel.SetActive(false);
        }
    }
}
