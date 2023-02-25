using Unity.Netcode;
using Visualization.UI;

namespace Visualization.ClassDiagram.Editors
{
    public static class MainEditorFactory
    {
        public static MainEditor Create(IVisualEditor visualEditor)
        {
            if (UIEditorManager.Instance.NetworkEnabled)
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    return new MainEditor(visualEditor);
                }
                else
                {
                    return new MainEditorClient(visualEditor);
                }
            }
            else
            {
                return new MainEditor(visualEditor);
            }
        }
    }
}
