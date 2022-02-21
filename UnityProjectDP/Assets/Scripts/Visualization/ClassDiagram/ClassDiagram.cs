using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using TMPro;
using System.Xml;
using OALProgramControl;
using UnityEngine.UI;
using AnimArch.Extensions;
using AnimArch.Visualization.Animating;
using AnimArch.Visualization.UI;
using AnimArch.XMIParsing;

namespace AnimArch.Visualization.ClassDiagrams
{
    public class ClassDiagram : Singleton<ClassDiagram>
    {
        public GameObject graphPrefab;
        public GameObject classPrefab;
        public GameObject associationNonePrefab;
        public GameObject associationFullPrefab;
        public GameObject associationSDPrefab;
        public GameObject associationDSPrefab;
        public GameObject dependsPrefab;
        public GameObject generalizationPrefab;
        public GameObject implementsPrefab;
        public GameObject realisationPrefab;
        public Graph graph;
        //private List<Class> DiagramClasses; //List of all classes from XMI
        private List<Relation> DiagramRelations; //List of all relations from XMI
        private Dictionary<string, GameObject> GameObjectRelations; // Dictionary of all objects created from list classes
        //private Dictionary<string, GameObject> GameObjectClasses; //Dictionary of all ojects created from relations list
        public List<ClassInDiagram> Classes { get; private set; }

        //Awake is called before the first frame and before Start()
        private void Awake()
        {
            // Before the first frame.
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

            // Get rid of already rendered relationships in diagram.
            if (GameObjectRelations != null)
            {
                foreach (GameObject RelationshipObject in GameObjectRelations.Values)
                {
                    Destroy(RelationshipObject);
                }

                GameObjectRelations.Clear();
            }
            if (graph != null)
            {
                Destroy(graph.gameObject);
                graph = null;
            }

            if (DiagramRelations != null)
            {
                DiagramRelations.Clear();
            }
            DiagramRelations = new List<Relation>();

            GameObjectRelations = new Dictionary<string, GameObject>();

            Classes = new List<ClassInDiagram>(); 

            OALProgram.Instance.ExecutionSpace = new CDClassPool();
            OALProgram.Instance.RelationshipSpace = new CDRelationshipPool();

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
            //Generate UI objects displaying the diagram
            Generate();

            //Set the layout of diagram so it is coresponding to EA view
            ManualLayout();

            fakeObjects();
        }
        public Graph CreateGraph()
        {
            ResetDiagram();
            var go = GameObject.Instantiate(graphPrefab);
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
                        TempCDClass.AddMethod(new CDMethod(TempCDClass, CurrentMethod.Name, EXETypes.ConvertEATypeName(CurrentMethod.ReturnValue)));
                    }
                }
                CurrentClass.Top *= -1;
                Classes.Add( new ClassInDiagram() { XMIParsedClass = CurrentClass });
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
                Relation.FromClass = Relation.SourceModelName.Replace(" ", "_");
                Relation.ToClass = Relation.TargetModelName.Replace(" ", "_");
                //Here you assign prefabs for each type of relation
                switch (Relation.PropertiesEa_type)
                {
                    case "Association":
                        switch (Relation.ProperitesDirection)
                        {
                            case "Source -> Destination": Relation.PrefabType = associationSDPrefab; break;
                            case "Destination -> Source": Relation.PrefabType = associationDSPrefab; break;
                            case "Bi-Directional": Relation.PrefabType = associationFullPrefab; break;
                            default: Relation.PrefabType = associationNonePrefab; break;
                        }
                        break;
                    case "Generalization": Relation.PrefabType = generalizationPrefab; break;
                    case "Dependency": Relation.PrefabType = dependsPrefab; break;
                    case "Realisation": Relation.PrefabType = realisationPrefab; break;
                    default: Relation.PrefabType = associationNonePrefab; break;
                }

                TempCDRelationship = OALProgram.Instance.RelationshipSpace.SpawnRelationship(Relation.FromClass, Relation.ToClass);
                Relation.OALName = TempCDRelationship.RelationshipName;

