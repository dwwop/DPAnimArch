using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConsolePanel : MonoBehaviour
{
    public GameObject inputField;
    public GameObject outputField;

    public void YieldOutput(string output) 
    {
        TMP_InputField tmpInpField =  outputField.GetComponent<TMP_InputField>();
        tmpInpField.text += output + "\n";
    }
}
