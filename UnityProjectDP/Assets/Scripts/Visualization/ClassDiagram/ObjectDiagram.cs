using AnimArch.Extensions.Unity;
using OALProgramControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace AnimArch.Visualization.Diagrams
{
    public class ObjectDiagram : Diagram
    {
        public Graph graph;
        public List<ObjectInDiagram> Objects { get; private set; }
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

            Objects
                .ForEach(Object => Object.VisualObject.GetComponent<RectTransform>().Shift(0, 0, 400));

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
                Object.VisualObject.GetComponent<RectTransform>().Shift(0, 200 * i++, 0);
            }
        }
        private void Generate()
        {
            //Render classes
            for (int i = 0; i < Objects.Count; i++)
            {
                GenerateObject(Objects[i]);
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
                attributes.GetComponent<TextMeshProUGUI>().text += AttributeName + " = " + Object.Instance.State[AttributeName] + "\n";
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
            Objects.Add
            (
                new ObjectInDiagram()
                {
                    Class = DiagramPool.Instance.ClassDiagram.FindClassByName("ASTLeaf"),
                    Instance = new CDClassInstance(1, new List<CDAttribute>()),
                    VisualObject = null,
                    VariableName = "Operand01"
                }
            );

            Objects.Add
            (
                new ObjectInDiagram()
                {
                    Class = DiagramPool.Instance.ClassDiagram.FindClassByName("ASTLeaf"),
                    Instance = new CDClassInstance(3, new List<CDAttribute>()),
                    VisualObject = null,
                    VariableName = "Operand02"
                }
            );

            Objects.Add
            (
                new ObjectInDiagram()
                {
                    Class = DiagramPool.Instance.ClassDiagram.FindClassByName("ASTLeaf"),
                    Instance = new CDClassInstance(5, new List<CDAttribute>()),
                    VisualObject = null,
                    VariableName = "Operand03"
                }
            );
        }
    }
}
