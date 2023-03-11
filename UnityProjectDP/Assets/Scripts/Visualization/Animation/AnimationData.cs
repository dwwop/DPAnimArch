using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Visualization.UI;

namespace Visualization.Animation
{
    //Holds data for created animations
    //Implemets methods to find, create and remove animations
    //has global acces: AnimationData.Instance.calledMethod()
    public class AnimationData : Singleton<AnimationData>
    {
        private List<Anim> animations;
        public Anim selectedAnim;
        string diagramPath = @"C:/AnimArch/exportedXMI.xml";
        public int diagramId = 4;
        public float AnimSpeed { get; set; }
        private void Awake()
        {
            animations = new List<Anim>();
            selectedAnim = new Anim("");
        }
        public List<Anim> getAnimList()
        {
            return animations;
        }
        public void AddAnim(Anim anim)
        {
            animations.Add(anim);
            MenuManager.SetAnimationButtonsActive(true);
        }
        public bool RemoveAnim(Anim anim)
        {
            if (animations.Count == 1)
            {
                selectedAnim = new Anim("");
            }
            return animations.Remove(anim);
        }
        public Anim FindAnimByName(string name)
        {
            foreach (Anim a in animations)
            {
                if (selectedAnim.AnimationName.Equals(name))
                {
                    return a;
                }
            }
            Debug.Log("Anim " + name + " not found");
            return new Anim();
        }
        public string GetDiagramPath()
        {
            return diagramPath;
        }
        public void SetDiagramPath(string diagramPath)
        {
            this.diagramPath = diagramPath;
            this.diagramId = 0;

        }
        public void ClearData()
        {
            animations.Clear();
            MenuManager.Instance.UpdateAnimations();
            selectedAnim = new Anim("");
            diagramId = 0;
            
            MenuManager.SetAnimationButtonsActive(false);
        }
    }
}
