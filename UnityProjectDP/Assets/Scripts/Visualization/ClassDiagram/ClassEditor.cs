using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AnimArch.Extensions;
using AnimArch.Parsing;
using AnimArch.Visualization.Animating;
using AnimArch.Visualization.UI;
using Networking;
using OALProgramControl;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace AnimArch.Visualization.Diagrams
{
    public class ClassEditor : Singleton<ClassEditor>
    {

        // private void Awake()
        // {
        //     DontDestroyOnLoad(gameObject);
        //     InitializeCreation();
        // }

        public enum Source
        {
            RPC,
            editor,
            loader
        }

        public void InitializeCreation()
        {
            if (DiagramPool.Instance.ClassDiagram.graph) return;

            ClassDiagramBuilder.CreateGraph();
        }

        public CDClass CreateNode(Class newClass)
        {
            Spawner.Instance.SpawnClass(newClass.Name);
            return GenerateNode(newClass);
        }

        public void CreateNodeFromRpc(string name)
        {
            var newClass = new Class
            {
                Name = name,
                // XmiId = _id.ToString(),
                Type = "uml:Class",
                Attributes = new List<Attribute>(),
                Methods = new List<Method>()
            };
            GenerateNode(newClass);
        }


        public CDClass GenerateNode(Class newClass)
        {
            MainEditor.AddNode(newClass);
            return DiagramPool.Instance.ClassDiagram.FindClassByName(newClass.Name).ClassInfo;
            // if (!DiagramPool.Instance.ClassDiagram.graph)
            //     InitializeCreation();
            //
            // CDClass tempCdClass = null;
            // var name = CurrentClassName(newClass.Name, ref tempCdClass);
            // if (tempCdClass == null)
            //     return null;
            //
            // var classInDiagram = new ClassInDiagram { ParsedClass = newClass, ClassInfo = tempCdClass };
            // DiagramPool.Instance.ClassDiagram.Classes.Add(classInDiagram);
            // var node = AddClassToGraph(name);
            // SetPosition(node);
            // if (classInDiagram.ParsedClass.Left == 0)
            // {
            //     SetClassGeometry(classInDiagram);
            // }
            //
            // return tempCdClass;
        }

        public static void SetClassGeometry(ClassInDiagram classInDiagram)
        {
            var position = classInDiagram.VisualObject.transform.localPosition;
            classInDiagram.ParsedClass.Left = position.x / 2.5f;
            classInDiagram.ParsedClass.Top = position.y / 2.5f;
        }

        public static void ReverseClassesGeometry()
        {
            foreach (var parsedClass in DiagramPool.Instance.ClassDiagram.GetClassList())
            {
                parsedClass.Top *= -1;
            }
        }

        private static string CurrentClassName(string name, ref CDClass TempCdClass)
        {
            TempCdClass = null;
            var i = 0;
            var currentName = name;
            var baseName = name;
            while (TempCdClass == null)
            {
                currentName = baseName + (i == 0 ? "" : i.ToString());
                TempCdClass = OALProgram.Instance.ExecutionSpace.SpawnClass(currentName);
                i++;
                if (i > 1000)
                {
                    break;
                }
            }

            return currentName;
        }

        private static void SetPosition(GameObject node)
        {
            var rect = node.GetComponent<RectTransform>();
            rect.position = new Vector3(100f, 200f, 1);
        }

        private GameObject AddClassToGraph(string name)
        {
            var currentClass = DiagramPool.Instance.ClassDiagram.Classes.Find(item => item.ParsedClass.Name == name)
                .ParsedClass;
            var node = DiagramPool.Instance.ClassDiagram.graph.AddNode();
            node.name = name;

            SetClassTmProName(node, name);
            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(name);
            if (classInDiagram == null) return node;

            classInDiagram.VisualObject = node;
            return node;
        }

        // Parser used to parse data from XML to C# data structures

        public static void CreateRelation(Relation relation)
        {
            Spawner.Instance.AddRelation(relation.FromClass, relation.ToClass, relation.PropertiesEaType);
        }

        public void CreateRelation(string sourceClass, string destinationClass, string relationType, bool fromRpc,
            bool noDirection = false)
        {
            if (!fromRpc)
                Spawner.Instance.AddRelation(sourceClass, destinationClass, relationType);
            var relation = new Relation
            {
                SourceModelName = sourceClass,
                TargetModelName = destinationClass,
                PropertiesEaType = relationType,
                PropertiesDirection = noDirection ? "none" : "Source -> Destination"
            };

            var relInDiag = CreateRelationEdge(relation);
            var sourceClassGo = DiagramPool.Instance.ClassDiagram.FindClassByName(sourceClass).VisualObject;
            var destinationClassGo = DiagramPool.Instance.ClassDiagram.FindClassByName(destinationClass).VisualObject;
            var edge = DiagramPool.Instance.ClassDiagram.graph
                .AddEdge(sourceClassGo, destinationClassGo, relation.PrefabType);
            relInDiag.VisualObject = edge;
        }

        public static RelationInDiagram CreateRelationEdge(Relation relation)
        {
            relation.FromClass = relation.SourceModelName.Replace(" ", "_");
            relation.ToClass = relation.TargetModelName.Replace(" ", "_");

            relation.PrefabType = relation.PropertiesEaType switch
            {
                "Association" => relation.PropertiesDirection switch
                {
                    "Source -> Destination" => DiagramPool.Instance.associationSDPrefab,
                    "Destination -> Source" => DiagramPool.Instance.associationDSPrefab,
                    "Bi-Directional" => DiagramPool.Instance.associationFullPrefab,
                    _ => DiagramPool.Instance.associationNonePrefab
                },
                "Generalization" => DiagramPool.Instance.generalizationPrefab,
                "Dependency" => DiagramPool.Instance.dependsPrefab,
                "Realisation" => DiagramPool.Instance.realisationPrefab,
                _ => DiagramPool.Instance.associationNonePrefab
            };

            var tempCdRelationship =
                OALProgram.Instance.RelationshipSpace.SpawnRelationship(relation.FromClass, relation.ToClass)
                ?? throw new ArgumentNullException(nameof(relation));
            relation.OALName = tempCdRelationship.RelationshipName;

            if ("Generalization".Equals(relation.PropertiesEaType) || "Realisation".Equals(relation.PropertiesEaType))
            {
                var fromClass = OALProgram.Instance.ExecutionSpace.getClassByName(relation.FromClass);
                var toClass = OALProgram.Instance.ExecutionSpace.getClassByName(relation.ToClass);

                if (fromClass != null && toClass != null)
                {
                    fromClass.SuperClass = toClass;
                }
            }

            var relInDiag = new RelationInDiagram { ParsedRelation = relation, RelationInfo = tempCdRelationship };
            DiagramPool.Instance.ClassDiagram.Relations.Add(relInDiag);
            return relInDiag;
        }

        private static Transform GetAttributeLayoutGroup(ClassInDiagram classInDiagram)
        {
            return GetAttributeLayoutGroup(classInDiagram.VisualObject);
        }

        private static Transform GetAttributeLayoutGroup(GameObject classGo)
        {
            return classGo.transform
                .Find("Background")
                .Find("Attributes")
                .Find("AttributeLayoutGroup");
        }
        
        private static Transform GetMethodLayoutGroup(ClassInDiagram classInDiagram)
        {
            return GetMethodLayoutGroup(classInDiagram.VisualObject);
        }

        private static Transform GetMethodLayoutGroup(GameObject classGo)
        {
            return classGo.transform
                .Find("Background")
                .Find("Methods")
                .Find("MethodLayoutGroup");
        }

        private static Transform GetClassHeader(ClassInDiagram classInDiagram)
        {
            return GetClassHeader(classInDiagram.VisualObject);
        }

        private static Transform GetClassHeader(GameObject classGo)
        {
            return classGo.transform.Find("Background").Find("HeaderLayout").Find("Header");
        }

        public static void SetClassName(string targetClass, string newName, bool fromRpc)
        {
            if (!fromRpc)
                Spawner.Instance.SetClassName(targetClass, newName);

            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(targetClass);
            if (classInDiagram == null)
                return;

            classInDiagram.ClassInfo.Name = newName;
            classInDiagram.ParsedClass.Name = newName;
            classInDiagram.VisualObject.name = newName;
            SetClassTmProName(classInDiagram.VisualObject, newName);

            foreach (var relationInDiagram in DiagramPool.Instance.ClassDiagram.Relations)
            {
                if (relationInDiagram.ParsedRelation.FromClass == targetClass)
                {
                    relationInDiagram.ParsedRelation.FromClass = newName;
                    relationInDiagram.ParsedRelation.SourceModelName = newName;
                    relationInDiagram.RelationInfo.FromClass = newName;
                }

                if (relationInDiagram.ParsedRelation.ToClass == targetClass)
                {
                    relationInDiagram.ParsedRelation.ToClass = newName;
                    relationInDiagram.ParsedRelation.TargetModelName = newName;
                    relationInDiagram.RelationInfo.ToClass = newName;
                }
            }

            foreach (var attribute in GetAttributeLayoutGroup(classInDiagram).GetComponents<AttributePopUpManager>())
            {
                attribute.classTxt = GetClassHeader(classInDiagram).GetComponent<TextMeshProUGUI>();
            }

            foreach (var method in GetMethodLayoutGroup(classInDiagram).GetComponents<MethodPopUpManager>())
            {
                method.classTxt = GetClassHeader(classInDiagram).GetComponent<TextMeshProUGUI>();
            }
        }

        private static string GetStringFromMethod(Method method)
        {
            var arguments = "(";
            if (method.arguments != null)
            {
                for (var index = 0; index < method.arguments.Count; index++)
                {
                    if (index < method.arguments.Count - 1)
                        arguments += (method.arguments[index] + ", ");
                    else arguments += (method.arguments[index]);
                }
            }

            arguments += "): ";

            return method.Name + arguments + method.ReturnValue + "\n";
        }

        public static Method GetMethodFromString(string str)
        {
            var method = new Method();
            
            var parts = str.Split(new[] { ": ", "\n" }, StringSplitOptions.None);
            
            var nameAndArguments = parts[0].Split(new[] { "(", ")" }, StringSplitOptions.None);
            method.Name = nameAndArguments[0];
            method.ReturnValue = parts[1];


            method.arguments = nameAndArguments[1].Split(", ").Where(x => x != "").ToList();

            return method;
        }

        public static void AddMethod(string targetClass, Method methodToAdd, Source source)
        {
            switch (source)
            {
                case Source.editor:
                    AddMethod(targetClass, methodToAdd);
                    Spawner.Instance.AddMethod(targetClass, methodToAdd.Name, methodToAdd.ReturnValue);
                    break;
                case Source.RPC:
                    AddMethod(targetClass, methodToAdd);
                    break;
                case Source.loader:
                    Spawner.Instance.AddMethod(targetClass, methodToAdd.Name, methodToAdd.ReturnValue);
                    var node = DiagramPool.Instance.ClassDiagram.FindClassByName(targetClass).VisualObject;
                    AddTmProMethod(node, GetStringFromMethod(methodToAdd));
                    break;
            }
        }

        public static void AddParameters(Method method, CDMethod cdMethod)
        {
            foreach (var argument in method.arguments)
            {
                var tokens = argument.Split(' ');
                var type = tokens[0];
                var name = tokens[1];

                cdMethod.Parameters.Add(new CDParameter { Name = name, Type = EXETypes.ConvertEATypeName(type) });
            }
        }

        private static void AddMethod(string targetClass, Method methodToAdd)
        {
            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(targetClass);
            if (classInDiagram == null)
                return;

            classInDiagram.ParsedClass.Methods ??= new List<Method>();

            methodToAdd.Id = (classInDiagram.ParsedClass.Methods.Count + 1).ToString();

            if (DiagramPool.Instance.ClassDiagram.FindMethodByName(targetClass, methodToAdd.Name) != null)
                return;

            classInDiagram.ParsedClass.Methods.Add(methodToAdd);

            if (!OALProgram.Instance.ExecutionSpace.ClassExists(targetClass))
                return;

            var cdClass = OALProgram.Instance.ExecutionSpace.getClassByName(targetClass);
            var cdMethod = new CDMethod(cdClass, methodToAdd.Name, EXETypes.ConvertEATypeName(methodToAdd.ReturnValue));

            if (methodToAdd.arguments != null)
                AddParameters(methodToAdd, cdMethod);

            cdClass.AddMethod(cdMethod);

            AddTmProMethod(classInDiagram.VisualObject, GetStringFromMethod(methodToAdd));
        }


        public static bool AddAttribute(string targetClass, Attribute attributeToAdd)
        {
            var c = DiagramPool.Instance.ClassDiagram.FindClassByName(targetClass);
            if (c == null)
            {
                return false;
            }

            c.ParsedClass.Attributes ??= new List<Attribute>();

            attributeToAdd.Id = (c.ParsedClass.Attributes.Count + 1).ToString();
            if (DiagramPool.Instance.ClassDiagram.FindAttributeByName(targetClass, attributeToAdd.Name) != null)
                return false;
            c.ParsedClass.Attributes.Add(attributeToAdd);
            return true;
        }

        public static void AddAttribute(string targetClass, Attribute attribute, bool fromRpc)
        {
            if (!fromRpc)
                Spawner.Instance.AddAttribute(targetClass, attribute.Name, attribute.Type);
            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(targetClass);
            var attributeText = attribute.Name + ": " + attribute.Type + "\n";
            AddTmProAttribute(classInDiagram.VisualObject, attributeText);
        }

        public static bool UpdateAttribute(string targetClass, string oldAttribute, Attribute newAttribute)
        {
            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(targetClass);
            if (classInDiagram == null)
                return false;

            if (DiagramPool.Instance.ClassDiagram.FindAttributeByName(targetClass, newAttribute.Name) != null)
                return false;

            var index = classInDiagram.ParsedClass.Attributes.FindIndex(x => x.Name == oldAttribute);
            var formerName = classInDiagram.ParsedClass.Attributes[index].Name;
            var formerType = classInDiagram.ParsedClass.Attributes[index].Type;
            newAttribute.Id = classInDiagram.ParsedClass.Attributes[index].Id;
            classInDiagram.ParsedClass.Attributes[index] = newAttribute;

            var oldAttributeText = formerName + ": " + formerType + "\n";
            var newAttributeText = newAttribute.Name + ": " + newAttribute.Type + "\n";
            UpdateTmProAttribute(classInDiagram.VisualObject, oldAttributeText, newAttributeText);
            return true;
        }

        public static bool UpdateMethod(string targetClass, string oldMethod, Method newMethod)
        {
            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(targetClass);
            if (classInDiagram == null)
                return false;

            if (DiagramPool.Instance.ClassDiagram.FindMethodByName(targetClass, newMethod.Name) != null)
                return false;


            var index = classInDiagram.ParsedClass.Methods.FindIndex(x => x.Name == oldMethod);
            var formerMethodTxt = GetStringFromMethod(classInDiagram.ParsedClass.Methods[index]);
            newMethod.Id = classInDiagram.ParsedClass.Methods[index].Id;
            classInDiagram.ParsedClass.Methods[index] = newMethod;

            var newMethodTxt = GetStringFromMethod(newMethod);
            UpdateTmProMethod(classInDiagram.VisualObject, formerMethodTxt, newMethodTxt);
            return true;
        }

        // called at manual layout
        public static void SetPosition(string className, Vector3 position, bool fromRpc)
        {
            if (!fromRpc)
                Spawner.Instance.SetPosition(className, position);
            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(className);
            if (classInDiagram != null)
            {
                classInDiagram
                    .VisualObject
                    .GetComponent<RectTransform>()
                    .position = position;
            }
        }

        private static void SetClassTmProName(GameObject classGo, string name)
        {
            GetClassHeader(classGo).GetComponent<TextMeshProUGUI>().text = name;
        }

        private static void AddTmProMethod(GameObject classGo, string method)
        {
            var attributesTransform = GetMethodLayoutGroup(classGo);

            var instance = Instantiate(DiagramPool.Instance.classMethodPrefab, attributesTransform, false);
            instance.name = method;
            instance.transform.Find("MethodText").GetComponent<TextMeshProUGUI>().text += method;

            instance.GetComponent<MethodPopUpManager>().classTxt =
                GetClassHeader(classGo).GetComponent<TextMeshProUGUI>();

            if (UIEditorManager.Instance.active)
                instance.GetComponentsInChildren<Button>(includeInactive: true)
                    .ForEach(x => x.gameObject.SetActive(true));
        }

        private static void UpdateTmProMethod(GameObject classGo, string oldMethodText, string newMethodText)
        {
            var oldMethod = GetMethodLayoutGroup(classGo).Find(oldMethodText);

            oldMethod.name = newMethodText;
            oldMethod.Find("MethodText").GetComponent<TextMeshProUGUI>().text = newMethodText;
        }

        private static void AddTmProAttribute(GameObject classGo, string attribute)
        {
            var attributesTransform = GetAttributeLayoutGroup(classGo);

            var instance = Instantiate(DiagramPool.Instance.classAttributePrefab, attributesTransform, false);
            instance.name = attribute;
            instance.transform.Find("AttributeText").GetComponent<TextMeshProUGUI>().text += attribute;

            instance.GetComponent<AttributePopUpManager>().classTxt =
                GetClassHeader(classGo).GetComponent<TextMeshProUGUI>();

            if (UIEditorManager.Instance.active)
                instance.GetComponentsInChildren<Button>(includeInactive: true)
                    .ForEach(x => x.gameObject.SetActive(true));
        }

        private static void UpdateTmProAttribute(GameObject classGo, string oldAttributeText, string newAttributeText)
        {
            var oldAttribute = GetAttributeLayoutGroup(classGo).Find(oldAttributeText);

            oldAttribute.name = newAttributeText;
            oldAttribute.Find("AttributeText").GetComponent<TextMeshProUGUI>().text = newAttributeText;
        }


        public void ResetGraph()
        {
            DiagramPool.Instance.ClassDiagram.graph = null;
        }
    }
}
