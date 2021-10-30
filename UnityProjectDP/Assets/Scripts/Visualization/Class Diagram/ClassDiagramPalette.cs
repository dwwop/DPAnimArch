using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml;
using OALProgramControl;

namespace AnimArch.Visualization.ClassDiagram
{
    public class ClassDiagramPalette : Singleton<ClassDiagramPalette>
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

        /*public void fakeObjects()
        {
            List<DiagramObject> dos = new List<DiagramObject>( new DiagramObject [] {
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
                dgo.VisualObject.GetComponent<RectTransform>().position = new Vector3(
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

            GameObjectClasses.Add(DiagramObject.VisualObject.name, DiagramObject.VisualObject);

            GameObject prefab = dependsPrefab;
            GameObject g;
            if (GameObjectClasses.TryGetValue(DiagramObject.VisualObject.name, out g) && GameObjectClasses.TryGetValue(DiagramObject.className, out g))
            {
                GameObject edge = graph.AddEdge(GameObjectClasses[DiagramObject.VisualObject.name], GameObjectClasses[DiagramObject.className], prefab);
                //Add relation node to dictionary
                //GameObjectRelations.Add(rel.FromClass + "/" + rel.ToClass, edge);
                //RELADD
                GameObjectRelations.Add(DiagramObject.VisualObject.name, edge);
                //Quickfix
                if (edge.gameObject.transform.childCount > 0)
                    StartCoroutine(QuickFix(edge.transform.GetChild(0).gameObject));

                edge.GetComponent<UEdge>().GraphEdge.Color = Color.cyan;
            }
            else
                Debug.Log("Cant find specified Edge in Dictionary");
        }*/

        //Auto arrange objects in space
        /*
        public void AutoLayout()
        {
            //TODO better automatic Layout
            graph.Layout();
        }
        
        //Create GameObjects from the parsed data sotred in list of Classes and Relations
        private void Generate()
        {
            Debug.Log("DIAGRAM CLASSES COUNT" + DiagramClasses.Count);
            Debug.Log("RELATION COUNT" + DiagramRelations.Count);
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
                //Debug.Log(node.name);

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
                GameObject g;
                if (GameObjectClasses.TryGetValue(rel.FromClass,out g) && GameObjectClasses.TryGetValue(rel.ToClass, out g))
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
                if (FindMethodByName(targetClass, methodToAdd.Name)==null)
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
        public void CreateRelationEdge(GameObject node1, GameObject node2)
        {
            GameObject edge = graph.AddEdge(node1, node2, associationFullPrefab);
        }*/

    }
}