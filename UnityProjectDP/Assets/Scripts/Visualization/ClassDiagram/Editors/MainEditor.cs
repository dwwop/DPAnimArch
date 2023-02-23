using System.Collections.Generic;
using System.Linq;
using AnimArch.Extensions;
using AnimArch.Visualization.Animating;
using Networking;
using OALProgramControl;
using UnityEngine;

namespace AnimArch.Visualization.Diagrams
{
    public class MainEditor
    {
        protected IVisualEditor _visualEditor;

        public MainEditor(IVisualEditor visualEditor)
        {
            _visualEditor = visualEditor;
        }

        public enum Source
        {
            RPC,
            Editor,
            Loader
        }

        public virtual void CreateNode(Class newClass)
        {
            var newCdClass = CDEditor.CreateNode(newClass);
            newClass.Name = newCdClass.Name;

            var classGo = _visualEditor.CreateNode(newClass);

            newClass = ParsedEditor.UpdateNodeGeometry(newClass, classGo);

            var classInDiagram = new ClassInDiagram
                { ParsedClass = newClass, ClassInfo = newCdClass, VisualObject = classGo };
            DiagramPool.Instance.ClassDiagram.Classes.Add(classInDiagram);
        }

        public virtual void UpdateNodeName(string oldName, string newName)
        {
            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(oldName);
            if (classInDiagram == null)
                return;

            classInDiagram.ParsedClass.Name = newName;
            classInDiagram.ClassInfo.Name = newName;
            classInDiagram.VisualObject.name = newName;

            _visualEditor.UpdateNode(classInDiagram.VisualObject);

            foreach (var relationInDiagram in DiagramPool.Instance.ClassDiagram.Relations)
            {
                if (relationInDiagram.ParsedRelation.FromClass == oldName)
                {
                    relationInDiagram.ParsedRelation.FromClass = newName;
                    relationInDiagram.ParsedRelation.SourceModelName = newName;
                    relationInDiagram.RelationInfo.FromClass = newName;
                }

                if (relationInDiagram.ParsedRelation.ToClass == oldName)
                {
                    relationInDiagram.ParsedRelation.ToClass = newName;
                    relationInDiagram.ParsedRelation.TargetModelName = newName;
                    relationInDiagram.RelationInfo.ToClass = newName;
                }
            }
        }

        public virtual void AddAttribute(string targetClass, Attribute attribute)
        {
            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(targetClass);
            if (classInDiagram == null) return;

            if (DiagramPool.Instance.ClassDiagram.FindAttributeByName(targetClass, attribute.Name) != null)
            {
                CDEditor.AddAttribute(classInDiagram, attribute);
                _visualEditor.AddAttribute(classInDiagram, attribute);
                return;
            }

            attribute.Id = (classInDiagram.ParsedClass.Attributes.Count + 1).ToString();

            ParsedEditor.AddAttribute(classInDiagram, attribute);
            CDEditor.AddAttribute(classInDiagram, attribute);
            _visualEditor.AddAttribute(classInDiagram, attribute);
        }

        public virtual void UpdateAttribute(string targetClass, string oldAttribute, Attribute newAttribute)
        {
            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(targetClass);
            if (classInDiagram == null)
                return;

            if (DiagramPool.Instance.ClassDiagram.FindAttributeByName(targetClass, oldAttribute) == null)
                return;

            ParsedEditor.UpdateAttribute(classInDiagram, oldAttribute, newAttribute);
            CDEditor.UpdateAttribute(classInDiagram, oldAttribute, newAttribute);
            _visualEditor.UpdateAttribute(classInDiagram, oldAttribute, newAttribute);
        }

        public virtual void AddMethod(string targetClass, Method method)
        {
            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(targetClass);
            if (classInDiagram == null)
                return;

            classInDiagram.ParsedClass.Methods ??= new List<Method>();

            if (DiagramPool.Instance.ClassDiagram.FindMethodByName(targetClass, method.Name) != null)
            {
                //TODO: david skontrolovat - vetva kedy je diagram z loadera?
                CDEditor.AddMethod(classInDiagram, method);
                _visualEditor.AddMethod(classInDiagram, method);
                return;
            }

            method.Id = (classInDiagram.ParsedClass.Methods.Count + 1).ToString();

            ParsedEditor.AddMethod(classInDiagram, method);
            CDEditor.AddMethod(classInDiagram, method);
            _visualEditor.AddMethod(classInDiagram, method);
        }

