using Visualization.ClassDiagram.Editors;

namespace Visualization.ClassDiagram
{
    public class IClassDiagramBuilder : Singleton<IClassDiagramBuilder>
    {
        public IVisualEditor visualEditor;

        public IClassDiagramBuilder()
        {
            visualEditor = VisualEditorFactory.Create();
        }

        public virtual void CreateGraph() { }
        public virtual void LoadDiagram() { }
        public virtual void MakeNetworkedGraph() { }
    }
}
