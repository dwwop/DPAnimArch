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

            AnimationData.Instance.ClearData();
        }
        public void LoadDiagram()
        {
            CreateGraph();
            //Call parser to load data from specified path to 
            int k = 0;
            // A trick used to skip empty diagrams in XMI file from EA
            while (Classes.Count < 1 && k < 10)
            {
                ParseData();
                k++;
                AnimationData.Instance.diagramId++;
            }

            //fakeObjects();

            //Generate UI objects displaying the diagram
            Generate();


            //Set the layout of diagram so it is coresponding to EA view
            ManualLayout();
            //AutoLayout();

            /*Classes
                .Where(Class => Class.isObject)
                .ForEach(Class => Class.VisualObject.GetComponent<RectTransform>().Shift(0, 0, 200));


            ClassInDiagram CLASS = FindClassByName("ASTLeaf");
            Classes
                .Where(Class => Class.isObject)
                .ForEach
                (
                    Class => CreateInterGraphLine(graph, Class.VisualObject, CLASS.VisualObject)
                );*/
            
            // DiagramPool.Instance.ObjectDiagram.LoadDiagram();
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
            foreach (Class CurrentClass in XMIClassList)
            {
                CurrentClass.Name = CurrentClass.Name.Replace(" ", "_");

                TempCDClass = null;
                int i = 0;
                string currentName = CurrentClass.Name;
                string baseName = CurrentClass.Name;
                while (TempCDClass == null)
                {
                    currentName = baseName + (i == 0 ? "" : i.ToString());
                    TempCDClass = OALProgram.Instance.ExecutionSpace.SpawnClass(currentName);
                    i++;
                    if (i > 1000)
                    {
                        break;
                    }
                }
                CurrentClass.Name = currentName;
                if (TempCDClass == null)
                {
                    continue;
                }

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
                        if (CurrentClass.attributes == null)
                        {
                            CurrentClass.attributes = new List<Attribute>();
                        }
                    }
                }

                if (CurrentClass.Methods != null)
                {
                    foreach (Method CurrentMethod in CurrentClass.Methods)
                    {
                        CurrentMethod.Name = CurrentMethod.Name.Replace(" ", "_");
                        CDMethod Method = new CDMethod(TempCDClass, CurrentMethod.Name, EXETypes.ConvertEATypeName(CurrentMethod.ReturnValue));
                        TempCDClass.AddMethod(Method);

                        foreach (string arg in CurrentMethod.arguments)
                        {
                            string[] tokens = arg.Split(' ');
                            string type = tokens[0];
                            string name = tokens[1];

                            Method.Parameters.Add(new CDParameter() { Name = name, Type = EXETypes.ConvertEATypeName(type) });
                        }
                    }
                }
                CurrentClass.Top *= -1;
                Classes.Add( new ClassInDiagram() { XMIParsedClass = CurrentClass, ClassInfo = TempCDClass });
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
                CreateRelationEdge(Relation);
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
            Debug.Log("DIAGRAM CLASSES COUNT" + Classes.Count);
            Debug.Log("RELATION COUNT" + Relations.Count);
            //Render classes
            for (int i = 0; i < Classes.Count; i++)
            {
                if (Classes[i].isObject)
                {
                    continue;
                }

                //Setting up
                var node = graph.AddNode();
                node.name = Classes[i].XMIParsedClass.Name;
                var background = node.transform.Find("Background");
                var header = background.Find("Header");
                var attributes = background.Find("Attributes");
                var methods = background.Find("Methods");

                // Printing the values into diagram
                header.GetComponent<TextMeshProUGUI>().text = Classes[i].XMIParsedClass.Name;

                //Attributes
                if (Classes[i].XMIParsedClass.Attributes != null)
                    foreach (Attribute attr in Classes[i].XMIParsedClass.Attributes)
                    {
                        attributes.GetComponent<TextMeshProUGUI>().text += attr.Name + ": " + attr.Type + "\n";
                    }


                //Methods
                if (Classes[i].XMIParsedClass.Methods != null)
                    foreach (Method method in Classes[i].XMIParsedClass.Methods)
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

                        methods.GetComponent<TextMeshProUGUI>().text += method.Name + arguments + " :" + method.ReturnValue + "\n";
                    }

                //Add Class to Dictionary
                FindClassByName(node.name).VisualObject = node;
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
        public bool AddMethod(string targetClass, Method methodToAdd)
        {
            var c = FindClassByName(targetClass);
            if (c == null)
            {
                return false;
            }
            
            if (c.XMIParsedClass.Methods == null) 
                c.XMIParsedClass.Methods = new List<Method>();
            if (c.XMIParsedClass.methods == null)
                c.XMIParsedClass.methods = new List<Method>();

            if (FindMethodByName(targetClass, methodToAdd.Name) != null) return false;
            
            c.XMIParsedClass.Methods.Add(methodToAdd);
            c.XMIParsedClass.methods.Add(methodToAdd);
            
            if (!OALProgram.Instance.ExecutionSpace.ClassExists(targetClass)) return true;
            
            var cdClass = OALProgram.Instance.ExecutionSpace.getClassByName(targetClass);
            cdClass.AddMethod(new CDMethod(cdClass, methodToAdd.Name, EXETypes.ConvertEATypeName(methodToAdd.ReturnValue)));
        
            return true;
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
        public bool AddAtr(String targetClass, Attribute attributeToAdd)
        {
            ClassInDiagram c = FindClassByName(targetClass);
            if (c == null)
            {
                return false;
            }

            if (c.XMIParsedClass.Attributes == null)
                c.XMIParsedClass.Attributes = new List<Attribute>();

            if (c.XMIParsedClass.attributes == null)
                c.XMIParsedClass.attributes = new List<Attribute>();
            

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
        private IEnumerator QuickFix(GameObject g)
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

        public void CreateRelation(GameObject node1, GameObject node2, string type, bool noDirection = false)
        {
            var relation = new Relation
            {
                SourceModelName = node1.name,
                TargetModelName = node2.name,
                PropertiesEa_type = type,
                ProperitesDirection = noDirection ? "none" : "Source -> Destination"
            };

            var relInDiag = CreateRelationEdge(relation);
            relInDiag.VisualObject = graph.AddEdge(node1, node2, relation.PrefabType);
        }

        private RelationInDiagram CreateRelationEdge(Relation relation)
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
        public void fakeObjects()
        {
            List<DiagramObject> dos = new List<DiagramObject>(new DiagramObject[] {
                new DiagramObject("Operand1", "ASTLeaf", new List<(string, string, string)>( new [] {
                    ("value", "string", "string"),
                    ("type", "string", "\"Due to false \"")
                })),
                new DiagramObject("Operand2", "ASTLeaf", new List<(string, string, string)>( new [] {
                    ("value", "string", "string"),
                    ("type", "string", "\"pandemic narrative.\"")
                }))
            });

            int i = 0;
            foreach (DiagramObject dgo in dos)
            {
                AddDiagramObject(dgo);
                dgo.VisualObject.GetComponent<RectTransform>().position
                    = new Vector3
                    (
                        0,
                        300 * i++
                    );
            }

            /*GameObject Line = Instantiate(interGraphLinePrefab);
            Line.GetComponent<LineRenderer>().SetPositions(
                new Vector3[]
                {
                    dos[0].VisualObject.GetComponent<RectTransform>().position,
                    dos[1].VisualObject.GetComponent<RectTransform>().position
                }
            );
            Line.GetComponent<LineRenderer>().widthMultiplier = 6f;*/
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
            /*
            GameObject prefab = dependsPrefab;
            GameObject startingClass = FindClassByName(DiagramObject.VisualObject.name)?.VisualObject;
            GameObject finishingClass = FindClassByName(DiagramObject.className)?.VisualObject;
            if (startingClass != null && finishingClass != null)
            {
                GameObject edge = graph.AddEdge(startingClass, finishingClass, prefab);
                Relations.Add
                (
                    new RelationInDiagram()
                    {
                        VisualObject = edge,
                        XMIParsedRelation
                            = new Relation()
                            {
                                OALName = DiagramObject.VisualObject.name,
                                FromClass = startingClass.name,
                                ToClass = finishingClass.name
                            },
                        RelationInfo
                            = new CDRelationship
                            (
                                startingClass.name,
                                finishingClass.name,
                                DiagramObject.VisualObject.name
                            )
                    }
                );

                //Quickfix
                if (edge.gameObject.transform.childCount > 0)
                {
                    StartCoroutine(QuickFix(edge.transform.GetChild(0).gameObject));
                }

                edge.GetComponent<UEdge>().GraphEdge.Color = Color.cyan;
            }
            else
            {
                Debug.LogError
                (
                    string.Format
                    (
                        "Can't find specified Edge \"{0}\"->\"{1}\"",
                        DiagramObject.VisualObject.name,
                        DiagramObject.className
                    )
                );
            }*/
        }
    }
}