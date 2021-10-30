using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using AnimArch.Visualization.ClassDiagram;
public class MethodMenu : MonoBehaviour
{
    public GameObject MtdPanel;
    public TMP_InputField inp;
    public TMP_Dropdown dropdown;
    private TMP_Text mtdTxt;
    private TMP_Text classTxt;
    private Method mtd;
    public void ActivateCreation(TMP_Text classTxt, TMP_Text mtdTxt)
    {
        MtdPanel.SetActive(true);
        this.mtdTxt = mtdTxt;
        this.classTxt = classTxt;
        mtd = new Method();

    }
    public void SetName(string atrName)
    {
        mtd.Name = atrName;
    }
    public void SetType(string type)
    {
        mtd.ReturnValue = type;
    }
    public void SaveMtd()
    {
        SetName(inp.text);
        SetType(dropdown.options[dropdown.value].text);
        if (Diagram.Instance.AddMethod(classTxt.text, mtd))
        {
            mtdTxt.text += mtd.Name + "() :" + mtd.ReturnValue + "\n";
        }

        mtd = new Method();
        MtdPanel.SetActive(false);
        inp.text = "";
    }
}
