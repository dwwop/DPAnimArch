using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OALProgramControl;
using AnimArch.Extensions;
using AnimArch.Visualization.Animating;

namespace AnimArch.Visualization.Diagrams
{
    public class ClassDiagram : Diagram
    {
        public Graph graph;
        public List<ClassInDiagram> Classes { get; private set; }
        public List<RelationInDiagram> Relations { get; private set; }

        //Awake is called before the first frame and before Start()
        private void Awake()
        {
            Classes = new List<ClassInDiagram>();
            DiagramPool.Instance.ClassDiagram = this;
            ResetDiagram();
        }

        private void ResetDiagram()
        {
            // Get rid of already rendered classes in diagram.
            if (Classes != null)
            {
                foreach (var Class in Classes)
                {
                    Destroy(Class.VisualObject);
                }

                Classes.Clear();
            }

            Classes = new List<ClassInDiagram>();

            // Get rid of already rendered relations in diagram.
            if (Relations != null)
            {
                foreach (var relation in Relations)
                {
                    Destroy(relation.VisualObject);
                }

                Relations.Clear();
            }

            Relations = new List<RelationInDiagram>();

            if (graph != null)
            {
                Destroy(graph.gameObject);
                graph = null;
            }

            OALProgram.Instance.Reset();
            ClassEditor.Instance.ResetGraph();

            AnimationData.Instance.ClearData();
        }

        public void LoadDiagram()
        {
            CreateGraph();
            var k = 0;
            // A trick used to skip empty diagrams in XMI file from EA
            while (Classes.Count < 1 && k < 10)
            {
                ClassEditor.ParseData();
                k++;
                AnimationData.Instance.diagramId++;
            }

            RenderRelations();
            RenderClassesManual();
        }

        public Graph CreateGraph()
        {
            ResetDiagram();
            var go = Instantiate(DiagramPool.Instance.graphPrefab);
            graph = go.GetComponent<Graph>();
            graph.nodePrefab = DiagramPool.Instance.classPrefab;
            return graph;
        }

        //Auto arrange objects in space
        public void RenderClassesAuto()
        {
            graph.Layout();
        }

        //Set layout as close as possible to EA layout
        private void RenderClassesManual()
        {
            foreach (var classInDiagram in Classes)
            {
                var x = classInDiagram.ParsedClass.Left * 1.25f;
                var y = classInDiagram.ParsedClass.Top * 1.25f;
                var z = classInDiagram.VisualObject.GetComponent<RectTransform>().position.z;
                ClassEditor.SetPosition(classInDiagram.ParsedClass.Name, new Vector3(x, y, z), false);
            }
        }

        //Create GameObjects from the parsed data stored in list of Classes and Relations
        private void RenderRelations()
        {
            //Render Relations between classes
            foreach (var rel in Relations)
            {
                var prefab = rel.ParsedRelation.PrefabType;
                if (prefab == null)
                {
                    prefab = DiagramPool.Instance.associationNonePrefab;
                }

                var startingClass = FindClassByName(rel.ParsedRelation.FromClass)?.VisualObject;
                var finishingClass = FindClassByName(rel.ParsedRelation.ToClass)?.VisualObject;
                if (startingClass != null && finishingClass != null)
                {
                    var edge = graph.AddEdge(startingClass, finishingClass, prefab);

                    rel.VisualObject = edge;
                    // Quickfix

                    if (edge.gameObject.transform.childCount > 0)
                    {
                        StartCoroutine(QuickFix(edge.transform.GetChild(0).gameObject));
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
                .Where(_ => string.Equals(methodName, _class.ParsedClass.Name))
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
                .Where(_ => string.Equals(attributeName, _class.ParsedClass.Name))
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

        public GameObject FindEdge(string classA, string classB)
        {
            GameObject result = null;

            var rel = OALProgram.Instance.RelationshipSpace.GetRelationshipByClasses(classA, classB);
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

        //Fix used to minimalize relation displaying bug
        private static IEnumerator QuickFix(GameObject g)
        {
            yield return new WaitForSeconds(0.05f);
            g.SetActive(false);
            yield return new WaitForSeconds(0.05f);
            g.SetActive(true);
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