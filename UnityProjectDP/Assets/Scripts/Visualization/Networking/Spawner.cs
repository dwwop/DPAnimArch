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

        public void SpawnClass(string name)
        {
            if (IsServer)
            {
                SpawnClassClientRpc(name);
            }
            else
            {
                SpawnClassServerRpc(name);
            }
        }

        // Server RPC - if client creates instance, server RPC is called and instance is created at server side.
        [ServerRpc(RequireOwnership = false)]
        public void SpawnClassServerRpc(string name)
        {
           ClassEditor.Instance.CreateNodeFromRpc(name);
        }

        [ClientRpc]
        public void SpawnClassClientRpc(string name)
        {
            if (IsServer)
                return;
            ClassEditor.Instance.CreateNodeFromRpc(name);
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

        [ServerRpc(RequireOwnership = false)]
        public void SetClassNameServerRpc(string targetClass, string newName)
        {
            ClassEditor.SetClassName(targetClass, newName, true);
        }

        [ClientRpc]
        public void SetClassNameClientRpc(string targetClass, string newName)
        {
            if (IsServer)
                return;
            ClassEditor.SetClassName(targetClass, newName, true);
        }

        public void AddAttribute(string targetClass, string name, string type)
        {
            if (IsServer)
                AddAttributeClientRpc(targetClass, name, type);
            else
                AddAttributeServerRpc(targetClass, name, type);
        }

        [ServerRpc(RequireOwnership = false)]
        public void AddAttributeServerRpc(string targetClass, string name, string type)
        {
            var attribute = new AnimArch.Visualization.Diagrams.Attribute()
            {
                Name = name,
                Type = type
            };
            ClassEditor.AddAttribute(targetClass, attribute, true);
        }

        [ClientRpc]
        public void AddAttributeClientRpc(string targetClass, string name, string type)
        {
            if (IsServer)
                return;
            var attribute = new AnimArch.Visualization.Diagrams.Attribute()
            {
                Name = name,
                Type = type
            };
            ClassEditor.AddAttribute(targetClass, attribute, true);
        }

        public void AddMethod(string targetClass, string name, string returnValue)
        {
            if (IsServer)
                AddMethodClientRpc(targetClass, name, returnValue);
            else
                AddMethodServerRpc(targetClass, name, returnValue);
        }

        [ServerRpc(RequireOwnership = false)]
        public void AddMethodServerRpc(string targetClass, string name, string returnValue)
        {
            var method = new Method
            {
                Name = name,
                ReturnValue = returnValue
            };
            ClassEditor.AddMethod(targetClass, method, ClassEditor.Source.RPC);
        }

        [ClientRpc]
        public void AddMethodClientRpc(string targetClass, string name, string returnValue)
        {
            if (IsServer)
                return;
            var method = new Method
            {
                Name = name,
                ReturnValue = returnValue
            };
            ClassEditor.AddMethod(targetClass, method, ClassEditor.Source.RPC);
        }

        public void AddRelation(string sourceClass, string destinationClass, string relationType)
        {
            if (IsServer)
            {
                AddRelationClientRpc(sourceClass, destinationClass, relationType);
            }
            else
            {
                AddRelationServerRpc(sourceClass, destinationClass, relationType);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void AddRelationServerRpc(string sourceClass, string destinationClass, string relationType)
        {
            ClassEditor.Instance.CreateRelation(sourceClass, destinationClass, relationType, true);
        }

        [ClientRpc]
        public void AddRelationClientRpc(string sourceClass, string destinationClass, string relationType)
        {
            if (IsServer)
                return;
            ClassEditor.Instance.CreateRelation(sourceClass, destinationClass, relationType, true);
        }

        public void SetPosition(string className, Vector3 position)
        {
            if (IsServer)
            {
                SetPositionClientRpc(className, position);
            }
            else
            {
                SetPositionServerRpc(className, position);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetPositionServerRpc(string className, Vector3 position)
        {
            ClassEditor.SetPosition(className, position, true);
        }

        [ClientRpc]
        public void SetPositionClientRpc(string className, Vector3 position)
        {
            if (IsServer)
                return;
            ClassEditor.SetPosition(className, position, true);
        }
    }
}
