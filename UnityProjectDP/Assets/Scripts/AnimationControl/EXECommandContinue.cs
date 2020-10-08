namespace OALProgramControl
{
    public class EXECommandContinue : EXECommand
    {
        public override bool Execute(OALProgram OALProgram, EXEScope Scope)
        {
            return Scope.PropagateControlCommand(LoopControlStructure.Continue);
        }
        public override string ToCodeSimple()
        {
            return "continue";
        }
    }
}
