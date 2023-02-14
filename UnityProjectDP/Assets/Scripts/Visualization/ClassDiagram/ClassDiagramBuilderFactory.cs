
using AnimArch.Visualization.UI;
using Unity.Netcode;

namespace AnimArch.Visualization.Diagrams
{
    public static class ClassDiagramBuilderFactory
    {
        public static IClassDiagramBuilder Create()
        {
            if (UIEditorManager.Instance.NetworkEnabled)
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    return new ClassDiagramBuilderServer();
                }
                else
                {
                    return new ClassDiagramBuilderClient();
                }
            }
            else
            {
                return new ClassDiagramBuilder();
            }
        }
    }
}
