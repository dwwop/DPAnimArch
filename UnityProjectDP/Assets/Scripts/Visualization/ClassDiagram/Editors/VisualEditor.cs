using TMPro;
using UnityEngine;

namespace AnimArch.Visualization.Diagrams
{
    public class VisualEditor
    {
        public static GameObject CreateNode(Class newClass)
        {
            var node = DiagramPool.Instance.ClassDiagram.graph.AddNode();
            node.name = newClass.Name;

            SetDefaultPosition(node);
            UpdateClass(node);

            return node;
        }


        private static void SetDefaultPosition(GameObject node)
        {
            var rect = node.GetComponent<RectTransform>();
            rect.position = new Vector3(100f, 200f, 1);
        }

        private static void UpdateClass(GameObject classGo)
        {
            GetClassHeader(classGo).GetComponent<TextMeshProUGUI>().text = classGo.name;
        }

        private static Transform GetClassHeader(GameObject classGo)
        {
            return classGo.transform.Find("Background").Find("HeaderLayout").Find("Header");
        }
    }
}