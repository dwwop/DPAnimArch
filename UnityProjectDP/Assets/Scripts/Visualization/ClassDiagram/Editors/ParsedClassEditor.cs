using System.Collections.Generic;
using UnityEngine;

namespace AnimArch.Visualization.Diagrams
{
    public static class ParsedClassEditor
    {
        
        
        public static Class CreateNode(string name, string id)
        {
            return new Class
            {
                Name = name,
                XmiId = id,
                Type = "uml:Class",
                Attributes = new List<Attribute>(),
                Methods = new List<Method>()
            };
        }
        
        public static Class CreateNode(int id)
        {
            return CreateNode("NewClass_", id.ToString());
        }
        

        public static Class UpdateNodeGeometry(Class newClass, GameObject classGo)
        {
            if (newClass.Left != 0)
                return newClass;
            var position = classGo.transform.localPosition;
            newClass.Left = position.x / 2.5f;
            newClass.Top = position.y / 2.5f;
            return newClass;
        }
        
        public static void ReverseNodesGeometry()
        {
            foreach (var parsedClass in DiagramPool.Instance.ClassDiagram.GetClassList())
            {
                parsedClass.Top *= -1;
            }
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
        public static void AddMethod(ClassInDiagram classInDiagram, Method method){
            classInDiagram.ParsedClass.Methods.Add(method);
        }

        public static void UpdateMethod(ClassInDiagram classInDiagram, string oldMethod, Method newMethod)
        {
            var index = classInDiagram.ParsedClass.Methods.FindIndex(x => x.Name == oldMethod);
            newMethod.Id = classInDiagram.ParsedClass.Methods[index].Id;
            classInDiagram.ParsedClass.Methods[index] = newMethod;
        }
        
        // public  void UpdateNode()
        // {
        //     throw new System.NotImplementedException();
        // }
        //
        // public  void DeleteNode()
        // {
        //     throw new System.NotImplementedException();
        // }
        //
        // public  void AddEdge()
        // {
        //     throw new System.NotImplementedException();
        // }
        //
        // public  void DeleteEdge()
        // {
        //     throw new System.NotImplementedException();
        // }
        //
        // public  void AddMethod()
        // {
        //     throw new System.NotImplementedException();
        // }
        //
        // public  void UpdateMethod()
        // {
        //     throw new System.NotImplementedException();
        // }
        //
        // public  void DeleteMethod()
        // {
        //     throw new System.NotImplementedException();
        // }
        //
        // public  void AddAttribute()
        // {
        //     throw new System.NotImplementedException();
        // }
        //
        // public  void UpdateAttribute()
        // {
        //     throw new System.NotImplementedException();
        // }
        //
        // public  void DeleteAttribute()
        // {
        //     throw new System.NotImplementedException();
        // }
        //
        // public  void ClearDiagram()
        // {
        //     throw new System.NotImplementedException();
        // }
    }
}