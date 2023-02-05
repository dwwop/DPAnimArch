using System.Collections.Generic;
using AnimArch.Extensions;
using AnimArch.Visualization.Diagrams;
using Networking;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AnimArch.Visualization.UI
{
    public class UIEditorManager : Singleton<UIEditorManager>
    {
        private int _id;
        public bool active;
        private GameObject _node;
        private string _relType;

        public AttributePopUp attributePopUp;
        public MethodPopUp methodPopUp;
        public ClassPopUp classPopUp;
        public ParameterPopUp parameterPopUp;


        private static void InitializeCreation()
        {
            if (DiagramPool.Instance.ClassDiagram.graph) return;

            ClassDiagramBuilder.CreateGraph();
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            InitializeCreation();

            _id = 0;
        }


        public void StartEditing()
        {
            if (DiagramPool.Instance.ClassDiagram.graph != null)
                DiagramPool.Instance.ClassDiagram.graph.GetComponentsInChildren<Button>(includeInactive: true)
                    .ForEach(x => x.gameObject.SetActive(true));

            InitializeCreation();

            _id = 0;
            active = true;
        }

        public void EndEditing()
        {
            active = false;
            MenuManager.Instance.isSelectingNode = false;

            DiagramPool.Instance.ClassDiagram.graph.GetComponentsInChildren<Button>()
                .ForEach(x => x.gameObject.SetActive(false));
        }


        public void StartSelection(string type)
        {
            MenuManager.Instance.isSelectingNode = true;
            _relType = type;
        }

        public void SelectNode(GameObject selected)
        {
            if (!active || !MenuManager.Instance.isSelectingNode)
                return;
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
                AddRelation(selected);
            }
        }


        private void EndSelection()
        {
            Animating.Animation.Instance.HighlightClass(_node.name, false);
            _relType = null;
            _node = null;
            DiagramPool.Instance.ClassDiagram.graph.UpdateGraph();
            MenuManager.Instance.isSelectingNode = false;
            GameObject.Find("SelectionPanel").SetActive(false);
        }

        public void AddNode()
        {
            var newClass = new Class(_id.ToString());

            MainEditor.CreateNode(newClass, MainEditor.Source.Editor);
            _id++;
        }

        private void AddRelation(GameObject secondNode)
        {
            if (_node == null || secondNode == null) return;
            var type = _relType.Split();
            
            var relation = new Relation
            {
                SourceModelName = _node.name,
                TargetModelName = secondNode.name,
                PropertiesEaType =  type.Length > 1 ? type[1] : type[0],
                PropertiesDirection = type.Length > 1 ? "none" : "Source -> Destination"
            };
            MainEditor.CreateRelation(relation, MainEditor.Source.Editor);
            // if (type.Length > 1)
                // ClassEditor.Instance.CreateRelation(_node.name, secondNode.name, type[1], false, true);
            // else
                // ClassEditor.Instance.CreateRelation(_node.name, secondNode.name, type[0], false);
            EndSelection();
        }
    }
}