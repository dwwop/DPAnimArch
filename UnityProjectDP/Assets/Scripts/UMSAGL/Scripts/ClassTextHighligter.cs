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

        public void HighlightClassLine(string line)
        {
            methodLayoutGroup.transform
                    .GetComponentsInChildren<TextMeshProUGUI>()
                    .First(x => x.text.Contains(line))
                    .color =
                AnimArch.Visualization.Animating.Animation.Instance.methodColor;
        }

        public void UnhighlightClassLine(string line)
        {
            methodLayoutGroup.transform
                .GetComponentsInChildren<TextMeshProUGUI>()
                .First(x => x.text.Contains(line))
                .color = Color.black;
        }
    }
}