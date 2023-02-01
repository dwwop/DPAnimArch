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

            //Parse all data to our List of "Class" objects
            foreach (var currentClass in classList)
            {
                currentClass.Name = currentClass.Name.Replace(" ", "_");

                MainEditor.CreateNodeSpawner(currentClass);
                var tempCdClass = DiagramPool.Instance.ClassDiagram.FindClassByName(currentClass.Name).ClassInfo;
                
                if (tempCdClass == null)
                    continue;

                if (currentClass.Attributes != null)
                {
                    foreach (var attribute in currentClass.Attributes)
                    {
                        attribute.Name = attribute.Name.Replace(" ", "_");
                        var attributeType = EXETypes.ConvertEATypeName(attribute.Type);
                        if (attributeType == null)
                        {
                            continue;
                        }

                        tempCdClass.AddAttribute(new CDAttribute(attribute.Name,
                            EXETypes.ConvertEATypeName(attributeType)));
                        currentClass.Attributes ??= new List<Attribute>();

                        ClassEditor.AddAttribute(currentClass.Name, attribute, false);
                    }
                }

                if (currentClass.Methods != null)
                {
                    foreach (var method in currentClass.Methods)
                    {
                        method.Name = method.Name.Replace(" ", "_");
                        var cdMethod = new CDMethod(tempCdClass, method.Name,
                            EXETypes.ConvertEATypeName(method.ReturnValue));
                        tempCdClass.AddMethod(cdMethod);

                        ClassEditor.AddParameters(method, cdMethod);
                        ClassEditor.AddMethod(currentClass.Name, method, ClassEditor.Source.loader);
                    }
                }

                currentClass.Top *= -1;
            }


            foreach (var relation in relationList)
            {
                ClassEditor.CreateRelationEdge(relation);
                ClassEditor.CreateRelation(relation);
            }
        }


        public static void CreateGraph()
        {
            DiagramPool.Instance.ClassDiagram.ResetDiagram();
            var go = Instantiate(DiagramPool.Instance.graphPrefab);
            DiagramPool.Instance.ClassDiagram.graph = go.GetComponent<Graph>();
            DiagramPool.Instance.ClassDiagram.graph.nodePrefab = DiagramPool.Instance.classPrefab;
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

            RenderRelations();
            RenderClassesManual();
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
                ClassEditor.SetPosition(classInDiagram.ParsedClass.Name, new Vector3(x, y, z), false);
            }
        }

        //Create GameObjects from the parsed data stored in list of Classes and Relations
        private static void RenderRelations()
        {
            //Render Relations between classes
            foreach (var rel in DiagramPool.Instance.ClassDiagram.Relations)
            {
                var prefab = rel.ParsedRelation.PrefabType;
                if (prefab == null)
                {
                    prefab = DiagramPool.Instance.associationNonePrefab;
                }

                var fromClass = DiagramPool.Instance.ClassDiagram.FindClassByName(rel.ParsedRelation.FromClass)
                    ?.VisualObject;
                var toClass = DiagramPool.Instance.ClassDiagram.FindClassByName(rel.ParsedRelation.ToClass)
                    ?.VisualObject;
                if (fromClass != null && toClass != null)
                {
                    var edge = DiagramPool.Instance.ClassDiagram.graph.AddEdge(fromClass, toClass, prefab);

                    rel.VisualObject = edge;
                    // Quickfix

                    if (edge.gameObject.transform.childCount > 0)
                    {
                        DiagramPool.Instance.ClassDiagram.StartCoroutine(QuickFix(edge.transform.GetChild(0).gameObject));
                    }
                }
                else
                {
                    Debug.LogError
                    (
                        $"Can't find specified Edge \"{rel.ParsedRelation.FromClass}\"->\"{rel.ParsedRelation.ToClass}\""
                    );
                }
            }
        }

        //Fix used to minimalize relation displaying bug
        private static IEnumerator QuickFix(GameObject g)
        {
            yield return new WaitForSeconds(0.05f);
            g.SetActive(false);
            yield return new WaitForSeconds(0.05f);
            g.SetActive(true);
        }
    }
}