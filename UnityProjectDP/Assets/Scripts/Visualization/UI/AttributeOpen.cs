using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AttributeOpen : MonoBehaviour
{
    public TMP_Text attributeTxt;
    public TMP_Text mtdTxt;
    public TMP_Text classTxt;
    public void OpenAttributeMenu()
    {
        ClassEditor.Instance.atrMenu.ActivateCreation(classTxt, attributeTxt);
    }
    public void OpenMethodMenu()
    {
        ClassEditor.Instance.mtdMenu.ActivateCreation(classTxt, mtdTxt);
    }
}
