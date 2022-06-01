using AnimArch.Extensions.Unity;
using OALProgramControl;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI.Extensions;


namespace AnimArch.Visualization.Diagrams
{
    public class ObjectRelation
    {
        public Graph graph;
        public ObjectInDiagram start;
        public ObjectInDiagram end;
        public string type;

        public ObjectRelation(Graph graph, ObjectInDiagram start, ObjectInDiagram end, string type)
        {
            this.graph = graph;
            this.start = start;
            this.end = end;
            this.type = type;
        }

        public void Generate()
        {
            GameObject edge = InitEdge();
            UEdge uEdge = edge.GetComponent<UEdge>();
            uEdge.Points = new Vector2[]
            {
                start.VisualObject.transform.position,
                end.VisualObject.transform.position
            };
        }

        private GameObject InitEdge()
        {
            if (type.Equals("Association"))
            {
                return graph.AddEdge(start.VisualObject, end.VisualObject, DiagramPool.Instance.associationNonePrefab);
            }

            if (type.Equals("Depends"))
            {
                return graph.AddEdge(start.VisualObject, end.VisualObject, DiagramPool.Instance.dependsPrefab);
            }

            return new GameObject();
        }
    }
}