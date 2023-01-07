using AnimArch.Visualization.Diagrams;
using TMPro;
using UnityEngine;

namespace AnimArch.Visualization.UI
{
    public class MethodPopUpManager : MonoBehaviour
    {
        public TMP_Text classTxt;
        public TMP_Text methodTxt;
        
        public void OpenMethodEditPopUp()
        {
            ClassEditor.Instance.mtdPopUp.ActivateCreation(classTxt, methodTxt);
        }
    }
}