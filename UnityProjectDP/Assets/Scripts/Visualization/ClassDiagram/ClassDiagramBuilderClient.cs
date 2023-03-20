using Visualization.Networking;

namespace Visualization.ClassDiagram
{
    public class ClassDiagramBuilderClient : ClassDiagramBuilder
    {
        public override void CreateGraph()
        {
            Spawner.Instance.CreateGraphServerRpc();
        }
    }
}
