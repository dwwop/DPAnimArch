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
           ClassEditor.Instance.CreateNodeFromRpc();
        }

        [ClientRpc]
        public void SpawnClassClientRpc()
        {
            if (IsServer)
                return;
           ClassEditor.Instance.CreateNodeFromRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetClassNameServerRpc(string targetClass, string newName)
        {
            ClassEditor.Instance.SetClassName(targetClass, newName, true);
        }

        [ClientRpc]
        public void SetClassNameClientRpc(string targetClass, string newName)
        {
            if (IsServer)
                return;
            ClassEditor.Instance.SetClassName(targetClass, newName, true);
        }

        public void SetClassName(string targetClass, string newName)
        {
            if (IsServer)
            {
                SetClassNameClientRpc(targetClass, newName);
            }
            else
            {
                SetClassNameServerRpc(targetClass, newName);
            }
        }
    }
}
