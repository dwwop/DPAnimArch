using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using AnimArch.Extensions;

public class ConsolePanel : Singleton<ConsolePanel>
{
    public GameObject inputField;
    public GameObject outputField;
    private TMP_InputField tmpInpField;
    private TMP_InputField tmpOutpField;
    private bool canWrite = false;

    private void Awake()
    {
        tmpInpField = inputField.GetComponent<TMP_InputField>();
        tmpOutpField = outputField.GetComponent<TMP_InputField>();
    }

    public void YieldOutput(string output) 
    {
        tmpOutpField.text += output + "\n";
    }

    public void ActivateInputField()
    {
        canWrite = true;
        tmpInpField.interactable =  true;
    }

    public void InputEntered()
    {
        if (canWrite && (tmpInpField.text.Length > 0) && "\n".Equals(tmpInpField.text.Last()))
        {
            canWrite = false;
            tmpInpField.interactable = false;

            tmpOutpField.text += tmpInpField.text;

            Animation.Instance.ReadValue = tmpInpField.text.Substring(0, tmpInpField.text.Length - 1);
            tmpInpField.text = "";

            Animation.Instance.IncrementBarrier();
        }
    }
}
