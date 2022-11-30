using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using AnimArch.Visualization.UI;
using Networking;
using OALProgramControl;

namespace AnimArch.Visualization.Diagrams
{
    public class ClassEditor : Singleton<ClassEditor>
    {
        private int _id;
        private Graph _graph;
        private bool _active;
        private GameObject _node;
        private string _relType;

        public AttributePopUp atrPopUp;
        public MethodPopUp mtdPopUp;
        public ClassPopUp classPopUp;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _id = 0;
        }

        public enum Source
        {
            RPC,
            editor,
            loader
        }

        public void InitializeCreation()
        {
            if (!_graph)
            {
                if (DiagramPool.Instance.ClassDiagram.graph)
                    _graph = DiagramPool.Instance.ClassDiagram.graph;
                else
                    _graph = DiagramPool.Instance.ClassDiagram.CreateGraph();

                _id = 0;
            }

            _active = true;
        }

        public void Uninitialize()
        {
            _active = false;
            MenuManager.Instance.isSelectingNode = false;
        }

        public void CreateNode()
        {
            var newClass = new Class
            {
                Name = "NewClass_" + _id,
                XmiId = _id.ToString(),
                Type = "uml:Class",
                Attributes = new List<Attribute>(),
                Methods = new List<Method>()
            };

            Spawner.Instance.SpawnClass(newClass.Name);
            GenerateNode(newClass);
        }

        public void CreateNodeFromRpc(string name)
        {
            var newClass = new Class
            {
                Name = name,
                XmiId = _id.ToString(),
                Type = "uml:Class",
                Attributes = new List<Attribute>(),
                Methods = new List<Method>()
            };
            GenerateNode(newClass);
        }

        public CDClass CreateNode(Class newClass)
        {
            Spawner.Instance.SpawnClass(newClass.Name);
            return GenerateNode(newClass);
        }

        private CDClass GenerateNode(Class newClass)
        {
            CDClass tempCdClass = null;
            var name = CurrentClassName(newClass.Name, ref tempCdClass);
            if (tempCdClass == null)
                return null;

            var classInDiagram = new ClassInDiagram { XMIParsedClass = newClass, ClassInfo = tempCdClass };
            DiagramPool.Instance.ClassDiagram.Classes.Add(classInDiagram);
            var node = AddClassToGraph(name);
            SetPosition(node);
            if (classInDiagram.XMIParsedClass.Left == 0)
            {
                var position = classInDiagram.VisualObject.transform.localPosition;
                classInDiagram.XMIParsedClass.Left = position.x / 2.5f;
                classInDiagram.XMIParsedClass.Top = -position.y / 2.5f;
            }

            _id++;
            return tempCdClass;
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
            var currentClass = DiagramPool.Instance.ClassDiagram.Classes.Find(item => item.XMIParsedClass.Name == name)
                .XMIParsedClass;
            if (!_graph)
                InitializeCreation();
            var node = _graph.AddNode();
            node.name = name;

            SetClassTmProName(node, name);
            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(name);
            if (classInDiagram == null) return node;

            classInDiagram.VisualObject = node;
            return node;
        }

        public void SelectNode(GameObject selected)
        {
            if (!_active || !MenuManager.Instance.isSelectingNode) return;
            if (selected == _node)
            {
                Animating.Animation.Instance.HighlightClass(_node.name, false);
                _node = null;
            }
            else if (_node == null)
            {
                _node = selected;
                Animating.Animation.Instance.HighlightClass(_node.name, true);
            }
            else
            {
                DrawRelation(selected);
            }
        }

        private void DrawRelation(GameObject secondNode)
        {
            if (_node == null || secondNode == null) return;
            var type = _relType.Split();
            if (type.Length > 1)
                CreateRelation(_node.name, secondNode.name, type[1], false, true);
            else
                CreateRelation(_node.name, secondNode.name, type[0], false);
            EndSelection();
        }

        private void EndSelection()
        {
            Animating.Animation.Instance.HighlightClass(_node.name, false);
            _relType = null;
            _node = null;
            _graph.UpdateGraph();
            MenuManager.Instance.isSelectingNode = false;
            GameObject.Find("SelectionPanel").SetActive(false);
        }

        public void StartSelection(string type)
        {
            MenuManager.Instance.isSelectingNode = true;
            _relType = type;
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
            var edge = _graph.AddEdge(sourceClassGo, destinationClassGo, relation.PrefabType);
            relInDiag.VisualObject = edge;
            Canvas.ForceUpdateCanvases();
        }


