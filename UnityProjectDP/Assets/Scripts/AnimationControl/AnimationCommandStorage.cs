using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public class AnimationCommandStorage
    {
        public List<List<AnimationCommand>> AnimationSteps { get; }
        private readonly Object AnimationStepsLock;
        public AnimationCommandStorage()
        {
            this.AnimationSteps = new List<List<AnimationCommand>>();
            this.AnimationSteps.Add(new List<AnimationCommand>());
            this.AnimationStepsLock = new Object();
        }

        public void AddAnimationStep(AnimationCommand Command)
        {
            lock (this.AnimationStepsLock)
            {
                this.AnimationSteps.Last().Add(Command);
            }
        }
        public void NewAnimationSequence()
        {
            lock (this.AnimationStepsLock)
            {
                if (this.AnimationSteps.Last().Any())
                {
                    this.AnimationSteps.Add(new List<AnimationCommand>());
                }
            }
        }

        public void ClearSteps()
        {
            if (this.AnimationSteps.Any())
            {
                AnimationSteps.Last();
                while (!this.AnimationSteps.Last().Any())
                {
                    this.AnimationSteps.RemoveAt(this.AnimationSteps.Count - 1);
                    if (!this.AnimationSteps.Any())
                    {
                        break;
                    }
                }
            }
        }
    }
}
