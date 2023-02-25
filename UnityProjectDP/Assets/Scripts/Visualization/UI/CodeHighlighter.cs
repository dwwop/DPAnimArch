using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Visualization.UI
{
    public class CodeHighlighter : MonoBehaviour
    {
        TMP_InputField targetText;
        Dictionary<string, string> keyWords;
        public static List<String> Keywords = new List<String>(new String[]
            {
                 "create", "object", "instance", "of",
                "relate", "to", "across",
                "select", "any", "many", "from", "instances", "where", "related", "by",
                "delete", "unrelate",
                "call",
                "if", "elif", "else", "end",
                "par", "thread",
                "while",
                "for", "each", "in",
                "break", "continue",
                "and", "or", "not",
                "assign",
                "false", "true",
                "cardinality", "empty", "not_empty"
            });
        void Awake()
        {
            string Colour = "#6a2ac9";
            keyWords = new Dictionary<string, string>
            {
                /*{ "call", Colour},
                { "while", Colour},
                { "for", Colour},
                { "from", Colour},
                { "across", Colour},
                { "create", Colour},
                { "object", Colour},
                { "instance", Colour},
                { "of", Colour},
                { "to", Colour}*/
            };

            foreach (string KeyWord in Keywords)
            {
                keyWords.Add(KeyWord, "#6a2ac9");
            }

            targetText = GetComponent<TMP_InputField>();
        }
        public void UpdateColors()
        {
            if (targetText != null)
            {
                int i;
                int offset = "<color=#FF0000>".Length + "</color>".Length;
                foreach (KeyValuePair<string, string> pair in keyWords)
                {

                    i = 0;
                    string searchedWord = pair.Key;
                    foreach (Match m in Regex.Matches(targetText.text, searchedWord))
                    {
                        string startColorTag = "<color=" + pair.Value + ">";
                        string endColorTag = "</color>";
                        int startIndex = m.Index;
                        int endIndex = m.Index + searchedWord.Length;
                        if(startIndex!=0 && (endIndex+i*offset)<targetText.text.Length)
                        {
                            if (!char.IsLetter(targetText.text[startIndex + i * offset - 1]) && !char.IsLetter(targetText.text[endIndex + i * offset]))
                            {
                            
                                if (targetText.text.Length > endIndex)
                                    targetText.text = targetText.text.Insert(endIndex + i * offset, endColorTag);
                                else
                                    targetText.text += endColorTag;
                                targetText.text = targetText.text.Insert(startIndex + i * offset, startColorTag);
                                i++;
                            }
                        }
                        else
                        {
                            if (targetText.text.Length > endIndex)
                                targetText.text = targetText.text.Insert(endIndex + i * offset, endColorTag);
                            else
                                targetText.text += endColorTag;
                            targetText.text = targetText.text.Insert(startIndex + i * offset, startColorTag);
                            i++;
                        }
                    }
                }
            }
        }
        struct Marker
        {
            public int start;
            public int end;
        }
        public void RemoveColors()
        {
            if (targetText==null)
            {
                return;
            }
            Debug.Log("removing colors");
            List<Marker> markers = new List<Marker>();
            Marker currentMarker=new Marker();
            int index = 0;
            /*foreach( char c in targetText.text)
            {
                if (c.Equals('<'))
                {
                    Debug.Log("st_index= " + index);
                    currentMarker.start = index;
                }
                if (c.Equals('>'))
                {
                    currentMarker.end = index;
                    markers.Add(currentMarker);
                    Debug.Log("end_index= " + index);
                    currentMarker = new Marker();
                }
                index++;
            }*/
            foreach (Match m in Regex.Matches(targetText.text, "<color="))
            {
                currentMarker = new Marker();
                index = m.Index;
                currentMarker.start = index;
                char c='r';
                int endIndex = index+1;
                while (c != '>' && endIndex < targetText.text.Length)
                {
                    c = targetText.text[endIndex];
                    endIndex++;
                }
                if (c == '>')
                {
                    currentMarker.end = endIndex;
                    markers.Add(currentMarker);
                }

            }
            markers.Reverse();
            foreach(Marker m in markers)
            {
                targetText.text = targetText.text.Remove(m.start, m.end - m.start);
            }

            markers = new List<Marker>();
            foreach (Match m in Regex.Matches(targetText.text, "</color>"))
            {
                currentMarker = new Marker();
                index = m.Index;
                currentMarker.start = index;
                currentMarker.end = index + 8;
                markers.Add(currentMarker);
            }
            markers.Reverse();
            foreach (Marker m in markers)
            {
                targetText.text = targetText.text.Remove(m.start, m.end - m.start);
            }

        }
    }
}