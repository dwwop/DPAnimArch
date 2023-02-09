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

        public void SpawnNode(string name, string id)
        {
            if (IsServer)
            {
                SpawnClassClientRpc(name, id);
            }
            else
            {
                SpawnClassServerRpc(name, id);
            }
        }

        // Server RPC - if client creates instance, server RPC is called and instance is created at server side.
        [ServerRpc(RequireOwnership = false)]
        public void SpawnClassServerRpc(string name, string id)
        {
            var newClass = new Class(name, id);
            MainEditor.CreateNode(newClass, MainEditor.Source.RPC);
        }

        [ClientRpc]
        public void SpawnClassClientRpc(string name, string id)
        {
            if (IsServer)
                return;

            var newClass = new Class(name, id);
            MainEditor.CreateNode(newClass, MainEditor.Source.RPC);
        }

        public void SetNodeName(string oldName, string newName)
        {
            if (IsServer)
            {
                SetClassNameClientRpc(oldName, newName);
            }
            else
            {
                SetClassNameServerRpc(oldName, newName);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void SetClassNameServerRpc(string oldName, string newName)
        {
            MainEditor.UpdateNodeName(oldName, newName, true);
        }

        [ClientRpc]
        public void SetClassNameClientRpc(string oldName, string newName)
        {
            if (IsServer)
                return;
            MainEditor.UpdateNodeName(oldName, newName, true);
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
            MainEditor.AddAttribute(targetClass, attribute, MainEditor.Source.RPC);
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
            MainEditor.AddAttribute(targetClass, attribute, MainEditor.Source.RPC);
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
            MainEditor.AddMethod(targetClass, method, MainEditor.Source.RPC);
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
            MainEditor.AddMethod(targetClass, method, MainEditor.Source.RPC);
        }

        public void AddRelation(string sourceClass, string destinationClass, string relationType, string direction)
        {
            if (IsServer)
            {
                AddRelationClientRpc(sourceClass, destinationClass, relationType, direction);
            }
            else
            {
                AddRelationServerRpc(sourceClass, destinationClass, relationType, direction);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void AddRelationServerRpc(string sourceClass, string destinationClass, string relationType, string direction)
        {
            var relation = new Relation
            {
                SourceModelName = sourceClass,
                TargetModelName = destinationClass,
                PropertiesEaType = relationType,
                PropertiesDirection = direction
            };
            MainEditor.CreateRelation(relation, MainEditor.Source.RPC);
        }

        [ClientRpc]
        public void AddRelationClientRpc(string sourceClass, string destinationClass, string relationType, string direction)
        {
            if (IsServer)
                return;
            var relation = new Relation
            {
                SourceModelName = sourceClass,
                TargetModelName = destinationClass,
                PropertiesEaType = relationType,
                PropertiesDirection = direction
            };
            MainEditor.CreateRelation(relation, MainEditor.Source.RPC);
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
            VisualEditor.SetPosition(className, position, true);
        }

        [ClientRpc]
        public void SetPositionClientRpc(string className, Vector3 position)
        {
            if (IsServer)
                return;
            VisualEditor.SetPosition(className, position, true);
        }
    }
}