        public static RelationInDiagram CreateRelationEdge(Relation relation)
        {
            relation.FromClass = relation.SourceModelName.Replace(" ", "_");
            relation.ToClass = relation.TargetModelName.Replace(" ", "_");
            
            switch (relation.PropertiesEaType)
            {
                case "Association":
                    switch (relation.PropertiesDirection)
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
            
            if ("Generalization".Equals(relation.PropertiesEaType) || "Realisation".Equals(relation.PropertiesEaType))
            {
                var fromClass = OALProgram.Instance.ExecutionSpace.getClassByName(relation.FromClass);
                var toClass = OALProgram.Instance.ExecutionSpace.getClassByName(relation.ToClass);
                
                if (fromClass != null && toClass != null)
                {
                    fromClass.SuperClass = toClass;
                }
            }
            var relInDiag = new RelationInDiagram { XMIParsedRelation = relation, RelationInfo = tempCdRelationship };
            DiagramPool.Instance.ClassDiagram.Relations.Add(relInDiag);
            return relInDiag;
        }

        public static void SetClassName(string targetClass, string newName, bool fromRpc)
        {
            if (!fromRpc)
                Spawner.Instance.SetClassName(targetClass, newName);

            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(targetClass);
            if (classInDiagram == null)
                return;

            classInDiagram.ClassInfo.Name = newName;
            classInDiagram.XMIParsedClass.Name = newName;
            classInDiagram.VisualObject.name = newName;
            SetClassTmProName(classInDiagram.VisualObject, newName);
        }

        private static string StringMethod(Method method)
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

            arguments += ") :";

            return method.Name + arguments + method.ReturnValue + "\n";
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
                    AddTmProMethod(node, StringMethod(methodToAdd));
                    break;
            }
        }

        private static void AddMethod(string targetClass, Method methodToAdd)
        {
            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(targetClass);
            if (classInDiagram == null)
                return;

            classInDiagram.XMIParsedClass.Methods ??= new List<Method>();

            methodToAdd.Id = (classInDiagram.XMIParsedClass.Methods.Count + 1).ToString();

            if (DiagramPool.Instance.ClassDiagram.FindMethodByName(targetClass, methodToAdd.Name) != null)
                return;

            classInDiagram.XMIParsedClass.Methods.Add(methodToAdd);

            if (!OALProgram.Instance.ExecutionSpace.ClassExists(targetClass))
                return;

            var cdClass = OALProgram.Instance.ExecutionSpace.getClassByName(targetClass);
            var cdMethod = new CDMethod(cdClass, methodToAdd.Name, EXETypes.ConvertEATypeName(methodToAdd.ReturnValue));

            if (methodToAdd.arguments != null)
                AddParameters(methodToAdd, cdMethod);

            cdClass.AddMethod(cdMethod);

            AddTmProMethod(classInDiagram.VisualObject, StringMethod(methodToAdd));
        }

        public static void AddParameters(Method method, CDMethod cdMethod)
        {
            foreach (var arg in method.arguments)
            {
                var tokens = arg.Split(' ');
                var type = tokens[0];
                var name = tokens[1];

                cdMethod.Parameters.Add(new CDParameter { Name = name, Type = EXETypes.ConvertEATypeName(type) });
            }
        }

        public static bool AddAttribute(string targetClass, Attribute attributeToAdd)
        {
            var c = DiagramPool.Instance.ClassDiagram.FindClassByName(targetClass);
            if (c == null)
            {
                return false;
            }

            c.XMIParsedClass.Attributes ??= new List<Attribute>();

            attributeToAdd.Id = (c.XMIParsedClass.Attributes.Count + 1).ToString();
            if (DiagramPool.Instance.ClassDiagram.FindAttributeByName(targetClass, attributeToAdd.Name) != null)
                return false;
            c.XMIParsedClass.Attributes.Add(attributeToAdd);
            return true;
        }


        private static void SetClassTmProName(GameObject classGo, string name)
        {
            classGo.transform
                .Find("Background")
                .Find("Header")
                .GetComponent<TextMeshProUGUI>()
                .text = name;
        }

        private static void AddTmProMethod(GameObject classGo, string method)
        {
            classGo.transform
                .Find("Background")
                .Find("Methods")
                .GetComponent<TextMeshProUGUI>()
                .text += method;
        }

        public void ResetGraph()
        {
            _graph = null;
        }
    }
}