                DiagramRelations.Add(Relation);
            }
        }

        //Auto arrange objects in space
        /*public void AutoLayout()
        {
            //TODO better automatic Layout
            graph.Layout();
        }*/

        //Set layout as close as possible to EA layout
        public void ManualLayout()
        {
            foreach (ClassInDiagram c in Classes)
            {
                c.VisualObject.GetComponent<RectTransform>().position
                    = new Vector3(c.XMIParsedClass.Left * 1.25f, c.XMIParsedClass.Top * 1.25f);
            }
        }
        //Create GameObjects from the parsed data sotred in list of Classes and Relations
        private void Generate()
        {
            Debug.Log("DIAGRAM CLASSES COUNT" + Classes.Count);
            Debug.Log("RELATION COUNT" + DiagramRelations.Count);
            //Render classes
            for (int i = 0; i < Classes.Count; i++)
            {
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
            foreach (Relation rel in DiagramRelations)
            {
                GameObject prefab = rel.PrefabType;
                if (prefab == null)
                {
                    prefab = associationNonePrefab;
                }

                GameObject startingClass = FindClassByName(rel.FromClass)?.VisualObject;
                GameObject finishingClass = FindClassByName(rel.ToClass)?.VisualObject;
                if (startingClass != null && finishingClass != null)
                {
                    GameObject edge = graph.AddEdge(startingClass, finishingClass, prefab);

                    GameObjectRelations.Add(rel.OALName, edge);
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
                        string.Format("Can't find specified Edge \"{0}\"->\"{1}\"", rel.FromClass, rel.ToClass)
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
        public bool AddMethod(String targetClass, Method methodToAdd)
        {
            ClassInDiagram c = FindClassByName(targetClass);
            if (c == null)
            {
                return false;
            }

            if (FindMethodByName(targetClass, methodToAdd.Name) == null)
            {
                if (c.XMIParsedClass.Methods == null)
                {
                    c.XMIParsedClass.Methods = new List<Method>();
                }
                c.XMIParsedClass.Methods.Add(methodToAdd);
                if (OALProgram.Instance.ExecutionSpace.ClassExists(targetClass))
                {
                    CDClass CDClass = OALProgram.Instance.ExecutionSpace.getClassByName(targetClass);
                    CDClass.AddMethod(new CDMethod(CDClass, methodToAdd.Name, methodToAdd.ReturnValue));
                }
            }
            else
            {
                return false;
            }

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

            bool result = false;

            if (FindAttributeByName(targetClass, attributeToAdd.Name) == null)
            {
                if (c.XMIParsedClass.Attributes == null)
                {
                    c.XMIParsedClass.Attributes = new List<Attribute>();
                }

                c.XMIParsedClass.Attributes.Add(attributeToAdd);

                result = true;
            }

            return result;
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
            GameObject Result = null;
            if (GameObjectRelations.ContainsKey(RelationshipName))
            {
                Result = GameObjectRelations[RelationshipName];
            }
            return Result;
        }
        public String FindOwnerOfRelation(String RelationName)
        {
            foreach (Relation Relation in DiagramRelations)
            {
                if (String.Equals(Relation.OALName, RelationName))
                {
                    return Relation.FromClass;
                }
            }
            return "";
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
        public List<Relation> GetRelationList()
        {
            return DiagramRelations;
        }
        public void CreateRelationEdge(GameObject node1, GameObject node2)
        {
            GameObject edge = graph.AddEdge(node1, node2, associationFullPrefab);
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

            foreach (DiagramObject dgo in dos)
            {
                AddDiagramObject(dgo);
                dgo.VisualObject.GetComponent<RectTransform>().position
                    = new Vector3
                    (
                        dgo.VisualObject.GetComponent<RectTransform>().position.x,
                        dgo.VisualObject.GetComponent<RectTransform>().position.y,
                        200
                    );
            }
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
                    XMIParsedClass = new Class() { Name = DiagramObject.VisualObject.name}
                }
            );

            GameObject prefab = dependsPrefab;
            GameObject startingClass = FindClassByName(DiagramObject.VisualObject.name)?.VisualObject;
            GameObject finishingClass = FindClassByName(DiagramObject.className)?.VisualObject;
            if (startingClass != null && finishingClass != null)
            {
                GameObject edge = graph.AddEdge(startingClass, finishingClass, prefab);
                GameObjectRelations.Add(DiagramObject.VisualObject.name, edge);

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
            }
        }
    }
}