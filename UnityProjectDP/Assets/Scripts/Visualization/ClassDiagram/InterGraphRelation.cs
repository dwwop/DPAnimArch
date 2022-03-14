using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AnimArch.Visualization.Diagrams
{
    public class InterGraphRelation : MonoBehaviour
    {
        public ObjectInDiagram Object;
        public ClassInDiagram Class;

        public void Initialize(ObjectInDiagram Object, ClassInDiagram Class)
        {
            this.Object = Object;
            this.Class = Class;
        }
    }
}
