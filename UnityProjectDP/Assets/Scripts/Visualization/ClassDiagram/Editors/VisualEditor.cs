using AnimArch.Visualization.UI;
using TMPro;
using UnityEngine;

namespace AnimArch.Visualization.Diagrams
{
    public static class VisualEditor
    {
        public static GameObject CreateNode(Class newClass)
        {
            var node = DiagramPool.Instance.ClassDiagram.graph.AddNode();
            node.name = newClass.Name;

            SetDefaultPosition(node);
            UpdateNodeName(node);

            return node;
        }

        private static void SetDefaultPosition(GameObject node)
        {
            var rect = node.GetComponent<RectTransform>();
            rect.position = new Vector3(100f, 200f, 1);
        }

        private static void UpdateNodeName(GameObject classGo)
        {
            GetNodeHeader(classGo).GetComponent<TextMeshProUGUI>().text = classGo.name;
        }

        private static Transform GetNodeHeader(GameObject classGo)
        {
            return classGo.transform.Find("Background").Find("HeaderLayout").Find("Header");
        }

        private static Transform GetAttributeLayoutGroup(GameObject classGo)
        {
            return classGo.transform
                .Find("Background")
                .Find("Attributes")
                .Find("AttributeLayoutGroup");
        }


        private static Transform GetMethodLayoutGroup(GameObject classGo)
        {
            return classGo.transform
                .Find("Background")
                .Find("Methods")
                .Find("MethodLayoutGroup");
        }


        public static void UpdateNode(GameObject classGo)
        {
            UpdateNodeName(classGo);

            foreach (var attribute in GetAttributeLayoutGroup(classGo).GetComponents<AttributePopUpManager>())
            {
                attribute.classTxt = GetNodeHeader(classGo).GetComponent<TextMeshProUGUI>();
            }

            foreach (var method in GetMethodLayoutGroup(classGo).GetComponents<MethodPopUpManager>())
            {
                method.classTxt = GetNodeHeader(classGo).GetComponent<TextMeshProUGUI>();
            }
        }
    }
}