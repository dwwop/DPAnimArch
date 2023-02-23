using AnimArch.Visualization.Diagrams;
using TMPro;
using UnityEngine;

namespace AnimArch.Visualization.UI
{
    public class AttributeManager : MonoBehaviour
    {
        public TMP_Text classTxt;
        public TMP_Text attributeTxt;

        public void OpenAttributeEditPopUp()
        {
            UIEditorManager.Instance.attributePopUp.ActivateCreation(classTxt, attributeTxt);
        }

        public void DeleteAttribute()
        {
            UIEditorManager.Instance.confirmPopUp.ActivateCreation(delegate
            {
                UIEditorManager.Instance.mainEditor.DeleteAttribute(classTxt.text, name);
            });
        }
    }
}
