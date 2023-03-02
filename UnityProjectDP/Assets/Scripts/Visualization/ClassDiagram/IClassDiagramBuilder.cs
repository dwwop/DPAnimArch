using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimArch.Visualization.Diagrams
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
