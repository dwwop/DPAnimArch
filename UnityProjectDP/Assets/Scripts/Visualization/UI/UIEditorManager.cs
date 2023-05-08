using System;
using AnimArch.Extensions;
using UnityEngine;
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
        private IClassDiagramBuilder _classDiagramBuilder;
        public MainEditor mainEditor;

        [SerializeField] public bool NetworkEnabled;

        public AttributePopUp attributePopUp;
        public MethodPopUp methodPopUp;
        public ClassPopUp classPopUp;
        public ParameterPopUp parameterPopUp;
        public ConfirmPopUp confirmPopUp;
        public ErrorPopUp errorPopUp;
        public ExitPopUp exitPopUp;

        public State state;
        public Relation relation;

        public void InitializeCreation()
        {
            Debug.Assert(_classDiagramBuilder != null);
            if (DiagramPool.Instance.ClassDiagram.graph != null) return;
            _classDiagramBuilder.CreateGraph();
            _classDiagramBuilder.MakeNetworkedGraph();
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

        public void StartSelection(string newRelationType)
        {
            var type = newRelationType.Split();
            var relType = type.Length > 1 ? type[1] : type[0];
            relation = new Relation
            {
                ConnectorXmiId = Guid.NewGuid().ToString(),
                PropertiesEaType = relType,
                PropertiesDirection = type.Length > 1 ? "none" : "Source -> Destination"
            };
            state = new SelectFirstState();
            MenuManager.Instance.isSelectingNode = true;
        }

        public void SelectNode(GameObject selected)
        {
            if (!active || state == null)
                return;
            state.Select(selected);
        }

        public void EndSelection()
        {
            Animation.Animation.Instance.HighlightClass(relation.SourceModelName, false);
            MenuManager.Instance.isSelectingNode = false;
            relation = null;
            state = null;
            GameObject.Find("SelectionPanel").SetActive(false);
        }

        public void AddRelation()
        {
            if (relation == null)
                return;

            if (DiagramPool.Instance.ClassDiagram.FindRelation(relation.SourceModelName, relation.TargetModelName,
                    relation.PropertiesEaType) != null)
            {
                errorPopUp.ActivateCreation();
                return;
            }

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
