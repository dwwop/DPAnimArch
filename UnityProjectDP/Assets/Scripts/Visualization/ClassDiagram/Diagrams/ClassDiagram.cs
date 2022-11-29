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
using JetBrains.Annotations;

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
            var go = GameObject.Instantiate(DiagramPool.Instance.graphPrefab);
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
            foreach (Class currentClass in XMIClassList)
            {
                currentClass.Name = currentClass.Name.Replace(" ", "_");

                TempCDClass = ClassEditor.Instance.CreateNode(currentClass);
                if (TempCDClass == null)
                    continue;

                if (currentClass.Attributes != null)
                {
                    foreach (Attribute attribute in currentClass.Attributes)
                    {
                        attribute.Name = attribute.Name.Replace(" ", "_");
                        string AttributeType = EXETypes.ConvertEATypeName(attribute.Type);
                        if (AttributeType == null)
                        {
                            continue;
                        }

                        TempCDClass.AddAttribute(new CDAttribute(attribute.Name, EXETypes.ConvertEATypeName(AttributeType)));
                        if (currentClass.attributes == null)
                        {
                            currentClass.attributes = new List<Attribute>();
                        }

                        ClassEditor.AddAttribute(currentClass.Name, attribute, false);
                    }
                }

                if (currentClass.Methods != null)
                {
                    foreach (Method method in currentClass.Methods)
                    {
                        method.Name = method.Name.Replace(" ", "_");
                        CDMethod Method = new CDMethod(TempCDClass, method.Name, EXETypes.ConvertEATypeName(method.ReturnValue));
                        TempCDClass.AddMethod(Method);

                        foreach (string arg in method.arguments)
                        {
                            string[] tokens = arg.Split(' ');
                            string type = tokens[0];
                            string name = tokens[1];

                            Method.Parameters.Add(new CDParameter() { Name = name, Type = EXETypes.ConvertEATypeName(type) });
                        }
                        ClassEditor.AddMethod(currentClass.Name, method, ClassEditor.Source.loader);
                    }
                }
                currentClass.Top *= -1;
            }

            List<Relation> XMIRelationList = XMIParser.ParseRelations();
            if (XMIRelationList == null)
            {
                XMIRelationList = new List<Relation>();
            }

            CDRelationship TempCDRelationship;

            //Parse all Relations between classes
            foreach (Relation relation in XMIRelationList)
            {
                CreateRelationEdge(relation);
                ClassEditor.CreateRelation(relation);
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
            foreach (ClassInDiagram classInDiagram in Classes)
            {
                var x = classInDiagram.XMIParsedClass.Left * 1.25f;
                var y = classInDiagram.XMIParsedClass.Top * 1.25f;
                var z = classInDiagram.VisualObject.GetComponent<RectTransform>().position.z;
                ClassEditor.Instance.SetPosition(classInDiagram.XMIParsedClass.Name, new Vector3( x, y, z ), false);
            }
        }
        //Create GameObjects from the parsed data sotred in list of Classes and Relations
        public void Generate()
        {

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
                    //Quickfix

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
                    .methods
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
                    .attributes
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
        public bool AddAtr(string targetClass, Attribute attributeToAdd)
        {
            var c = FindClassByName(targetClass);
            if (c == null)
            {
                return false;
            }

            if (c.XMIParsedClass.Attributes == null)
                c.XMIParsedClass.Attributes = new List<Attribute>();

            if (c.XMIParsedClass.attributes == null)
                c.XMIParsedClass.attributes = new List<Attribute>();


            attributeToAdd.Id = (c.XMIParsedClass.attributes.Count + 1).ToString();
            if (FindAttributeByName(targetClass, attributeToAdd.Name) != null) return false;
            c.XMIParsedClass.Attributes.Add(attributeToAdd);
            c.XMIParsedClass.attributes.Add(attributeToAdd);
            return true;
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
                .Where(relation => string.Equals(RelationshipName, relation.RelationInfo.RelationshipName))
                .FirstOrDefault()?
                .VisualObject;
        }
        
        public RelationInDiagram FindEdgeInfo(string RelationshipName)
        {
            return Relations
                .Where(relation => string.Equals(RelationshipName, relation.RelationInfo.RelationshipName))
                .FirstOrDefault();
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

        public RelationInDiagram CreateRelationEdge(Relation relation)
        {
            relation.FromClass = relation.SourceModelName.Replace(" ", "_");
            relation.ToClass = relation.TargetModelName.Replace(" ", "_");
            
            switch (relation.PropertiesEa_type)
            {
                case "Association":
                    switch (relation.ProperitesDirection)
                    {
                        case "Source -> Destination": relation.PrefabType = DiagramPool.Instance.associationSDPrefab; break;
                        case "Destination -> Source": relation.PrefabType = DiagramPool.Instance.associationDSPrefab; break;
                        case "Bi-Directional": relation.PrefabType = DiagramPool.Instance.associationFullPrefab; break;
                        default: relation.PrefabType = DiagramPool.Instance.associationNonePrefab; break;
                    }
                    break;
                case "Generalization": relation.PrefabType = DiagramPool.Instance.generalizationPrefab; break;
                case "Dependency": relation.PrefabType = DiagramPool.Instance.dependsPrefab; break;
                case "Realisation": relation.PrefabType = DiagramPool.Instance.realisationPrefab; break;
                default: relation.PrefabType = DiagramPool.Instance.associationNonePrefab; break;
            }

            var tempCdRelationship = OALProgram.Instance.RelationshipSpace.SpawnRelationship(relation.FromClass, relation.ToClass) 
                                     ?? throw new ArgumentNullException(nameof(relation));
            relation.OALName = tempCdRelationship.RelationshipName;

            if ("Generalization".Equals(relation.PropertiesEa_type) || "Realisation".Equals(relation.PropertiesEa_type))
            {
                var fromClass = OALProgram.Instance.ExecutionSpace.getClassByName(relation.FromClass);
                var toClass = OALProgram.Instance.ExecutionSpace.getClassByName(relation.ToClass);
                
                if (fromClass != null && toClass != null)
                {
                    fromClass.SuperClass = toClass;
                }
            }
            var relInDiag = new RelationInDiagram { XMIParsedRelation = relation, RelationInfo = tempCdRelationship };
            Relations.Add(relInDiag);
            return relInDiag;
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
    }
}