using System.Collections.Generic;
using Networking;
using OALProgramControl;

namespace AnimArch.Visualization.Diagrams
{
    public static class MainEditor
    {
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

        public static void CreateNodeSpawner(Class newClass)
        {
            Spawner.Instance.SpawnNode(newClass.Name, newClass.XmiId);
            CreateNode(newClass);
        }

        public static void CreateNodeFromRpc(string name, string id)
        {
            var newClass = ParsedClassEditor.CreateNode(name, id);
            CreateNode(newClass);
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

    

        public static void AddMethod(string targetClass, Method method, ClassEditor.Source source)
        {
            switch (source)
            {
                case ClassEditor.Source.editor:
                    AddMethod(targetClass, method);
                    Spawner.Instance.AddMethod(targetClass, method.Name, method.ReturnValue);
                    break;
                case ClassEditor.Source.RPC:
                    AddMethod(targetClass, method);
                    break;
                case ClassEditor.Source.loader:
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
        // public override void AddMethod()
        // {
        //     throw new System.NotImplementedException();
        // }
        //
        // public override void UpdateMethod()
        // {
        //     throw new System.NotImplementedException();
        // }
        //
        // public override void DeleteMethod()
        // {
        //     throw new System.NotImplementedException();
        // }
        //
        // public override void AddAttribute()
        // {
        //     throw new System.NotImplementedException();
        // }
        //
        // public override void UpdateAttribute()
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