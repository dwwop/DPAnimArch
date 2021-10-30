using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AnimArch.Visualization.ClassDiagram
{
    public class ClassDiagramLayer
    {
        public readonly Dictionary<string, DiagramElement<Class>> Classes;
        public readonly Dictionary<string, DiagramElement<Relation>> Relations;

        public ClassDiagramLayer()
        {
            this.Classes = new Dictionary<string, DiagramElement<Class>>();
            this.Relations = new Dictionary<string, DiagramElement<Relation>>();
        }

        public bool AddClass(Class DataObject, GameObject VisualObject)
        {
            if (this.Classes.ContainsKey(DataObject.Name))
            {
                return false;
            }

            Classes.Add(DataObject.Name, new DiagramElement<Class>(DataObject, VisualObject));

            return true;
        }

        public bool AddRelation(Relation DataObject, GameObject VisualObject)
        {
            string Name = RelationName(DataObject);

            if (this.Classes.ContainsKey(Name))
            {
                return false;
            }

            Relations.Add(Name, new DiagramElement<Relation>(DataObject, VisualObject));

            return true;
        }
        // TODO - should be performed by relation somehow
        private string RelationName(Relation Relation)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Relation.FromClass);
            sb.Append("->");
            sb.Append(Relation.ToClass);

            return sb.ToString();
        }
    }
}