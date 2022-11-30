using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using TMPro;
using OALProgramControl;
using UnityEngine.UI;
using AnimArch.Extensions;
using AnimArch.Visualization.Animating;
using AnimArch.XMIParsing;

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
        public void ResetDiagram()
        {
            // Get rid of already rendered classes in diagram.
            if (Classes != null)
            {
                foreach (ClassInDiagram Class in Classes)
                {
                    Destroy(Class.VisualObject);
                }

                Classes.Clear();
            }
            Classes = new List<ClassInDiagram>();

            // Get rid of already rendered relations in diagram.
            if (Relations != null)
            {
                foreach (RelationInDiagram Relation in Relations)
                {
                    Destroy(Relation.VisualObject);
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
            int k = 0;
            // A trick used to skip empty diagrams in XMI file from EA
            while (Classes.Count < 1 && k < 10)
            {
                ParseData();
                k++;
                AnimationData.Instance.diagramId++;
            }
            Generate();
            ManualLayout();
        }
        public Graph CreateGraph()
        {
            ResetDiagram();
            var go = Instantiate(DiagramPool.Instance.graphPrefab);
            graph = go.GetComponent<Graph>();
            return graph;
        }

        // Parser used to parse data from XML to C# data structures
        void ParseData()
        {
            List<Class> XMIClassList = XMIParser.ParseClasses();
            if (XMIClassList == null)
            {
                XMIClassList = new List<Class>();
            }

            CDClass TempCDClass;

            //Parse all data to our List of "Class" objects
            foreach (Class CurrentClass in XMIClassList)
            {
                CurrentClass.Name = CurrentClass.Name.Replace(" ", "_");

                TempCDClass = ClassEditor.Instance.CreateNode(CurrentClass);
                if (TempCDClass == null)
                    continue;

                if (CurrentClass.Attributes != null)
                {
                    foreach (Attribute CurrentAttribute in CurrentClass.Attributes)
                    {
                        CurrentAttribute.Name = CurrentAttribute.Name.Replace(" ", "_");
                        String AttributeType = EXETypes.ConvertEATypeName(CurrentAttribute.Type);
                        if (AttributeType == null)
                        {
                            continue;
                        }
                        TempCDClass.AddAttribute(new CDAttribute(CurrentAttribute.Name, EXETypes.ConvertEATypeName(AttributeType)));
                        if (CurrentClass.Attributes == null)
                        {
                            CurrentClass.Attributes = new List<Attribute>();
                        }
                    }
                }

                if (CurrentClass.Methods != null)
                {
                    foreach (var method in CurrentClass.Methods)
                    {
                        method.Name = method.Name.Replace(" ", "_");
                        var cdMethod = new CDMethod(TempCDClass, method.Name, EXETypes.ConvertEATypeName(method.ReturnValue));
                        TempCDClass.AddMethod(cdMethod);
                        
                        ClassEditor.AddParameters(method, cdMethod);
                        ClassEditor.AddMethod(CurrentClass.Name, method, ClassEditor.Source.loader);
                    }
                }
                CurrentClass.Top *= -1;
            }

            List<Relation> XMIRelationList = XMIParser.ParseRelations();
            if (XMIRelationList == null)
            {
                XMIRelationList = new List<Relation>();
            }

            CDRelationship TempCDRelationship;

            //Parse all Relations between classes
            foreach (Relation Relation in XMIRelationList)
            {
                ClassEditor.CreateRelationEdge(Relation);
            }
        }

        //Auto arrange objects in space
        public void AutoLayout()
        {
            //TODO better automatic Layout
            graph.Layout();
        }

        //Set layout as close as possible to EA layout
        public void ManualLayout()
        {
            foreach (ClassInDiagram c in Classes)
            {
                if (c.isObject)
                {
                    continue;
                }

                c.VisualObject.GetComponent<RectTransform>().position
                    = new Vector3(c.XMIParsedClass.Left * 1.25f, c.XMIParsedClass.Top * 1.25f, c.VisualObject.GetComponent<RectTransform>().position.z);
            }
        }
        //Create GameObjects from the parsed data sotred in list of Classes and Relations
        public void Generate()
        {
            //Render classes
            for (var i = 0; i < Classes.Count; i++)
            {
                if (Classes[i].isObject)
                {
                    continue;
                }

                //Setting up
                var node = FindClassByName(Classes[i].XMIParsedClass.Name).VisualObject;

                var background = node.transform.Find("Background");
                var attributes = background.Find("Attributes");
                
                //Attributes
                if (Classes[i].XMIParsedClass.Attributes != null)
                    foreach (Attribute attr in Classes[i].XMIParsedClass.Attributes)
                    {
                        attributes.GetComponent<TextMeshProUGUI>().text += attr.Name + ": " + attr.Type + "\n";
                    }

            }

            //Render Relations between classes
            foreach (RelationInDiagram rel in Relations)
            {
                GameObject prefab = rel.XMIParsedRelation.PrefabType;
                if (prefab == null)
                {
                    prefab = DiagramPool.Instance.associationNonePrefab;
                }

                GameObject startingClass = FindClassByName(rel.XMIParsedRelation.FromClass)?.VisualObject;
                GameObject finishingClass = FindClassByName(rel.XMIParsedRelation.ToClass)?.VisualObject;
                if (startingClass != null && finishingClass != null)
                {
                     GameObject edge = graph.AddEdge(startingClass, finishingClass, prefab);

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
                        string.Format("Can't find specified Edge \"{0}\"->\"{1}\"", rel.XMIParsedRelation.FromClass, rel.XMIParsedRelation.ToClass)
                    );
                }
            }
        }

        public ClassInDiagram FindClassByName(String className)
        {
            return Classes
                    .Where(classInDiagram => string.Equals(className, classInDiagram.XMIParsedClass.Name))
                    .IfMoreThan
                    (
                        count => Debug.LogError
                        (
                            string.Format
                            (
                                "More than 1 class named \"{0}\" found in ClassDiagram::Classes",
                                className
                            )
                        )
                    )
                    .FirstOrDefault();
        }
        
        public Method FindMethodByName(String className, String methodName)
        {
            ClassInDiagram _class = FindClassByName(className);
            if (_class == null)
            {
                return null;
            }

            return _class
                    .XMIParsedClass
                    .Methods
                    .Where(methodInClassDiagram => string.Equals(methodName, _class.XMIParsedClass.Name))
                    .IfMoreThan
                    (
                        count => Debug.LogError
                        (
                            string.Format
                            (
                                "More than 1 method named \"{0}::{1}\" found in ClassDiagram::Classes",
                                className,
                                methodName
                            )
                        )
                    )
                    .FirstOrDefault();
        }

        public Attribute FindAttributeByName(String className, String attributeName)
        {
            ClassInDiagram _class = FindClassByName(className);
            if (_class == null)
            {
                return null;
            }

            return _class
                    .XMIParsedClass
                    .Attributes
                    .Where(attributeInClassDiagram => string.Equals(attributeName, _class.XMIParsedClass.Name))
                    .IfMoreThan
                    (
                        count => Debug.LogError
                        (
                            string.Format
                            (
                                "More than 1 attribute named \"{0}::{1}\" found in ClassDiagram::Classes",
                                className,
                                attributeName
                            )
                        )
                    )
                    .FirstOrDefault();
        }
        
        public GameObject FindNode(String name)
        {
            GameObject g;
            g = FindClassByName(name).VisualObject;
            return g;
        }
        public GameObject FindEdge(string classA, string classB)
        {
            GameObject Result = null;

            CDRelationship Rel = OALProgram.Instance.RelationshipSpace.GetRelationshipByClasses(classA, classB);
            if (Rel != null)
            {
                Result = FindEdge(Rel.RelationshipName);
            }
            return Result;
        }
        
        public GameObject FindEdge(string RelationshipName)
        {
            return Relations
                .FirstOrDefault(relation => string.Equals(RelationshipName, relation.RelationInfo.RelationshipName))?
                .VisualObject;
        }
        
        public RelationInDiagram FindEdgeInfo(string RelationshipName)
        {
            return Relations
                .FirstOrDefault(relation => string.Equals(RelationshipName, relation.RelationInfo.RelationshipName));
        }
        
        public String FindOwnerOfRelation(String RelationName)
        {
            return Relations
                .Where(relation => string.Equals(RelationName, relation.XMIParsedRelation.OALName))
                .FirstOrCustomDefault<RelationInDiagram, string>(relationInDiagram => relationInDiagram.XMIParsedRelation.FromClass, "");
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
            return Classes.Select(classInDiagram => classInDiagram.XMIParsedClass);
        }
        public IEnumerable<Relation> GetRelationList()
        {
            return Relations.Select(relationInDiagram => relationInDiagram.XMIParsedRelation);
        }

        
        // Lukas
        public class DiagramObject
        {
            public string name;
            public string className;
            public GameObject VisualObject;
            // Name, Type, Value
            public List<(string, string, string)> Attributes;

            public DiagramObject(string name, string className, List<(string, string, string)> Attributes)
            {
                this.name = name;
                this.className = className;
                VisualObject = null;
                this.Attributes = Attributes;
            }
        }
        // Lukas
        public void AddDiagramObject(DiagramObject DiagramObject)
        {
            DiagramObject.VisualObject = graph.AddNode();
            DiagramObject.VisualObject.name = DiagramObject.name + ":" + DiagramObject.className;
            DiagramObject.VisualObject.transform.Find("Background").Find("Header").GetComponent<TextMeshProUGUI>().text = DiagramObject.VisualObject.name;
            DiagramObject.VisualObject.GetComponent<BackgroundHighlighter>().GetComponentInChildren<Image>().color = Color.cyan;

            foreach ((string, string, string) attr in DiagramObject.Attributes)
            {
                DiagramObject.VisualObject.transform.Find("Background").Find("Attributes").GetComponent<TextMeshProUGUI>().text
                    += attr.Item1 + " : " + string.Format("{0}", attr.Item3) + "\n";
            }

            Classes.Add
            (
                new ClassInDiagram()
                {
                    VisualObject = DiagramObject.VisualObject,
                    XMIParsedClass = new Class() { Name = DiagramObject.VisualObject.name},
                    isObject = true
                }
            );
        }
    }
}