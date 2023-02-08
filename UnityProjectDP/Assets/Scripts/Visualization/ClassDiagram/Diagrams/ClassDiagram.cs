using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OALProgramControl;
using AnimArch.Extensions;

namespace AnimArch.Visualization.Diagrams
{
    public class ClassDiagram : Diagram
    {
        public Graph graph;
        public List<ClassInDiagram> Classes { get; } = new();
        public List<RelationInDiagram> Relations { get; } = new();

        //Awake is called before the first frame and before Start()
        private void Awake()
        {
            DiagramPool.Instance.ClassDiagram = this;
            MainEditor.ClearDiagram();
        }

        public ClassInDiagram FindClassByName(string className)
        {
            return Classes
                .Where(classInDiagram => string.Equals(className, classInDiagram.ParsedClass.Name))
                .IfMoreThan
                (
                    _ => Debug.LogError
                    (
                        $"More than 1 class named \"{className}\" found in ClassDiagram::Classes"
                    )
                )
                .FirstOrDefault();
        }

        public Method FindMethodByName(string className, string methodName)
        {
            var _class = FindClassByName(className);

            return _class?.ParsedClass.Methods
                .Where(x => string.Equals(methodName, x.Name))
                .IfMoreThan
                (
                    _ => Debug.LogError
                    (
                        $"More than 1 method named \"{className}::{methodName}\" found in ClassDiagram::Classes"
                    )
                )
                .FirstOrDefault();
        }

        public Attribute FindAttributeByName(string className, string attributeName)
        {
            var _class = FindClassByName(className);

            return _class?.ParsedClass.Attributes
                .Where(x => Equals(attributeName, x.Name))
                .IfMoreThan
                (
                    _ => Debug.LogError
                    (
                        $"More than 1 attribute named \"{className}::{attributeName}\" found in ClassDiagram::Classes"
                    )
                )
                .FirstOrDefault();
        }

        public GameObject FindNode(string name)
        {
            GameObject g;
            g = FindClassByName(name).VisualObject;
            return g;
        }

        public GameObject FindEdge(string fromClass, string toClass)
        {
            GameObject result = null;

            var rel = OALProgram.Instance.RelationshipSpace.GetRelationshipByClasses(fromClass, toClass);
            if (rel != null)
            {
                result = FindEdge(rel.RelationshipName);
            }

            return result;
        }

        public GameObject FindEdge(string relationshipName)
        {
            return Relations
                .FirstOrDefault(relation => string.Equals(relationshipName, relation.RelationInfo.RelationshipName))?
                .VisualObject;
        }

        public RelationInDiagram FindEdgeInfo(string relationshipName)
        {
            return Relations
                .FirstOrDefault(relation => string.Equals(relationshipName, relation.RelationInfo.RelationshipName));
        }

        public string FindOwnerOfRelation(string relationName)
        {
            return Relations
                .Where(relation => string.Equals(relationName, relation.ParsedRelation.OALName))
                .FirstOrCustomDefault(
                    relationInDiagram => relationInDiagram.ParsedRelation.FromClass, "");
        }


        public IEnumerable<Class> GetClassList()
        {
            return Classes.Select(classInDiagram => classInDiagram.ParsedClass);
        }

        public IEnumerable<Relation> GetRelationList()
        {
            return Relations.Select(relationInDiagram => relationInDiagram.ParsedRelation);
        }
    }
}
