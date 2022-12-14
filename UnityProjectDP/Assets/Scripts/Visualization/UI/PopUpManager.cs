using UnityEngine;
using TMPro;
using AnimArch.Visualization.Diagrams;

namespace AnimArch.Visualization.UI
{
    public class PopUpManager : MonoBehaviour
    {
        public TMP_Text attributeTxt;
        public TMP_Text mtdTxt;
        public TMP_Text classTxt;

        public void OpenAttributePopUp()
        {
            ClassEditor.Instance.atrPopUp.ActivateCreation(classTxt, attributeTxt);
        }

        public void OpenMethodPopUp()
        {
            ClassEditor.Instance.mtdPopUp.ActivateCreation(classTxt, mtdTxt);
        }

        public void OpenClassPopUp()
        {
            ClassEditor.Instance.classPopUp.ActivateCreation(classTxt);
        }
    }
}