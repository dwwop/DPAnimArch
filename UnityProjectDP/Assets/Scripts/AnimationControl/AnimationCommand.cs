using System.Collections;
using UnityEngine; // Filip

namespace OALProgramControl
{
    public class AnimationCommand
    {
        private EXEScope SuperScope { get; }
        private EXECommand DecoratedCommand { get; }
        public bool IsCall { get; }
        public AnimationCommand(EXEScope SuperScope, EXECommand DecoratedCommand)
        {
            this.SuperScope = SuperScope;
            this.DecoratedCommand = DecoratedCommand;
            this.IsCall = DecoratedCommand is EXECommandCall;
        }

        public IEnumerator Execute()
        {
            if (this.IsCall)
            {
                OALCall Call = ((EXECommandCall)this.DecoratedCommand).CreateOALCall();
                yield return Animation.Instance.ResolveCallFunct(Call);

               /*Color c1 = Animation.Instance.classColor;// Filip
                Color c2 = Animation.Instance.methodColor;// Filip
                Color c3 = Animation.Instance.relationColor;// Filip

                Animation.Instance.classColor = Color.blue;// Filip
                Animation.Instance.methodColor = Color.yellow;// Filip
                Animation.Instance.relationColor = Color.blue;// Filip
                yield return Animation.Instance.ResolveCallFunct(Call); // Filip

                Animation.Instance.classColor = c1; // Filip
                Animation.Instance.methodColor = c2;// Filip
                Animation.Instance.relationColor = c3;// Filip*/
            }
            else
            {
                yield return DecoratedCommand.Execute(OALProgram.Instance, this.SuperScope);
            }

            Animation.Instance.IncrementBarrier();
        }
        public string ToCode()
        {
            return this.DecoratedCommand.ToCode();
        }
    }
}
