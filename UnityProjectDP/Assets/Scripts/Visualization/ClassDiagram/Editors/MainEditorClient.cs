using Networking;
using Unity.Netcode;

namespace AnimArch.Visualization.Diagrams
{
    public class MainEditorClient : MainEditor
    {
        public MainEditorClient(IVisualEditor visualEditor) : base(visualEditor)
        {
        }

        public override void DeleteNode(string className)
        {
            Spawner.Instance.DeleteClassServerRpc(className);
        }
    }
}
