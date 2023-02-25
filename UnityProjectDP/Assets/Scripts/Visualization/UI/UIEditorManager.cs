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

        private void EndSelection()
        {
            Animation.Animation.Instance.HighlightClass(_fromClass.name, false);
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

            mainEditor.CreateRelation(relation, MainEditor.Source.Editor);
            EndSelection();
        }
    }
}
