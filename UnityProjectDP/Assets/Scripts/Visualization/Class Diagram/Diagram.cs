using OALProgramControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace AnimArch.Visualization.ClassDiagram
{
    public class Diagram : Singleton<Diagram>
    {
        public List<ClassDiagramLayer> DiagramLayers { get; private set; } = new List<ClassDiagramLayer>();

        private void Awake()
        {
            Clear();
        }
        public Graph CreateNewGraph()
        {
            Clear();
            return CreateGraph();
        }
        public Graph CreateGraph()
        {
            ClassDiagramLayer NewDiagramLayer = new ClassDiagramLayer
            {
                Diagram = GameObject.Instantiate(ClassDiagramPalette.Instance.graphPrefab).GetComponent<Graph>()
            };

            DiagramLayers.Add(NewDiagramLayer);

            return NewDiagramLayer.Diagram;
        }
        private void Clear()
        {
            DiagramLayers.ForEach((Layer) => Layer.ManipulateGameObject(Destroy));
            OALProgram.Instance.ExecutionSpace.ClassPool.Clear();
            OALProgram.Instance.RelationshipSpace.RelationshipPool.Clear();
            AnimationData.Instance.ClearData();
        }
        // Legacy
        public void LoadDiagram()
        {
            CreateGraph();

            ClassDiagramLayer CurrentLayer = DiagramLayers.LastOrDefault();
            //Call parser to load data from specified path to 
            int k = 0;
            // A trick used to skip empty diagrams in XMI file from EA
            while (CurrentLayer.Classes.Count < 1 && k < 10)
            {
                ParseData();
                k++;
                AnimationData.Instance.diagramId++;
            }
            //Generate UI objects displaying the diagram
            Generate();

            //fakeObjects();
            //Set the layout of diagram so it is coresponding to EA view
            ManualLayout();
        }
        // Legacy
        void ParseData()
        {
            ClassDiagramLayer CurrentLayer = DiagramLayers.LastOrDefault();

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
                        TempCDClass.AddMethod(new CDMethod(CurrentMethod.Name, EXETypes.ConvertEATypeName(CurrentMethod.ReturnValue)));
                    }
                }
                CurrentClass.Top *= -1;
                CurrentLayer.AddClass(CurrentClass, null);
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
                            case "Source -> Destination": Relation.PrefabType   = ClassDiagramPalette.Instance.associationSDPrefab; break;
                            case "Destination -> Source": Relation.PrefabType   = ClassDiagramPalette.Instance.associationDSPrefab; break;
                            case "Bi-Directional": Relation.PrefabType          = ClassDiagramPalette.Instance.associationFullPrefab; break;
                            default: Relation.PrefabType                        = ClassDiagramPalette.Instance.associationNonePrefab; break;
                        }
                        break;
                    case "Generalization":  Relation.PrefabType = ClassDiagramPalette.Instance.generalizationPrefab; break;
                    case "Dependency":      Relation.PrefabType = ClassDiagramPalette.Instance.dependsPrefab; break;
                    case "Realisation":     Relation.PrefabType = ClassDiagramPalette.Instance.realisationPrefab; break;
                    default:                Relation.PrefabType = ClassDiagramPalette.Instance.associationNonePrefab; break;
                }

                TempCDRelationship = OALProgram.Instance.RelationshipSpace.SpawnRelationship(Relation.FromClass, Relation.ToClass);
                Relation.OALName = TempCDRelationship.RelationshipName;

                CurrentLayer.AddRelation(Relation, null);
            }
        }
        // Legacy
        private void Generate()
        {
            ClassDiagramLayer CurrentLayer = DiagramLayers.LastOrDefault();

            Debug.Log("DIAGRAM CLASSES COUNT" + CurrentLayer.Classes.Count);
            Debug.Log("RELATION COUNT" + CurrentLayer.Relations.Count);
            //Render classes
            for (int i = 0; i < CurrentLayer.Classes.Values.Count; i++)
            {
                Class CurrentClass = CurrentLayer.Classes.Values.ToList()[i].DataObject;

                //Setting up
                var node = CurrentLayer.Diagram.AddNode();
                node.name = CurrentClass.Name;
                var background = node.transform.Find("Background");
                var header = background.Find("Header");
                var attributes = background.Find("Attributes");
                var methods = background.Find("Methods");

                // Printing the values into diagram
                header.GetComponent<TextMeshProUGUI>().text = CurrentClass.Name;

                //Attributes
                if (CurrentClass.Attributes != null)
                    foreach (Attribute attr in CurrentClass.Attributes)
                    {
                        attributes.GetComponent<TextMeshProUGUI>().text += attr.Name + ": " + attr.Type + "\n";
                    }


                //Methods
                if (CurrentClass.Methods != null)
                    foreach (Method method in CurrentClass.Methods)
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
                CurrentLayer.Classes[node.name].VisualObject = node;
                //Debug.Log(node.name);

            }

            //Render Relations between classes
            foreach (Relation rel in CurrentLayer.Relations.Values.Select((x) => x.DataObject))
            {
                GameObject prefab = rel.PrefabType;
                if (prefab == null)
                {
                    prefab = ClassDiagramPalette.Instance.associationNonePrefab;
                    Debug.Log("Unknown prefab");
                }
                DiagramElement<Class> g;
                if (CurrentLayer.Classes.TryGetValue(rel.FromClass, out g) && CurrentLayer.Classes.TryGetValue(rel.ToClass, out g))
                {
                    GameObject edge = CurrentLayer.Diagram.AddEdge(CurrentLayer.Classes[rel.FromClass].VisualObject, CurrentLayer.Classes[rel.ToClass].VisualObject, prefab);
                    //Add relation node to dictionary
                    //GameObjectRelations.Add(rel.FromClass + "/" + rel.ToClass, edge);
                    //RELADD
                    CurrentLayer.Relations[ClassDiagramLayer.RelationName(rel)].VisualObject = edge;
                    //Quickfix
                    if (edge.gameObject.transform.childCount > 0)
                        StartCoroutine(QuickFix(edge.transform.GetChild(0).gameObject));
                }
                else
                    Debug.Log("Cant find specified Edge in Dictionary");
            }
        }
        // Legacy
        private IEnumerator QuickFix(GameObject g)
        {
            yield return new WaitForSeconds(0.05f);
            g.SetActive(false);
            yield return new WaitForSeconds(0.05f);
            g.SetActive(true);
        }
        // Legacy
        public void ManualLayout()
        {
            ClassDiagramLayer CurrentLayer = DiagramLayers.LastOrDefault();
            CurrentLayer.Classes.Values.Select(x => x.VisualObject.GetComponent<RectTransform>().position = new Vector3(x.DataObject.Left * 1.25f, x.DataObject.Top * 1.25f));
        }
        // Legacy
        public void AutoLayout()
        {
            ClassDiagramLayer CurrentLayer = DiagramLayers.LastOrDefault();
            CurrentLayer.Diagram.Layout();
        }
        // Legacy
        public void CreateRelationEdge(GameObject node1, GameObject node2)
        {
            ClassDiagramLayer CurrentLayer = DiagramLayers.LastOrDefault();
            GameObject edge = CurrentLayer.Diagram.AddEdge(node1, node2, ClassDiagramPalette.Instance.associationFullPrefab);
        }
        // Legacies all the way down
        public Class FindClassByName(String searchedClass)
        {
            Class result = null;
            foreach (Class c in DiagramLayers.LastOrDefault().Classes.Values.Select((x) => x.DataObject))
            {
                if (c.Name.Equals(searchedClass))
                {
                    result = c;
                    Debug.Log("result:" + c.Name);
                    return result;
                }
            }

            Debug.Log("Class " + searchedClass + " not found");

            return result;
        }
        public Method FindMethodByName(String searchedClass, String searchedMethod)
        {
            Method result = null;
            Class c = FindClassByName(searchedClass);
            if (c == null)
                return null;
            if (c.Methods == null)
            {
                return null;
            }
            foreach (Method m in c.Methods)
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
        public bool AddMethod(String targetClass, Method methodToAdd)
        {
            Class c = FindClassByName(targetClass);
            if (c == null)
                return false;
            else
            {
                if (FindMethodByName(targetClass, methodToAdd.Name) == null)
                {
                    if (c.Methods == null)
                    {
                        c.Methods = new List<Method>();
                    }
                    c.Methods.Add(methodToAdd);
                    if (OALProgram.Instance.ExecutionSpace.ClassExists(targetClass))
                    {
                        OALProgram.Instance.ExecutionSpace.getClassByName(targetClass).AddMethod(new CDMethod(methodToAdd.Name, methodToAdd.ReturnValue));
                    }

                }
                else
                {
                    return false;
                }

            }
            return true;
        }
        public Attribute FindAttributeByName(String searchedClass, String attribute)
        {
            Attribute result = null;
            Class c = FindClassByName(searchedClass);
            if (c == null)
                return null;
            if (c.Attributes == null)
            {
                return null;
            }
            foreach (Attribute atr in c.Attributes)
            {
                if (atr.Name.Equals(attribute))
                {
                    result = atr;
                    return result;
                }
            }
            Debug.Log("Method " + attribute + "not found");
            return result;
        }
        public bool AddAtr(String targetClass, Attribute atr)
        {
            Class c = FindClassByName(targetClass);
            if (c == null)
                return false;
            else
            {
                if (FindAttributeByName(targetClass, atr.Name) == null)
                {
                    if (c.Attributes == null)
                    {
                        c.Attributes = new List<Attribute>();
                    }
                    c.Attributes.Add(atr);
                }
                else return false;

            }
            return true;
        }
        public GameObject FindNode(String name)
        {
            GameObject g;
            g = DiagramLayers.LastOrDefault().Classes[name].VisualObject;
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
            if (DiagramLayers.LastOrDefault().Relations.ContainsKey(RelationshipName))
            {
                Result = DiagramLayers.LastOrDefault().Relations[RelationshipName].VisualObject;
            }
            return Result;
        }
        public String FindOwnerOfRelation(String RelationName)
        {
            foreach (Relation Relation in DiagramLayers.LastOrDefault().Relations.Values.Select((x) => x.DataObject))
            {
                if (String.Equals(Relation.OALName, RelationName))
                {
                    return Relation.FromClass;
                }
            }
            return "";
        }
        public List<Class> GetClassList()
        {
            return DiagramLayers.Count == 0 ? new List<Class>() : DiagramLayers.Last().Classes.Values.Select((x) => x.DataObject).ToList();
        }
        public List<Relation> GetRelationList()
        {
            return DiagramLayers.Count == 0 ? new List<Relation>() : DiagramLayers.Last().Relations.Values.Select((x) => x.DataObject).ToList();
        }
    }
}