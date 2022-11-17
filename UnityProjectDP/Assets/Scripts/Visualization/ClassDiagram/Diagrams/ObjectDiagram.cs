using System.Collections.Generic;
using OALProgramControl;
using TMPro;
using UnityEngine;

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

            GenerateObjects();

            //Generate UI objects displaying the diagram
            Generate();

            //Set the layout of diagram so it is corresponding to EA view
            ManualLayout();
            //AutoLayout();

            graph.transform.position = new Vector3(0, 0, 800);
        }

        public void LoadDiagram1()
        {
            GenerateObjects();

            //Generate UI objects displaying the diagram
            Generate();

            //Set the layout of diagram so it is corresponding to EA view
            ManualLayout();
            //AutoLayout();

            graph.transform.position = new Vector3(0, 0, 800);
        }

        private void GenerateObjects()
        {
            // Animating.Animation instance = Animating.Animation.Instance;
            // if (!string.IsNullOrEmpty(instance.startClassName))
            // {
            //     Objects.Add
            //     (
            //         new ObjectInDiagram
            //         {
            //             Class = DiagramPool.Instance.ClassDiagram.FindClassByName(instance.startClassName),
            //             Instance = new CDClassInstance(1, new List<CDAttribute>()),
            //             VisualObject = null,
            //             VariableName = "client"
            //         }
            //     );
            // }
            //
            // foreach (var cdClass in OALProgram.Instance.SuperScope.Commands)
            // {}
            // var cd = OALProgram.Instance.ExecutionSpace.ClassPool;
            // var i = 0;
            // foreach (var objectInDiagram in cd.Select(classInDiagram => new ObjectInDiagram
            //          {
            //              Class = DiagramPool.Instance.ClassDiagram.FindClassByName(classInDiagram.Name),
            //              Instance = classInDiagram.CreateClassInstance(),
            //              VisualObject = null,
            //              VariableName = ""
            //          }))
            // {
            //     Objects.Add(objectInDiagram);
            //     // classInDiagram.objects.Add(objectInDiagram);
            //     i++;
            // }
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
            foreach (ObjectInDiagram objectInDiagram in Objects)
            {
                // objectInDiagram.VisualObject.GetComponent<RectTransform>()
                //     .Shift(300 * ((int) (i / 2) - 1), 200 * (i % 2), 0);
                objectInDiagram.VisualObject.transform.position = objectInDiagram.Class.VisualObject.transform.position;
                i++;
            }
        }

        private void Generate()
        {
            //Render classes
            for (int i = 0; i < Objects.Count; i++)
            {
                Debug.Log(Objects[i].Class.ClassInfo.Name);
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
            node.name = Object.VariableName + " : " + Object.Class.XMIParsedClass.Name;
            var background = node.transform.Find("Background");
            var header = background.Find("Header");
            var attributes = background.Find("Attributes");
            var methods = background.Find("Methods");

            // Printing the values into diagram
            header.GetComponent<TextMeshProUGUI>().text = node.name;

            //Attributes
            foreach (string AttributeName in Object.Instance.State.Keys)
            {
                attributes.GetComponent<TextMeshProUGUI>().text +=
                    AttributeName + " = " + Object.Instance.State[AttributeName] + "\n";
            }

            foreach (Method method in Object.Class.XMIParsedClass.Methods)
            {
                string arguments = "(";
                if (method.arguments != null)
                    for (int d = 0; d < method.arguments.Count; d++)
                    {
                        if (d < method.arguments.Count - 1)
                            arguments += (method.arguments[d] + ", ");
                        else arguments += (method.arguments[d]);
                    }

                arguments += ")";

                methods.GetComponent<TextMeshProUGUI>().text +=
                    method.Name + arguments + " :" + method.ReturnValue + "\n";
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

        public ObjectInDiagram AddObject(string className, string variableName, CDClassInstance instance)
        {
            ObjectInDiagram objectInDiagram = new ObjectInDiagram
            {
                Class = DiagramPool.Instance.ClassDiagram.FindClassByName(className),
                Instance = instance,
                VisualObject = null,
                VariableName = variableName
            };
            AddObject(objectInDiagram);
            return objectInDiagram;
        }

        public void AddRelation(ObjectInDiagram start, ObjectInDiagram end)
        {
            ObjectRelation relation = new ObjectRelation(graph, start, end);
            Relations.Add(relation);
            relation.Generate();
        }

        public ObjectInDiagram FindByID(long instanceID)
        {
            foreach (var objectInDiagram in Objects)
            {
                if (objectInDiagram.Instance.UniqueID == instanceID)
                {
                    return objectInDiagram;
                }
            }

            return null;
        }

        public bool AddAttributeValue(long instanceID, string attr, string expr)
        {
            ObjectInDiagram objectInDiagram = FindByID(instanceID);
            if (objectInDiagram == null)
            {
                return false;
            }

            objectInDiagram.Instance.SetAttribute(attr, expr);
            var background = objectInDiagram.VisualObject.transform.Find("Background");
            var attributes = background.Find("Attributes");
            attributes.GetComponent<TextMeshProUGUI>().text = "";

            //Attributes
            foreach (string AttributeName in objectInDiagram.Instance.State.Keys)
            {
                attributes.GetComponent<TextMeshProUGUI>().text +=
                    AttributeName + " = " + objectInDiagram.Instance.State[AttributeName] + "\n";
            }

            return true;
        }
    }
}