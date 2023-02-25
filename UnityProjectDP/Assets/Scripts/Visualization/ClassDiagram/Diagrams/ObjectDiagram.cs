using System.Collections.Generic;
using OALProgramControl;
using TMPro;
using UMSAGL.Scripts;
using UnityEngine;
using Visualization.ClassDiagram.ClassComponents;
using Visualization.ClassDiagram.ComponentsInDiagram;
using Visualization.ClassDiagram.Relations;

namespace Visualization.ClassDiagram.Diagrams
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
            if (DiagramPool.Instance.ClassDiagram && DiagramPool.Instance.ClassDiagram.Classes != null)
            {
                foreach (var classDiagramClass in DiagramPool.Instance.ClassDiagram.Classes)
                {
                    classDiagramClass.ClassInfo.Instances.Clear();
                }
            }

            // Get rid of already rendered classes in diagram.
            if (Objects != null)
            {
                foreach (ObjectInDiagram Object in Objects)
                {
                    Destroy(Object.VisualObject);
                }
            }

            Objects = new List<ObjectInDiagram>();
            Relations = new List<ObjectRelation>();
            // if (Relations != null)
            // {
            //     foreach (ObjectRelation rel in Relations)
            //     {
            //         Destroy(rel.GameObject);
            //     }
            //
            //     Relations.Clear();
            // }
            //
            if (DiagramPool.Instance.RelationsClassToObject != null)
            {
                foreach (InterGraphRelation igr in DiagramPool.Instance.RelationsClassToObject)
                {
                    Destroy(igr);
                    igr.Destroy();
                }

                DiagramPool.Instance.RelationsClassToObject = new List<InterGraphRelation>();
            }

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

        private void GenerateObjects()
        {
        }

        public Graph CreateGraph()
        {
            ResetDiagram();
            var go = GameObject.Instantiate(DiagramPool.Instance.graphPrefab);
            graph = go.GetComponent<Graph>();
            graph.nodePrefab = DiagramPool.Instance.objectPrefab;
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
            node.SetActive(false);
            node.name = Object.VariableName + " : " + Object.Class.ParsedClass.Name;
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

            foreach (Method method in Object.Class.ParsedClass.Methods)
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
            InterGraphLine.GetComponent<InterGraphRelation>().Hide();
        }

        public void AddObject(ObjectInDiagram Object)
        {
            Objects.Add(Object);
            GenerateObject(Object);
            graph.Layout();
        }

        public void ShowObject(ObjectInDiagram Object)
        {
            Object.VisualObject.SetActive(true);
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
            return objectInDiagram;
        }

        public void AddRelation(string start, long end, string endClass, string type)
        {
            if (start.Equals(endClass))
            {
                return;
            }

            CDClass startClass = OALProgram.Instance.ExecutionSpace.getClassByName(start);
            foreach (var startClassInstance in startClass.Instances)
            {
                ObjectRelation relation = new ObjectRelation(graph, startClassInstance.UniqueID,
                    end, type, "R" + Relations.Count);
                if (!ContainsObjectRelation(relation))
                {
                    Relations.Add(relation);
                    relation.Generate();
                }
            }
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

        private bool ContainsObjectRelation(ObjectRelation objectRelation)
        {
            foreach (var relation in Relations)
            {
                if (relation.Equals(objectRelation))
                {
                    return true;
                }
            }

            return false;
        }

        public ObjectRelation FindRelation(long callerInstanceId, long calledInstanceId)
        {
            foreach (var objectRelation in Relations)
            {
                if ((objectRelation.startUniqueId == callerInstanceId &&
                     objectRelation.endUniqueId == calledInstanceId) ||
                    objectRelation.startUniqueId == calledInstanceId && objectRelation.endUniqueId == callerInstanceId)
                {
                    return objectRelation;
                }
            }

            return null;
        }
    }
}
