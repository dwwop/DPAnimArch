using System.Collections.Generic;
using Networking;

namespace AnimArch.Visualization.Diagrams
{
    public static class MainEditor
    {
        public enum Source
        {
            RPC,
            Editor,
            Loader
        }

        private static void CreateNode(Class newClass)
        {
            var newCdClass = CDClassEditor.CreateNode(newClass);
            newClass.Name = newCdClass.Name;

            var classGo = VisualEditor.CreateNode(newClass);

            newClass = ParsedClassEditor.UpdateNodeGeometry(newClass, classGo);

            var classInDiagram = new ClassInDiagram
                { ParsedClass = newClass, ClassInfo = newCdClass, VisualObject = classGo };
            DiagramPool.Instance.ClassDiagram.Classes.Add(classInDiagram);
        }

        public static void CreateNode(Class newClass, Source source)
        {
            newClass.Name = newClass.Name.Replace(" ", "_");
            switch (source)
            {
                case Source.Editor:
                    CreateNode(newClass);
                    Spawner.Instance.SpawnNode(newClass.Name, newClass.XmiId);
                    break;
                case Source.RPC:
                    CreateNode(newClass);
                    break;
                case Source.Loader:
                    Spawner.Instance.SpawnNode(newClass.Name, newClass.XmiId);
                    CreateNode(newClass);
                    break;
            }
        }

        public static void UpdateNodeName(string oldName, string newName, bool fromRpc)
        {
            if (!fromRpc)
                Spawner.Instance.SetNodeName(oldName, newName);

            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(oldName);
            if (classInDiagram == null)
                return;

            classInDiagram.ParsedClass.Name = newName;
            classInDiagram.ClassInfo.Name = newName;
            classInDiagram.VisualObject.name = newName;

            VisualEditor.UpdateNode(classInDiagram.VisualObject);

            RelationshipEditor.UpdateNodeName(oldName, newName);
        }

        private static void AddAttribute(string targetClass, Attribute attribute)
        {
            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(targetClass);
            if (classInDiagram == null) return;

            if (DiagramPool.Instance.ClassDiagram.FindAttributeByName(targetClass, attribute.Name) != null)
                return;

            attribute.Id = (classInDiagram.ParsedClass.Attributes.Count + 1).ToString();

            ParsedClassEditor.AddAttribute(classInDiagram, attribute);
            CDClassEditor.AddAttribute(classInDiagram, attribute);
            VisualEditor.AddAttribute(classInDiagram, attribute);
        }

        public static void AddAttribute(string targetClass, Attribute attribute, Source source)
        {
            attribute.Name = attribute.Name.Replace(" ", "_");
            switch (source)
            {
                case Source.Editor:
                    AddAttribute(targetClass, attribute);
                    Spawner.Instance.AddAttribute(targetClass, attribute.Name, attribute.Type);
                    break;
                case Source.RPC:
                    AddAttribute(targetClass, attribute);
                    break;
                case Source.Loader:
                    Spawner.Instance.AddAttribute(targetClass, attribute.Name, attribute.Type);
                    var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(targetClass);

                    CDClassEditor.AddAttribute(classInDiagram, attribute);
                    VisualEditor.AddAttribute(classInDiagram, attribute);
                    break;
            }
        }

        public static void UpdateAttribute(string targetClass, string oldAttribute, Attribute newAttribute)
        {
            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(targetClass);
            if (classInDiagram == null)
                return;

            if (DiagramPool.Instance.ClassDiagram.FindAttributeByName(targetClass, newAttribute.Name) != null)
                return;

            ParsedClassEditor.UpdateAttribute(classInDiagram, oldAttribute, newAttribute);
            CDClassEditor.UpdateAttribute(classInDiagram, oldAttribute, newAttribute);
            VisualEditor.UpdateAttribute(classInDiagram, oldAttribute, newAttribute);
        }

        private static void AddMethod(string targetClass, Method method)
        {
            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(targetClass);
            if (classInDiagram == null)
                return;

            classInDiagram.ParsedClass.Methods ??= new List<Method>();

            if (DiagramPool.Instance.ClassDiagram.FindMethodByName(targetClass, method.Name) != null)
                return;

            method.Id = (classInDiagram.ParsedClass.Methods.Count + 1).ToString();

            ParsedClassEditor.AddMethod(classInDiagram, method);
            CDClassEditor.AddMethod(classInDiagram, method);
            VisualEditor.AddMethod(classInDiagram, method);
        }


        public static void AddMethod(string targetClass, Method method, Source source)
        {
            method.Name = method.Name.Replace(" ", "_");
            switch (source)
            {
                case Source.Editor:
                    AddMethod(targetClass, method);
                    Spawner.Instance.AddMethod(targetClass, method.Name, method.ReturnValue);
                    break;
                case Source.RPC:
                    AddMethod(targetClass, method);
                    break;
                case Source.Loader:
                    Spawner.Instance.AddMethod(targetClass, method.Name, method.ReturnValue);
                    var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(targetClass);

                    CDClassEditor.AddMethod(classInDiagram, method);
                    VisualEditor.AddMethod(classInDiagram, method);
                    break;
            }
        }

        public static void UpdateMethod(string targetClass, string oldMethod, Method newMethod)
        {
            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(targetClass);
            if (classInDiagram == null)
                return;

            if (DiagramPool.Instance.ClassDiagram.FindMethodByName(targetClass, newMethod.Name) != null)
                return;

            ParsedClassEditor.UpdateMethod(classInDiagram, oldMethod, newMethod);
            CDClassEditor.UpdateMethod(classInDiagram, oldMethod, newMethod);
            VisualEditor.UpdateMethod(classInDiagram, oldMethod, newMethod);
        }


        // public override void DeleteNode()
        // {
        //     throw new System.NotImplementedException();
        // }
        //
        // public override void AddEdge()
        // {
        //     throw new System.NotImplementedException();
        // }
        //
        // public override void DeleteEdge()
        // {
        //     throw new System.NotImplementedException();
        // }
        //
        // public override void DeleteMethod()
        // {
        //     throw new System.NotImplementedException();
        // }
        //
        // public override void DeleteAttribute()
        // {
        //     throw new System.NotImplementedException();
        // }
        //
        // public override void ClearDiagram()
        // {
        //     throw new System.NotImplementedException();
        // }
    }
}