        public virtual void UpdateMethod(string targetClass, string oldMethod, Method newMethod)
        {
            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(targetClass);
            if (classInDiagram == null)
                return;

            if (DiagramPool.Instance.ClassDiagram.FindMethodByName(targetClass, oldMethod) == null)
                return;

            ParsedEditor.UpdateMethod(classInDiagram, oldMethod, newMethod);
            CDEditor.UpdateMethod(classInDiagram, oldMethod, newMethod);
            _visualEditor.UpdateMethod(classInDiagram, oldMethod, newMethod);
        }

        private void CreateRelation(Relation relation)
        {
            relation.FromClass = relation.SourceModelName.Replace(" ", "_");
            relation.ToClass = relation.TargetModelName.Replace(" ", "_");

            var cdRelation = CDEditor.CreateRelation(relation);
            var relationGo = _visualEditor.CreateRelation(relation);

            var relationInDiagram = new RelationInDiagram
                { ParsedRelation = relation, RelationInfo = cdRelation, VisualObject = relationGo };
            DiagramPool.Instance.ClassDiagram.Relations.Add(relationInDiagram);
        }


        public void CreateRelation(Relation relation, Source source)
        {
            switch (source)
            {
                case Source.Loader:
                case Source.Editor:
                    CreateRelation(relation);
                    break;
                case Source.RPC:
                    CreateRelation(relation);
                    break;
            }
        }

        public virtual void DeleteRelation(GameObject relation)
        {
            var relationInDiagram = DiagramPool.Instance.ClassDiagram.Relations
                .Find(x => x.VisualObject.Equals(relation));

            CDEditor.DeleteRelation(relationInDiagram);
            _visualEditor.DeleteRelation(relationInDiagram);

            DiagramPool.Instance.ClassDiagram.Relations.Remove(relationInDiagram);
        }

        private void DeleteNodeFromRelations(ClassInDiagram classInDiagram)
        {
            DiagramPool.Instance.ClassDiagram.Relations
                .Where(x => x.ParsedRelation.FromClass == classInDiagram.ParsedClass.Name
                            || x.ParsedRelation.ToClass == classInDiagram.ParsedClass.Name)
                .ForEach(x =>
                {
                    CDEditor.DeleteRelation(x);
                    _visualEditor.DeleteRelation(x);
                });

            DiagramPool.Instance.ClassDiagram.Relations
                .RemoveAll(x => x.ParsedRelation.FromClass == classInDiagram.ParsedClass.Name
                                || x.ParsedRelation.ToClass == classInDiagram.ParsedClass.Name);
        }

        public virtual void DeleteNode(string className)
        {
            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(className);
            if (classInDiagram == null)
                return;

            DeleteNodeFromRelations(classInDiagram);

            CDEditor.DeleteNode(classInDiagram);
            _visualEditor.DeleteNode(classInDiagram);

            DiagramPool.Instance.ClassDiagram.Classes.Remove(classInDiagram);
        }

        public virtual void DeleteAttribute(string className, string attributeName)
        {
            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(className);
            if (classInDiagram == null)
                return;

            if (DiagramPool.Instance.ClassDiagram.FindAttributeByName(className, attributeName) == null)
                return;

            ParsedEditor.DeleteAttribute(classInDiagram, attributeName);
            CDEditor.DeleteAttribute(classInDiagram, attributeName);
            _visualEditor.DeleteAttribute(classInDiagram, attributeName);
        }

        public virtual void DeleteMethod(string className, string methodName)
        {
            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(className);
            if (classInDiagram == null)
                return;

            if (DiagramPool.Instance.ClassDiagram.FindMethodByName(className, methodName) == null)
                return;

            ParsedEditor.DeleteMethod(classInDiagram, methodName);
            CDEditor.DeleteMethod(classInDiagram, methodName);
            _visualEditor.DeleteMethod(classInDiagram, methodName);
        }

        public void ClearDiagram()
        {
            // Get rid of already rendered classes in diagram.
            if (DiagramPool.Instance.ClassDiagram.Classes != null)
            {
                foreach (var Class in DiagramPool.Instance.ClassDiagram.Classes) Object.Destroy(Class.VisualObject);

                DiagramPool.Instance.ClassDiagram.Classes.Clear();
            }


            // Get rid of already rendered relations in diagram.
            if (DiagramPool.Instance.ClassDiagram.Relations != null)
            {
                foreach (var relation in DiagramPool.Instance.ClassDiagram.Relations)
                    Object.Destroy(relation.VisualObject);

                DiagramPool.Instance.ClassDiagram.Relations.Clear();
            }


            if (DiagramPool.Instance.ClassDiagram.graph != null)
            {
                Object.Destroy(DiagramPool.Instance.ClassDiagram.graph.gameObject);
                DiagramPool.Instance.ClassDiagram.graph = null;
            }

            OALProgram.Instance.Reset();

            AnimationData.Instance.ClearData();
        }
    }
}
