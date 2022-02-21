using UnityEngine;

namespace AnimArch.Visualization.Animating
{
    [System.Serializable]
    public class AnimMethod //Filip
    {
        [SerializeField]
        public string Name;
        [SerializeField]
        public string Code;

        public AnimMethod(string Name, string Code)
        {
            this.Name = Name;
            this.Code = Code;
        }
    }
}