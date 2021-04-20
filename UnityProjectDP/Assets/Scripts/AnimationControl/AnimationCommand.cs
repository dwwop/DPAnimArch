using System.Collections;

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
