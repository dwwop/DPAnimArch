using OALProgramControl;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CallStackPanel : MonoBehaviour
{
    public List<GameObject> CallStackItems;

    private void Awake()
    {
        foreach (GameObject CallStackItem in CallStackItems)
        {
            CallStackItem.SetActive(false);
        }
    }

    public void Refresh(List<MethodCallRecord> MethodCalls)
    {
        foreach (GameObject CallStackItem in CallStackItems)
        {
            CallStackItem.SetActive(false);
        }

        if (MethodCalls.Count > CallStackItems.Count)
        {
            MethodCalls = MethodCalls.GetRange(MethodCalls.Count - CallStackItems.Count, CallStackItems.Count);
        }

        CallStackItem callStackItem;
        for (int i = 0; i < MethodCalls.Count; i++)
        {
            callStackItem = CallStackItems[i].GetComponent<CallStackItem>();
            CallStackItems[i].SetActive(true);
            callStackItem.Label.GetComponent<TMP_Text>().SetText(MethodCalls[i].ClassName + "::" + MethodCalls[i].MethodName + "()");
        }
    }
}
