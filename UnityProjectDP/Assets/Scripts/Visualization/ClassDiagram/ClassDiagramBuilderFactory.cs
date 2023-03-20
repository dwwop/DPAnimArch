using Unity.Netcode;
using Visualization.UI;

namespace Visualization.ClassDiagram
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
