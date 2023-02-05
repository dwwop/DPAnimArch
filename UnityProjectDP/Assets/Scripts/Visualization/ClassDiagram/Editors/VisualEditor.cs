using AnimArch.Extensions;
using AnimArch.Visualization.UI;
using Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnimArch.Visualization.Diagrams
{
    public static class VisualEditor
    {
        static VisualEditor()
        {
        }

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
        
        
        // called at manual layout
        public static void SetPosition(string className, Vector3 position, bool fromRpc)
        {
            if (!fromRpc)
                Spawner.Instance.SetPosition(className, position);
            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(className);
            if (classInDiagram != null)
            {
                classInDiagram
                    .VisualObject
                    .GetComponent<RectTransform>()
                    .position = position;
            }
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

        private static string GetStringFromMethod(Method method)
        {
            var arguments = "(";
            if (method.arguments != null)
            {
                for (var index = 0; index < method.arguments.Count; index++)
                {
                    if (index < method.arguments.Count - 1)
                        arguments += (method.arguments[index] + ", ");
                    else arguments += (method.arguments[index]);
                }
            }

            arguments += "): ";

            return method.Name + arguments + method.ReturnValue;
        }

        public static void AddMethod(ClassInDiagram classInDiagram, Method method)
        {
            var methodLayoutGroup = GetMethodLayoutGroup(classInDiagram.VisualObject);
            var instance = Object.Instantiate(DiagramPool.Instance.classMethodPrefab, methodLayoutGroup, false);

            instance.name = method.Name;
            instance.transform.Find("MethodText").GetComponent<TextMeshProUGUI>().text += GetStringFromMethod(method);
            instance.GetComponent<MethodPopUpManager>().classTxt =
                GetNodeHeader(classInDiagram.VisualObject).GetComponent<TextMeshProUGUI>();

            if (UIEditorManager.Instance.active)
                instance.GetComponentsInChildren<Button>(includeInactive: true)
                    .ForEach(x => x.gameObject.SetActive(true));
        }

        public static void UpdateMethod(ClassInDiagram classInDiagram, string oldMethod, Method newMethod)
        {
            var method = GetMethodLayoutGroup(classInDiagram.VisualObject).Find(oldMethod);

            method.name = newMethod.Name;
            method.Find("MethodText").GetComponent<TextMeshProUGUI>().text = GetStringFromMethod(newMethod);
        }

        private static string GetStringFromAttribute(Attribute attribute)
        {
            return attribute.Name + ": " + attribute.Type;
        }

        public static void AddAttribute(ClassInDiagram classInDiagram, Attribute attribute)
        {
            var attributeLayoutGroup = GetAttributeLayoutGroup(classInDiagram.VisualObject);
            var instance = Object.Instantiate(DiagramPool.Instance.classAttributePrefab, attributeLayoutGroup, false);

            instance.name = attribute.Name;
            instance.transform.Find("AttributeText").GetComponent<TextMeshProUGUI>().text +=
                GetStringFromAttribute(attribute);
            instance.GetComponent<AttributePopUpManager>().classTxt =
                GetNodeHeader(classInDiagram.VisualObject).GetComponent<TextMeshProUGUI>();

            if (UIEditorManager.Instance.active)
                instance.GetComponentsInChildren<Button>(includeInactive: true)
                    .ForEach(x => x.gameObject.SetActive(true));
        }


        public static void UpdateAttribute(ClassInDiagram classInDiagram, string oldAttribute, Attribute newAttribute)
        {
            var attribute = GetAttributeLayoutGroup(classInDiagram.VisualObject).Find(oldAttribute);

            attribute.name = newAttribute.Name;
            attribute.Find("AttributeText").GetComponent<TextMeshProUGUI>().text = GetStringFromAttribute(newAttribute);
        }
    }
}