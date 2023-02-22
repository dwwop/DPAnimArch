using AnimArch.Extensions;
using AnimArch.Visualization.UI;
using Unity.Netcode;

public class NetworkClass : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (UIEditorManager.Instance.active)
            GetComponentsInChildren<UnityEngine.UI.Button>(true)
                .ForEach(x => x.gameObject.SetActive(true));
    }
}
