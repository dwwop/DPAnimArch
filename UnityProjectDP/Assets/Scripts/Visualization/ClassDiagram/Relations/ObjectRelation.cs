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
        private readonly Graph _graph;
        private readonly ObjectInDiagram _start;
        private readonly ObjectInDiagram _end;

        public ObjectRelation(Graph graph, ObjectInDiagram start, ObjectInDiagram end)
        {
            _graph = graph;
            _start = start;
            _end = end;
        }

        public void Generate()
        {
            var edge = InitEdge();
            var uEdge = edge.GetComponent<UEdge>();
            uEdge.Points = new Vector2[]
            {
                _start.VisualObject.transform.position,
                _end.VisualObject.transform.position
            };
        }

        private GameObject InitEdge()
        {
            return _graph.AddEdge(_start.VisualObject, _end.VisualObject,
                DiagramPool.Instance.associationNonePrefab);
        }
    }
}