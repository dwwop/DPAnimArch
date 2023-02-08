using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using AnimArch.Parsing;
using AnimArch.Visualization.Animating;
using OALProgramControl;
using UnityEngine;

namespace AnimArch.Visualization.Diagrams
{
    public class ClassDiagramBuilder : MonoBehaviour
    {
        private static void ParseData()
        {
            var path = AnimationData.Instance.GetDiagramPath();
            List<Class> classList;
            List<Relation> relationList;

            switch (Path.GetExtension(path))
            {
                case ".xml":
                {
                    var xmlDocument = XMIParser.OpenDiagram();
                    classList = XMIParser.ParseClasses(xmlDocument) ?? new List<Class>();
                    relationList = XMIParser.ParseRelations(xmlDocument) ?? new List<Relation>();
                    break;
                }
                case ".json":
                {
                    var jsonDocument = JsonParser.OpenDiagram();
                    classList = JsonParser.ParseClasses(jsonDocument) ?? new List<Class>();
                    relationList = JsonParser.ParseRelations(jsonDocument) ?? new List<Relation>();
                    break;
                }
                default:
                    return;
            }

            if (classList.Count == 0)
                return;
            //Parse all data to our List of "Class" objects
            foreach (var currentClass in classList)
            {
                currentClass.Name = currentClass.Name.Replace(" ", "_");

                MainEditor.CreateNode(currentClass, MainEditor.Source.Loader);
                var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(currentClass.Name);

                if (classInDiagram.ClassInfo == null)
                    continue;

                currentClass.Attributes ??= new List<Attribute>();
                foreach (var attribute in currentClass.Attributes)
                {
                    MainEditor.AddAttribute(currentClass.Name, attribute, MainEditor.Source.Loader);
                }


                currentClass.Methods ??= new List<Method>();
                foreach (var method in currentClass.Methods)
                {
                    MainEditor.AddMethod(currentClass.Name, method, MainEditor.Source.Loader);
                }

                currentClass.Top *= -1;
            }


            foreach (var relation in relationList)
            {
                MainEditor.CreateRelation(relation, MainEditor.Source.Loader);
            }
        }


        public static void CreateGraph()
        {
            MainEditor.ClearDiagram();
            var go = Instantiate(DiagramPool.Instance.graphPrefab);
            DiagramPool.Instance.ClassDiagram.graph = go.GetComponent<Graph>();
            DiagramPool.Instance.ClassDiagram.graph.nodePrefab = DiagramPool.Instance.classPrefab;
        }

        //Auto arrange objects in space
        public void RenderClassesAuto()
        {
            DiagramPool.Instance.ClassDiagram.graph.Layout();
        }


        //Set layout as close as possible to EA layout
        private static void RenderClassesManual()
        {
            foreach (var classInDiagram in DiagramPool.Instance.ClassDiagram.Classes)
            {
                var x = classInDiagram.ParsedClass.Left * 1.25f;
                var y = classInDiagram.ParsedClass.Top * 1.25f;
                var z = classInDiagram.VisualObject.GetComponent<RectTransform>().position.z;
                VisualEditor.SetPosition(classInDiagram.ParsedClass.Name, new Vector3(x, y, z), false);
            }
        }

        public static void LoadDiagram()
        {
            CreateGraph();
            var k = 0;
            // A trick used to skip empty diagrams in XMI file from EA
            while (DiagramPool.Instance.ClassDiagram.Classes.Count < 1 && k < 10)
            {
                ParseData();
                k++;
                AnimationData.Instance.diagramId++;
            }

            RenderClassesManual();
        }
    }
}
