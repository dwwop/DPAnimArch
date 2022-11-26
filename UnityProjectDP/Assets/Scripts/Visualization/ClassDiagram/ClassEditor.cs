using UnityEngine;
using TMPro;
using AnimArch.Visualization.UI;
using Networking;
using OALProgramControl;

namespace AnimArch.Visualization.Diagrams
{
    public class ClassEditor : Singleton<ClassEditor>
    {
        private Graph _graph;
        private int _id;
        private bool _active;
        private GameObject _node;
        private string _relType;

        public AttributePopUp atrPopUp;
        public MethodPopUp mtdPopUp;
        public ClassPopUp classPopUp;
        
        public void InitializeCreation()
        {
            if (!_graph)
            {
                if (!DiagramPool.Instance.ClassDiagram.graph)
                {
                    _graph = DiagramPool.Instance.ClassDiagram.CreateGraph();
                }
                else
                {
                    _graph = DiagramPool.Instance.ClassDiagram.graph;
                }
                _id = 0;
            }
            _active = true;
        }

        public void Uninitialize()
        {
            _active = false;
            MenuManager.Instance.isSelectingNode = false;
        }

        public void CreateNodeFromRpc()
        {
            var newClass = new Class
            {
                Name = "NewClass_" + _id++
            };
            GenerateNode(newClass);
        }

        public void CreateNodeFromRpc(string name)
        {
            var newClass = new Class
            {
                Name = name
            };
            GenerateNode(newClass);
        }

        public void CreateNode()
        {
            var newClass = new Class
            {
                Name = "NewClass_" + _id++
            };
            Spawner.Instance.SpawnClass(newClass.Name);
            GenerateNode(newClass);
        }

        public CDClass CreateNode(Class newClass)
        {
            Spawner.Instance.SpawnClass(newClass.Name);
            return GenerateNode(newClass);
        }

        public CDClass GenerateNode(Class newClass)
        {
            CDClass tempCdClass = null;
            var name = CurrentClassName(newClass.Name, ref tempCdClass);
            if (tempCdClass == null)
                return null;

            DiagramPool.Instance.ClassDiagram.Classes.Add(new ClassInDiagram
                { XMIParsedClass = newClass, ClassInfo = tempCdClass});
            var node = AddClassToGraph(name);
            SetPosition(node);
            return tempCdClass;
        }

        public static string CurrentClassName(string name, ref CDClass TempCdClass)
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

        private void SetPosition(GameObject node)
        {
            var rect = node.GetComponent<RectTransform>();
            rect.position = new Vector3(100f, 200f, 1);
        }

        public GameObject AddClassToGraph(string name)
        {
            var currentClass = DiagramPool.Instance.ClassDiagram.Classes.Find(item => item.XMIParsedClass.Name == name)
                .XMIParsedClass;
            if (!_graph)
                InitializeCreation();
            var node = _graph.AddNode();
            node.name = name;

            SetClassTmProName(node, name);
            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(name);
            if (classInDiagram != null)
            {
                classInDiagram.VisualObject = node;
            }
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
                DiagramPool.Instance.ClassDiagram.CreateRelation(_node, secondNode, type[1], true);
            else
                DiagramPool.Instance.ClassDiagram.CreateRelation(_node, secondNode, type[0]);
            EndSelection();
        }

        private void EndSelection()
        {
            _relType = null;
            _node = null;
            _graph.UpdateGraph();
            MenuManager.Instance.isSelectingNode = false;
            Animating.Animation.Instance.UnhighlightAll();
            GameObject.Find("Selection RightPanel").SetActive(false);
        }

        public void StartSelection(string type)
        {
            MenuManager.Instance.isSelectingNode = true;
            _relType = type;
        }
        
        public void SetClassName(string targetClass, string newName, bool fromRpc)
        {
            if (!fromRpc)
                Spawner.Instance.SetClassName(targetClass, newName);
            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(targetClass);
            if (classInDiagram == null)
            {
                return;
            }

            classInDiagram.ClassInfo.Name = newName;
            classInDiagram.XMIParsedClass.Name = newName;
            classInDiagram.VisualObject.name = newName;
            SetClassTmProName(classInDiagram.VisualObject, newName);
        }

        public static void SetClassTmProName(GameObject classGo, string name)
        {
            classGo.transform
                .Find("Background")
                .Find("Header")
                .GetComponent<TextMeshProUGUI>()
                .text = name;
        }
    }
}
