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
            _graph = DiagramPool.Instance.ClassDiagram.CreateGraph();
            _active = true;
            _id = 0;
        }

        public void Uninitialize()
        {
            _active = false;
            MenuManager.Instance.isSelectingNode = false;
        }

        public void CreateNodeFromRpc()
        {
            CreateNode2();
        }

        public void CreateNode()
        {
            Spawner.Instance.SpawnClass();
            CreateNode2();
        }
        public void CreateNode2()
        {
            var node = _graph.AddNode();
            node.name = "NewClass " + _id;
            var background = node.transform.Find("Background");
            var header = background.Find("Header");
            // var attributes = background.Find("Attributes");
            // var methods = background.Find("Methods");

            header.GetComponent<TextMeshProUGUI>().text = node.name;
            var rc = node.GetComponent<RectTransform>();
            rc.position = new Vector3(100f, 200f, 1);
            _id++;


            var newClass = new Class
            {
                Name = node.name
            };
            var pos = node.transform.position;
            newClass.Left = pos.x / 1.25f;
            newClass.Top = pos.y / 1.25f;

            CDClass TempCDClass = null;
            var i = 0;
            var currentName = node.name;
            var baseName = node.name;
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

            node.name = currentName;
            DiagramPool.Instance.ClassDiagram.Classes.Add(new ClassInDiagram
                { XMIParsedClass = newClass, ClassInfo = TempCDClass, VisualObject = node });
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
    }
}