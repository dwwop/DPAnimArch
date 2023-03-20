using Unity.Netcode;
using Visualization.UI;

namespace Visualization.ClassDiagram.Editors
{
    public static class VisualEditorFactory
    {
        public static IVisualEditor Create()
        {
            if (UIEditorManager.Instance.NetworkEnabled)
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    return new VisualEditorServer();
                }
                else
                {
                    return new VisualEditorClient();
                }
            }
            else
            {
                return new VisualEditor();
            }
        }
    }
}
