﻿using System;
using AnimArch.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Visualization.ClassDiagram;
using Visualization.ClassDiagram.Editors;
using Visualization.ClassDiagram.Relations;
using Visualization.UI.PopUps;

namespace Visualization.UI
{
    public class UIEditorManager : Singleton<UIEditorManager>
    {
        public bool active;
        private GameObject _fromClass;
        private string _relType;
        private IClassDiagramBuilder _classDiagramBuilder;
        public MainEditor mainEditor;

        [SerializeField]
        public bool NetworkEnabled;

        public AttributePopUp attributePopUp;
        public MethodPopUp methodPopUp;
        public ClassPopUp classPopUp;
        public ParameterPopUp parameterPopUp;
        public ConfirmPopUp confirmPopUp;
        public ErrorPopUp errorPopUp;
        public ExitPopUp exitPopUp;

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
            mainEditor = MainEditorFactory.Create(_classDiagramBuilder.visualEditor);
        }

        public void CreateNewDiagram()
        {
            mainEditor.ClearDiagram();
            StartEditing();
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
            if (selected == _fromClass)
            {
                Animation.Animation.Instance.HighlightClass(_fromClass.name, false);
                _fromClass = null;
            }
            else if (_fromClass == null)
            {
                _fromClass = selected;
                Animation.Animation.Instance.HighlightClass(_fromClass.name, true);
            }
            else
            {
                AddRelation(selected);
            }
        }

        public void EndSelection()
        {
            Animation.Animation.Instance.HighlightClass(_fromClass.name, false);
            _relType = null;
            _fromClass = null;
            MenuManager.Instance.isSelectingNode = false;
            GameObject.Find("SelectionPanel").SetActive(false);
        }

        private void AddRelation(GameObject toClass)
        {
            if (_fromClass == null || toClass == null)
                return;
            var type = _relType.Split();
            var relType = type.Length > 1 ? type[1] : type[0];

            if (DiagramPool.Instance.ClassDiagram.FindRelation(_fromClass.name, toClass.name, relType) != null)
            {
                errorPopUp.ActivateCreation();
                return;
            }
            
            var relation = new Relation
            {
                ConnectorXmiId = Guid.NewGuid().ToString(),
                SourceModelName = _fromClass.name,
                TargetModelName = toClass.name,
                PropertiesEaType = relType,
                PropertiesDirection = type.Length > 1 ? "none" : "Source -> Destination"
            };

            mainEditor.CreateRelation(relation);
            EndSelection();
        }
        
        private void Update()
        {
            if (Input.GetKey(KeyCode.Escape) && !exitPopUp.gameObject.activeSelf)
            {
                exitPopUp.ActivateCreation();
            }
        }
    }
}
