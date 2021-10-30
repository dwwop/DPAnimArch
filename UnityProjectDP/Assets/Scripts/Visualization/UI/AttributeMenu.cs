using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using AnimArch.Visualization.ClassDiagram;

public class AttributeMenu : MonoBehaviour
{
    public GameObject AtrPanel;
    public TMP_InputField inp;
    public TMP_Dropdown dropdown;
    private TMP_Text atrTxt;
    private TMP_Text classTxt;
    private Attribute atr;
    public Toggle isArray;
    public void ActivateCreation(TMP_Text classTxt,TMP_Text atrTxt)
    {
        AtrPanel.SetActive(true);
        this.atrTxt = atrTxt;
        this.classTxt = classTxt;
        atr = new Attribute();

    }
    public void SetName(string atrName)
    {
        atr.Name = atrName;
    }
    public void SetType(string type)
    {
        atr.Type = type;
    }
    public void SaveAtr()
    {
        SetName(inp.text);
        SetType(dropdown.options[dropdown.value].text);
        if (Diagram.Instance.AddAtr(classTxt.text, atr))
        {
            if (isArray.isOn)
            {
                atrTxt.text += atr.Name + "[]: " + atr.Type + "\n";
            }
            else
            {
                atrTxt.text += atr.Name + ": " + atr.Type + "\n";
            }
        }
        atr = new Attribute();
        AtrPanel.SetActive(false);
        inp.text = "";
        isArray.isOn = false;
    }
}
