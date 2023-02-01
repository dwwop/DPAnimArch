using System.Collections.Generic;
using UnityEngine;

namespace AnimArch.Visualization.Diagrams
{
    public class ParsedClassEditor
    {
        public static Class CreateNode(int id)
        {
            return new Class
            {
                Name = "NewClass_" + id,
                XmiId = id.ToString(),
                Type = "uml:Class",
                Attributes = new List<Attribute>(),
                Methods = new List<Method>()
            };
        }
        

        public static Class UpdateClassGeometry(Class newClass, GameObject classGo)
        {
            if (newClass.Left != 0)
                return newClass;
            var position = classGo.transform.localPosition;
            newClass.Left = position.x / 2.5f;
            newClass.Top = position.y / 2.5f;
            return newClass;
        }
        
        public  void UpdateNode()
        {
            throw new System.NotImplementedException();
        }

        public  void DeleteNode()
        {
            throw new System.NotImplementedException();
        }

        public  void AddEdge()
        {
            throw new System.NotImplementedException();
        }

        public  void DeleteEdge()
        {
            throw new System.NotImplementedException();
        }

        public  void AddMethod()
        {
            throw new System.NotImplementedException();
        }

        public  void UpdateMethod()
        {
            throw new System.NotImplementedException();
        }

        public  void DeleteMethod()
        {
            throw new System.NotImplementedException();
        }

        public  void AddAttribute()
        {
            throw new System.NotImplementedException();
        }

        public  void UpdateAttribute()
        {
            throw new System.NotImplementedException();
        }

        public  void DeleteAttribute()
        {
            throw new System.NotImplementedException();
        }

        public  void ClearDiagram()
        {
            throw new System.NotImplementedException();
        }
    }
}