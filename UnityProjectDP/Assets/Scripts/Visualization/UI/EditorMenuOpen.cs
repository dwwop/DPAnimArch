using UnityEngine;
using TMPro;
using AnimArch.Visualization.Diagrams;

namespace AnimArch.Visualization.UI
{
    public class EditorMenuOpen : MonoBehaviour
    {
        public TMP_Text attributeTxt;
        public TMP_Text mtdTxt;
        public TMP_Text classTxt;
        public void OpenAttributeMenu()
        {
            ClassEditor.Instance.atrPopUp.ActivateCreation(classTxt, attributeTxt);
        }
        public void OpenMethodMenu()
        {
            ClassEditor.Instance.mtdPopUp.ActivateCreation(classTxt, mtdTxt);
        }

        public void OpenClassMenu()
        {
            ClassEditor.Instance.classPopUp.ActivateCreation(classTxt);
        }
    }
}