using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnimArch.Visualization.Diagrams;
using Unity.Netcode;
using UnityEngine;

namespace Networking
{
    [RequireComponent(typeof(NetworkObject))]
    public class Spawner : NetworkSingleton<Spawner>
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void SpawnClass()
        {
            if (IsServer)
            {
                SpawnClassClientRpc();
            }
            else
            {
                SpawnClassServerRpc();
            }
        }

        // Server RPC - if client creates instance, server RPC is called and instance is created at server side.
        [ServerRpc(RequireOwnership = false)]
        public void SpawnClassServerRpc(ServerRpcParams rpcParams = default)
        {
            Debug.Log("Executing spawn class RPC");
            ClassEditor.Instance.CreateNodeFromRpc();
        }

        [ClientRpc]
        public void SpawnClassClientRpc()
        {
            if (IsServer)
                return;
            Debug.Log("Executing spawn class RPC");
            ClassEditor.Instance.CreateNodeFromRpc();
        }

        public void SetClassName()
        {

        }
    }
}
