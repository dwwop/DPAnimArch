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
        public bool active;
        private GameObject _fromClass;
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
        }

        public void CreateNewDiagram()
        {
            MainEditor.ClearDiagram();
            StartEditing();
        }
        
        public void StartEditing()
        {
            if (DiagramPool.Instance.ClassDiagram.graph != null)
                DiagramPool.Instance.ClassDiagram.graph.GetComponentsInChildren<Button>(includeInactive: true)
                    .ForEach(x => x.gameObject.SetActive(true));

            InitializeCreation();

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
                Animating.Animation.Instance.HighlightClass(_fromClass.name, false);
                _fromClass = null;
            }
            else if (_fromClass == null)
            {
                _fromClass = selected;
                Animating.Animation.Instance.HighlightClass(_fromClass.name, true);
            }
            else
            {
                AddRelation(selected);
            }
        }


        private void EndSelection()
        {
            Animating.Animation.Instance.HighlightClass(_fromClass.name, false);
            _relType = null;
            _fromClass = null;
            DiagramPool.Instance.ClassDiagram.graph.UpdateGraph();
            MenuManager.Instance.isSelectingNode = false;
            GameObject.Find("SelectionPanel").SetActive(false);
        }

        private void AddRelation(GameObject toClass)
        {
            if (_fromClass == null || toClass == null) return;
            var type = _relType.Split();

            var relation = new Relation
            {
                SourceModelName = _fromClass.name,
                TargetModelName = toClass.name,
                PropertiesEaType = type.Length > 1 ? type[1] : type[0],
                PropertiesDirection = type.Length > 1 ? "none" : "Source -> Destination"
            };
            
            MainEditor.CreateRelation(relation, MainEditor.Source.Editor);
            EndSelection();
        }
    }
}
