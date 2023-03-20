using UnityEngine;
using Visualization.ClassDiagram.ClassComponents;
using Visualization.ClassDiagram.ComponentsInDiagram;
using Visualization.ClassDiagram.Relations;

namespace Visualization.ClassDiagram.Editors
{
    public abstract class IVisualEditor
    {
        public abstract void UpdateNodeName(GameObject classGo);

        public abstract GameObject CreateNode(Class newClass);

        public abstract void SetPosition(string className, Vector3 position);

        public abstract void UpdateNode(GameObject classGo);

        public abstract void AddAttribute(ClassInDiagram classInDiagram, Attribute attribute);

        public abstract void UpdateAttribute(ClassInDiagram classInDiagram, string oldAttribute, Attribute newAttribute);

        public abstract void AddMethod(ClassInDiagram classInDiagram, Method method);

        public abstract void UpdateMethod(ClassInDiagram classInDiagram, string oldMethod, Method newMethod);

        public abstract GameObject CreateRelation(Relation relation);

        public abstract void DeleteRelation(RelationInDiagram relationInDiagram);

        public abstract void DeleteNode(ClassInDiagram classInDiagram);

        public abstract void DeleteAttribute(ClassInDiagram classInDiagram, string attribute);

        public abstract void DeleteMethod(ClassInDiagram classInDiagram, string method);
    }
}
