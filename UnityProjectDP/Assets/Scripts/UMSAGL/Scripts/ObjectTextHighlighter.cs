using TMPro;
using UnityEngine;
using Animation = Visualization.Animation.Animation;

namespace UMSAGL.Scripts
{
    public class ObjectTextHighlighter : MonoBehaviour
    {
        string startColorTag = "<color=>";
        string endColorTag = "</color>";
        [SerializeField] private TMP_Text methodsText;
        string text;


        public void HighlightObjectLine(string line)
        {
            startColorTag = "<color=#" + Animation.Instance.GetColorCode("method") + ">";
            text = methodsText.text;
            Debug.Log(text);
            string startline = line.Replace(")", "");
            int start, end = 0;
            if (text.Contains(startline))
            {
                Debug.Log("Contains " + startline);
                start = text.IndexOf(startline, 0);
                string processedText = text.Substring(start);
                int i = 0;
                while (i < processedText.Length - 1 && processedText[i] != '\n')
                {
                    i++;
                }

                end = start + i + 1;
                //end = methodsText.text.Length - 2;
                if (end != -1)
                    text = text.Insert(end, endColorTag);
                if (start != -1)
                    text = text.Insert(start, startColorTag);
                methodsText.text = text;
            }
        }

        public void UnHighlightObjectLine(string line)
        {
            text = methodsText.text;
            text = text.Replace(startColorTag, "");
            text = text.Replace(endColorTag, "");
            methodsText.text = text;
        }
    }
}
