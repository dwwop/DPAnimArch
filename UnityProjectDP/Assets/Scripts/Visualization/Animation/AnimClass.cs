using System.Collections.Generic;
using UnityEngine;

namespace Visualization.Animation
{
    [System.Serializable]
    public class AnimClass  //Filip
    {
        [SerializeField]
        public string Name;
        [SerializeField]
        public List<AnimMethod> Methods;

        public AnimClass(string Name)
        {
            this.Name = Name;
            this.Methods = new List<AnimMethod>();
        }
    }
}
