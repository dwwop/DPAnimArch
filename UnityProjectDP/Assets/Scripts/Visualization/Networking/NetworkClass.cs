using AnimArch.Extensions;
using AnimArch.Visualization.UI;
using Unity.Netcode;
using UnityEngine;

public class NetworkClass : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (UIEditorManager.Instance.active)
            GetComponentsInChildren<UnityEngine.UI.Button>(true)
                .ForEach(x => x.gameObject.SetActive(true));
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdatePostionServerRpc(Vector3 postion)
    {
        if (IsClient && !IsHost)
            return;
        transform.position = postion;
    }
}
