namespace OALProgramControl
{
    public class EXECommandBreak : EXECommand
    {
        public override bool Execute(OALProgram OALProgram, EXEScope Scope)
        {
            return Scope.PropagateControlCommand(LoopControlStructure.Break);
        }
        public override string ToCodeSimple()
        {
            return "break";
        }
    }
}
