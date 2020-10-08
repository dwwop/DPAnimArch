using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextHighlighter : MonoBehaviour
{
    string startColorTag= "<color=>";
    string endColorTag = "</color>";
    [SerializeField]
    private TMP_Text methodsText;
    string text;

    public void HighlightLine(string line)
    {
        startColorTag = "<color=#" + Animation.Instance.GetColorCode("method")+">";
        //TODO REWORK
        text=methodsText.text;
        Debug.Log(text);
        string startline = line.Replace(")", "");
        int start, end=0;
        if (text.Contains(startline))
        {
            Debug.Log("Contains " + startline);
            start = text.IndexOf(startline, 0);
            string processedText = text.Substring(start);
            int i = 0;
            while (i < processedText.Length - 1&&processedText[i]!='\n')
            {
                i++;
            }
            end = start + i+1;
            //end = methodsText.text.Length - 2;
            if (end != -1)
                text=text.Insert(end, endColorTag);
            if (start != -1)
                text=text.Insert(start, startColorTag);
            methodsText.text = text;
        }
    }
    public void UnHighlightLine(string line)
    {
        //TODO REWROK
        text= methodsText.text;
        text=text.Replace(startColorTag, "");
        text=text.Replace(endColorTag, "");
        methodsText.text = text;
    }

}

