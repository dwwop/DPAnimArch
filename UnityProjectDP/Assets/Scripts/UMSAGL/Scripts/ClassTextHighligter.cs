using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Color = UnityEngine.Color;

namespace UMSAGL.Scripts
{
    public class ClassTextHighligter : MonoBehaviour
    {
        public LayoutGroup methodLayoutGroup;

        private TextMeshProUGUI GetLineText(string line)
        {
            return methodLayoutGroup.transform
                .GetComponentsInChildren<TextMeshProUGUI>()
                .First(x => x.text.Contains(line));
        }

        public void HighlightClassLine(string line)
        {
            GetLineText(line).color =
                AnimArch.Visualization.Animating.Animation.Instance.methodColor;
        }

        public void UnhighlightClassLine(string line)
        {
            GetLineText(line).color = Color.black;
        }
    }
}
