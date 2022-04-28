using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PanelSourceCodeAnimation : MonoBehaviour
{
    public GameObject LabelClassMethodName;
    public GameObject TextAreaSourceCode;

    public void SetMethodLabelText(string className, string methodName)
    {
        string text = string.Format("{0}::{1}", className, methodName);
        LabelClassMethodName.GetComponent<TextMeshProUGUI>().SetText(text);
    }

    public void SetSourceCodeText(string text)
    {
        TextAreaSourceCode.GetComponent<TMP_InputField>().text = text;
    }
}
