using Networking;

namespace AnimArch.Visualization.Diagrams
{
    public static class MainEditor
    {
        public static void CreateNode(Class newClass)
        {
            var newCdClass = CDClassEditor.CreateNode(newClass);
            newClass.Name = newCdClass.Name;

            var classGo = VisualEditor.CreateNode(newClass);

            newClass = ParsedClassEditor.UpdateClassGeometry(newClass, classGo);

            var classInDiagram = new ClassInDiagram
                { ParsedClass = newClass, ClassInfo = newCdClass, VisualObject = classGo };
            DiagramPool.Instance.ClassDiagram.Classes.Add(classInDiagram);
        }

        public static void CreateNodeSpawner(Class newClass)
        {
            Spawner.Instance.SpawnClass(newClass.Name, newClass.XmiId);
            CreateNode(newClass);
        }

        public static void CreateNodeFromRpc(string name, string id)
        {
            var newClass = ParsedClassEditor.CreateNode(name, id);
            CreateNode(newClass);
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