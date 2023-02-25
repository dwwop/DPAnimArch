using AnimArch.Visualization.UI;
using Unity.Netcode;

namespace AnimArch.Visualization.Diagrams
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
