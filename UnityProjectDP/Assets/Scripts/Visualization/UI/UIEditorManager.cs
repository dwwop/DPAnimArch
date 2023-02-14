﻿using System.Collections.Generic;
using AnimArch.Extensions;
using AnimArch.Visualization.Diagrams;
using Networking;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AnimArch.Visualization.UI
{
    public class UIEditorManager : Singleton<UIEditorManager>
    {
        public bool active;
        private GameObject _node;
        private string _relType;
        private IClassDiagramBuilder _classDiagramBuilder;

        [SerializeField]
        public bool NetworkEnabled;

        public AttributePopUp attributePopUp;
        public MethodPopUp methodPopUp;
        public ClassPopUp classPopUp;
        public ParameterPopUp parameterPopUp;

        public void InitializeCreation()
        {
            Debug.Assert(_classDiagramBuilder != null);
            if (DiagramPool.Instance.ClassDiagram.graph == null)
            {
                _classDiagramBuilder.CreateGraph();
                _classDiagramBuilder.MakeNetworkedGraph();
            }
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _classDiagramBuilder = ClassDiagramBuilderFactory.Create();
        }

        public void StartEditing()
        {
            if (DiagramPool.Instance.ClassDiagram.graph == null)
                InitializeCreation();
            Debug.Assert(DiagramPool.Instance.ClassDiagram.graph);
            DiagramPool.Instance.ClassDiagram.graph.GetComponentsInChildren<Button>(includeInactive: true)
                .ForEach(x => x.gameObject.SetActive(true));
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

        private void AddRelation(GameObject secondNode)
        {
            if (_node == null || secondNode == null) return;
            var type = _relType.Split();

            var relation = new Relation
            {
                SourceModelName = _node.name,
                TargetModelName = secondNode.name,
                PropertiesEaType = type.Length > 1 ? type[1] : type[0],
                PropertiesDirection = type.Length > 1 ? "none" : "Source -> Destination"
            };

            MainEditor.CreateRelation(relation, MainEditor.Source.Editor);
            EndSelection();
        }
    }
}
