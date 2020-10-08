using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using System.Xml;
using OALProgramControl;

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
    private List<Class> DiagramClasses; //List of all classes from XMI
    private List<Relation> DiagramRelations; //List of all relations from XMI
    private Dictionary<string, GameObject> GameObjectRelations; // Dictionary of all objects created from list classes
    private Dictionary<string, GameObject> GameObjectClasses; //Dictionary of all ojects created from relations list

    //Awake is called before the first frame and before Start()
    private void Awake()
    {
        //Asign memory for variables before the first frame
        GameObjectClasses = new Dictionary<string, GameObject>();
        DiagramClasses = new List<Class>();
        GameObjectRelations = new Dictionary<string, GameObject>();
        DiagramRelations = new List<Relation>();
    }
    private void Start()
    {
        //Atempt to load diagram at the start of the application
        LoadDiagram();
    }
    public void LoadDiagram()
    {
        var go = GameObject.Instantiate(graphPrefab);
        graph = go.GetComponent<Graph>();

        //Call parser to load data from specified path to 
        ParseData();
        //Generate UI objects displaying the diagram
        Generate();
        //Set the layout of diagram so it is coresponding to EA view
        ManualLayout();
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
                }
            }

            if (CurrentClass.Methods != null)
            {    
                foreach (Method CurrentMethod in CurrentClass.Methods)
                {
                    CurrentMethod.Name = CurrentMethod.Name.Replace(" ", "_");
                    TempCDClass.AddMethod(new CDMethod(CurrentMethod.Name, EXETypes.ConvertEATypeName(CurrentMethod.ReturnValue)));
                }
            }
            CurrentClass.Top *= -1;
            DiagramClasses.Add(CurrentClass);
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
            Debug.Log(Relation.PropertiesEa_type);
            Debug.Log(Relation.ProperitesDirection);
            //Here you assign prefabs for each type of relation
            switch (Relation.PropertiesEa_type)
            {
                case "Association": switch (Relation.ProperitesDirection)
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
    public void AutoLayout()
    {
        //TODO better automatic Layout
        graph.Layout();
    }

    //Set layout as close as possible to EA layout
    public void ManualLayout()
    {
        foreach (Class c in DiagramClasses)
        {
            GameObjectClasses[c.Name].GetComponent<RectTransform>().position = new Vector3(c.Left*1.25f, c.Top*1.25f);
        }
    }
    //Create GameObjects from the parsed data sotred in list of Classes and Relations
    private void Generate()
    {
        //Render classes
        for (int i = 0; i < DiagramClasses.Count; i++)
        {
            //Setting up
            var node = graph.AddNode();
            node.name = DiagramClasses[i].Name;
            var background = node.transform.Find("Background");
            var header = background.Find("Header");
            var attributes = background.Find("Attributes");
            var methods = background.Find("Methods");

            // Printing the values into diagram
            header.GetComponent<TextMeshProUGUI>().text = DiagramClasses[i].Name;

            //Attributes
            if (DiagramClasses[i].Attributes != null)
                foreach (Attribute attr in DiagramClasses[i].Attributes)
                {
                    attributes.GetComponent<TextMeshProUGUI>().text += attr.Name + ": " + attr.Type + "\n";
                }


            //Methods
            if (DiagramClasses[i].Methods != null)
                foreach (Method method in DiagramClasses[i].Methods)
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
            GameObjectClasses.Add(node.name, node);
            Debug.Log(node.name);

        }

        //Render Relations between classes
        foreach (Relation rel in DiagramRelations)
        {
            GameObject prefab = rel.PrefabType;
            if (prefab == null)
            {
                prefab = associationNonePrefab;
                Debug.Log("Unknown prefab");
            }

            if (GameObjectClasses[rel.FromClass] != null && GameObjectClasses[rel.ToClass] != null)
            {
                GameObject edge = graph.AddEdge(GameObjectClasses[rel.FromClass], GameObjectClasses[rel.ToClass], prefab);
                //Add relation node to dictionary
                //GameObjectRelations.Add(rel.FromClass + "/" + rel.ToClass, edge);
                //RELADD
                GameObjectRelations.Add(rel.OALName, edge);
                //Quickfix
                if(edge.gameObject.transform.childCount>0)
                    StartCoroutine(QuickFix(edge.transform.GetChild(0).gameObject));
            }
            else
                Debug.Log("Cant find specified Edge in Dictionary");
        }
    }

    public Class FindClassByName(String searchedClass)
    {
        Class result=null;
        foreach(Class c in DiagramClasses)
        {
            
            if (c.Name.Equals(searchedClass))
            {
                result = c;
                Debug.Log("result:"+c.Name);
                return result;
            }
        }
        Debug.Log("Class " + searchedClass+ " not found");
        return result;
    }
    public Method FindMethodByName(String searchedClass,String searchedMethod)
    {
        Method result = null;
        Class c = FindClassByName(searchedClass);
        if(c==null)
            return null;
        foreach(Method m in c.methods)
        {
            if (m.Name.Equals(searchedMethod))
            {
                result = m;
                return result;
            }
        }
        Debug.Log("Method " + searchedMethod + "not found");
        return result;
    }
    public GameObject FindNode(String name)
    {
        GameObject g;
        g = GameObjectClasses[name];
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
    public List<Class> GetClassList()
    {
        return DiagramClasses;
    }
    public List<Relation> GetRelationList()
    {
        return DiagramRelations;
    }

}
