using System;
using AnimArch.Extensions.Unity;
using OALProgramControl;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace AnimArch.Visualization.Diagrams
{
    public class ObjectDiagram : Diagram
    {
        public Graph graph;
        public List<ObjectInDiagram> Objects { get; private set; }
        public List<ObjectRelation> Relations { get; private set; }

        private void Awake()
        {
            DiagramPool.Instance.ObjectDiagram = this;
            ResetDiagram();
        }

        public void ResetDiagram()
        {
            // Get rid of already rendered classes in diagram.
            if (Objects != null)
            {
                foreach (ObjectInDiagram Object in Objects)
                {
                    Destroy(Object.VisualObject);
                }

                Objects.Clear();
            }

            Objects = new List<ObjectInDiagram>();
            Relations = new List<ObjectRelation>();

            if (graph != null)
            {
                Destroy(graph.gameObject);
                graph = null;
            }
        }

        public void LoadDiagram()
        {
            CreateGraph();

            fakeObjects();

            //Generate UI objects displaying the diagram
            Generate();

            //Set the layout of diagram so it is corresponding to EA view
            ManualLayout();
            //AutoLayout();

            // Objects
            //     .ForEach(Object => Object.VisualObject.GetComponent<RectTransform>().Shift(0, 0, 400));
            graph.transform.position = new Vector3(0, 0, 800);
            //Objects
            //.ForEach
            //(
            //    Object => CreateInterGraphLine(graph, Object.VisualObject, Object.Class.VisualObject)
            //);
        }

        public Graph CreateGraph()
        {
            ResetDiagram();
            var go = GameObject.Instantiate(DiagramPool.Instance.graphPrefab);
            graph = go.GetComponent<Graph>();
            return graph;
        }

        public void ManualLayout()
        {
            int i = 0;
            foreach (ObjectInDiagram Object in Objects)
            {
                Object.VisualObject.GetComponent<RectTransform>().Shift(300 * ((int) (i / 2) - 1), 200 * (i % 2), 0);
                i++;
            }
        }

        private void Generate()
        {
            //Render classes
            for (int i = 0; i < Objects.Count; i++)
            {
                GenerateObject(Objects[i]);
            }

            foreach (ObjectRelation relation in Relations)
            {
                relation.Generate();
            }
        }

        private void GenerateObject(ObjectInDiagram Object)
        {
            //Setting up
            var node = graph.AddNode();
            node.name = Object.VariableName + ":" + Object.Class.XMIParsedClass.Name;
            var background = node.transform.Find("Background");
            var header = background.Find("Header");
            var attributes = background.Find("Attributes");

            // Printing the values into diagram
            header.GetComponent<TextMeshProUGUI>().text = node.name;

            //Attributes
            foreach (string AttributeName in Object.Instance.State.Keys)
            {
                attributes.GetComponent<TextMeshProUGUI>().text +=
                    AttributeName + " = " + Object.Instance.State[AttributeName] + "\n";
            }

            //Add Class to Dictionary
            Object.VisualObject = node;

            // Create Edge towards class
            GameObject InterGraphLine = CreateInterGraphLine(Object.Class.VisualObject, Object.VisualObject);
            InterGraphLine.GetComponent<InterGraphRelation>().Initialize(Object, Object.Class);
            DiagramPool.Instance.RelationsClassToObject.Add
            (
                InterGraphLine.GetComponent<InterGraphRelation>()
            );
        }

        public void AddObject(ObjectInDiagram Object)
        {
            Objects.Add(Object);
            GenerateObject(Object);
            graph.Layout();
        }

        private void fakeObjects()
        {
            ObjectInDiagram x_leaf =
                new ObjectInDiagram()
                {
                    Class = DiagramPool.Instance.ClassDiagram.FindClassByName("ASTLeaf"),
                    Instance = new CDClassInstance(1, new List<CDAttribute>()),
                    VisualObject = null,
                    VariableName = "x"
                };

            ObjectInDiagram y_leaf =
                new ObjectInDiagram()
                {
                    Class = DiagramPool.Instance.ClassDiagram.FindClassByName("ASTLeaf"),
                    Instance = new CDClassInstance(3, new List<CDAttribute>()),
                    VisualObject = null,
                    VariableName = "y"
                };

            ObjectInDiagram composite =
                new ObjectInDiagram()
                {
                    Class = DiagramPool.Instance.ClassDiagram.FindClassByName("ASTComposite"),
                    Instance = new CDClassInstance(2, new List<CDAttribute>()),
                    VisualObject = null,
                    VariableName = "Composite"
                };

            ObjectInDiagram plus_operator = new ObjectInDiagram()
            {
                Class = DiagramPool.Instance.ClassDiagram.FindClassByName("Operator"),
                Instance = new CDClassInstance(6, new List<CDAttribute>()),
                VisualObject = null,
                VariableName = "Plus"
            };
            ObjectInDiagram evaluator1 = new ObjectInDiagram()
            {
                Class = DiagramPool.Instance.ClassDiagram.FindClassByName("OperationEvaluator"),
                Instance = new CDClassInstance(7, new List<CDAttribute>()),
                VisualObject = null,
                VariableName = "x"
            };
            ObjectInDiagram evaluator2 = new ObjectInDiagram()
            {
                Class = DiagramPool.Instance.ClassDiagram.FindClassByName("OperationEvaluator"),
                Instance = new CDClassInstance(8, new List<CDAttribute>()),
                VisualObject = null,
                VariableName = "y"
            };
            ObjectInDiagram evaluator4 = new ObjectInDiagram()
            {
                Class = DiagramPool.Instance.ClassDiagram.FindClassByName("OperationEvaluator"),
                Instance = new CDClassInstance(9, new List<CDAttribute>()),
                VisualObject = null,
                VariableName = "composite"
            };


            Objects.Add(evaluator1);
            Objects.Add(evaluator2);
            Objects.Add(x_leaf);
            Objects.Add(y_leaf);
            Objects.Add(composite);
            Objects.Add(plus_operator);
            Objects.Add(evaluator4);

            Relations.Add(new ObjectRelation(graph, x_leaf, composite, "Association"));
            Relations.Add(new ObjectRelation(graph, y_leaf, composite, "Association"));
            Relations.Add(new ObjectRelation(graph, composite, plus_operator, "Association"));

            Relations.Add(new ObjectRelation(graph, x_leaf, evaluator1, "Depends"));
            Relations.Add(new ObjectRelation(graph, y_leaf, evaluator2, "Depends"));
            Relations.Add(new ObjectRelation(graph, composite, evaluator4, "Depends"));
        }
    }
}