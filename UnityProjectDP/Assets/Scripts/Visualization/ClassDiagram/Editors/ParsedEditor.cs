using UnityEngine;
using Visualization.ClassDiagram.ClassComponents;
using Visualization.ClassDiagram.ComponentsInDiagram;

namespace Visualization.ClassDiagram.Editors
{
    public static class ParsedEditor
    {
        public static Class UpdateNodeGeometry(Class newClass, GameObject classGo)
        {
            var position = classGo.transform.localPosition;
            newClass.Left = position.x / 2.5f;
            newClass.Top = position.y / 2.5f;
            return newClass;
        }

        public static void ReverseNodesGeometry()
        {
            foreach (var parsedClass in DiagramPool.Instance.ClassDiagram.GetClassList()) parsedClass.Top *= -1;
        }

        public static void AddAttribute(ClassInDiagram classInDiagram, Attribute attribute)
        {
            classInDiagram.ParsedClass.Attributes.Add(attribute);
        }

        public static void UpdateAttribute(ClassInDiagram classInDiagram, string oldAttribute, Attribute newAttribute)
        {
            var index = classInDiagram.ParsedClass.Attributes.FindIndex(x => x.Name == oldAttribute);
            newAttribute.Id = classInDiagram.ParsedClass.Attributes[index].Id;
            classInDiagram.ParsedClass.Attributes[index] = newAttribute;
        }

        public static void AddMethod(ClassInDiagram classInDiagram, Method method)
        {
            classInDiagram.ParsedClass.Methods.Add(method);
        }

        public static void UpdateMethod(ClassInDiagram classInDiagram, string oldMethod, Method newMethod)
        {
            var index = classInDiagram.ParsedClass.Methods.FindIndex(x => x.Name == oldMethod);
            newMethod.Id = classInDiagram.ParsedClass.Methods[index].Id;
            classInDiagram.ParsedClass.Methods[index] = newMethod;
        }


        public static void DeleteAttribute(ClassInDiagram classInDiagram, string attribute)
        {
            classInDiagram.ParsedClass.Attributes.RemoveAll(x => x.Name == attribute);
        }

        public static void DeleteMethod(ClassInDiagram classInDiagram, string method)
        {
            classInDiagram.ParsedClass.Methods.RemoveAll(x => x.Name == method);
        }
    }
}
