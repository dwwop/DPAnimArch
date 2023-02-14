using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimArch.Visualization.Diagrams
{
    public class IClassDiagramBuilder
    {
        public virtual void CreateGraph() { }
        public virtual void LoadDiagram() { }
        public virtual void MakeNetworkedGraph() { }
    }
}
