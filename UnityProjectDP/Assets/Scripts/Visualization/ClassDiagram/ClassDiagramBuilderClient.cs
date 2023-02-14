using Networking;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace AnimArch.Visualization.Diagrams
{
    public class ClassDiagramBuilderClient : ClassDiagramBuilder
    {
        public override void CreateGraph()
        {
            Spawner.Instance.CreateGraphServerRpc();
        }
    }
}
