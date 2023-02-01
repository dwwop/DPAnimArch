using Networking;

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

            classInDiagram.ClassInfo.Name = newName;
            classInDiagram.ParsedClass.Name = newName;
            classInDiagram.VisualObject.name = newName;

            VisualEditor.UpdateNode(classInDiagram.VisualObject);

            RelationshipEditor.UpdateNodeName(oldName, newName);
        }

        // public override void UpdateNode()
        // {
        //     throw new System.NotImplementedException();
        // }
        //
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