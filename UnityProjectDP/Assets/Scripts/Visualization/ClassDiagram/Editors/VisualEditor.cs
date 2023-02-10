using System.Collections;
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


        private static void SetDefaultPosition(GameObject node)
        {
            var rect = node.GetComponent<RectTransform>();
            rect.position = new Vector3(100f, 200f, 1);
        }


        private static void UpdateNodeName(GameObject classGo)
        {
            GetNodeHeader(classGo).GetComponent<TextMeshProUGUI>().text = classGo.name;
        }


        public static GameObject CreateNode(Class newClass)
        {
            var node = DiagramPool.Instance.ClassDiagram.graph.AddNode();
            node.name = newClass.Name;

            SetDefaultPosition(node);
            UpdateNodeName(node);

            return node;
        }


        public static void SetPosition(string className, Vector3 position, bool fromRpc)
        {
            if (!fromRpc)
                Spawner.Instance.SetPosition(className, position);
            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(className);
            if (classInDiagram != null)
                classInDiagram
                    .VisualObject
                    .GetComponent<RectTransform>()
                    .position = position;
        }


        public static void UpdateNode(GameObject classGo)
        {
            UpdateNodeName(classGo);

            foreach (var attribute in GetAttributeLayoutGroup(classGo).GetComponents<AttributeManager>())
                attribute.classTxt = GetNodeHeader(classGo).GetComponent<TextMeshProUGUI>();

            foreach (var method in GetMethodLayoutGroup(classGo).GetComponents<MethodManager>())
                method.classTxt = GetNodeHeader(classGo).GetComponent<TextMeshProUGUI>();
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
            instance.GetComponent<AttributeManager>().classTxt =
                GetNodeHeader(classInDiagram.VisualObject).GetComponent<TextMeshProUGUI>();

            if (UIEditorManager.Instance.active)
                instance.GetComponentsInChildren<Button>(true)
                    .ForEach(x => x.gameObject.SetActive(true));
        }


        public static void UpdateAttribute(ClassInDiagram classInDiagram, string oldAttribute, Attribute newAttribute)
        {
            var attribute = GetAttributeLayoutGroup(classInDiagram.VisualObject).Find(oldAttribute);

            attribute.name = newAttribute.Name;
            attribute.Find("AttributeText").GetComponent<TextMeshProUGUI>().text = GetStringFromAttribute(newAttribute);
        }


        private static string GetStringFromMethod(Method method)
        {
            var arguments = "(";
            if (method.arguments != null)
                for (var index = 0; index < method.arguments.Count; index++)
                    if (index < method.arguments.Count - 1)
                        arguments += method.arguments[index] + ", ";
                    else arguments += method.arguments[index];

            arguments += "): ";

            return method.Name + arguments + method.ReturnValue;
        }


        public static void AddMethod(ClassInDiagram classInDiagram, Method method)
        {
            var methodLayoutGroup = GetMethodLayoutGroup(classInDiagram.VisualObject);
            var instance = Object.Instantiate(DiagramPool.Instance.classMethodPrefab, methodLayoutGroup, false);

            instance.name = method.Name;
            instance.transform.Find("MethodText").GetComponent<TextMeshProUGUI>().text += GetStringFromMethod(method);
            instance.GetComponent<MethodManager>().classTxt =
                GetNodeHeader(classInDiagram.VisualObject).GetComponent<TextMeshProUGUI>();

            if (UIEditorManager.Instance.active)
                instance.GetComponentsInChildren<Button>(true)
                    .ForEach(x => x.gameObject.SetActive(true));
        }


        public static void UpdateMethod(ClassInDiagram classInDiagram, string oldMethod, Method newMethod)
        {
            var method = GetMethodLayoutGroup(classInDiagram.VisualObject).Find(oldMethod);

            method.name = newMethod.Name;
            method.Find("MethodText").GetComponent<TextMeshProUGUI>().text = GetStringFromMethod(newMethod);
        }


        //Fix used to minimize relation displaying bug
        private static IEnumerator QuickFix(GameObject g)
        {
            yield return new WaitForSeconds(0.05f);
            g.SetActive(false);
            yield return new WaitForSeconds(0.05f);
            g.SetActive(true);
        }

        public static GameObject CreateRelation(Relation relation)
        {
            var prefab = relation.PropertiesEaType switch
            {
                "Association" => relation.PropertiesDirection switch
                {
                    "Source -> Destination" => DiagramPool.Instance.associationSDPrefab,
                    "Destination -> Source" => DiagramPool.Instance.associationDSPrefab,
                    "Bi-Directional" => DiagramPool.Instance.associationFullPrefab,
                    _ => DiagramPool.Instance.associationNonePrefab
                },
                "Generalization" => DiagramPool.Instance.generalizationPrefab,
                "Dependency" => DiagramPool.Instance.dependsPrefab,
                "Realisation" => DiagramPool.Instance.realisationPrefab,
                _ => DiagramPool.Instance.associationNonePrefab
            };

            var sourceClassGo = DiagramPool.Instance.ClassDiagram.FindClassByName(relation.FromClass).VisualObject;
            var destinationClassGo = DiagramPool.Instance.ClassDiagram.FindClassByName(relation.ToClass).VisualObject;

            var edge = DiagramPool.Instance.ClassDiagram.graph.AddEdge(sourceClassGo, destinationClassGo, prefab);

            if (edge.gameObject.transform.childCount > 0)
                DiagramPool.Instance.ClassDiagram.StartCoroutine(QuickFix(edge.transform.GetChild(0).gameObject));

            return edge;
        }

        public static void DeleteRelation(RelationInDiagram relationInDiagram)
        {
            DiagramPool.Instance.ClassDiagram.graph.RemoveEdge(relationInDiagram.VisualObject);
        }

        public static void DeleteNode(ClassInDiagram classInDiagram)
        {
            DiagramPool.Instance.ClassDiagram.graph.RemoveNode(classInDiagram.VisualObject);
        }
        
        public static void DeleteAttribute(ClassInDiagram classInDiagram, string attribute)
        {
            Object.Destroy(GetAttributeLayoutGroup(classInDiagram.VisualObject).Find(attribute).transform.gameObject);
        }


        public static void DeleteMethod(ClassInDiagram classInDiagram, string method)
        {
            Object.Destroy(GetMethodLayoutGroup(classInDiagram.VisualObject).Find(method).transform.gameObject);
        }
    }
}
