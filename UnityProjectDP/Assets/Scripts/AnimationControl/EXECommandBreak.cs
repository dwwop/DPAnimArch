namespace OALProgramControl
{
    public class EXECommandBreak : EXECommand
    {
        public override bool Execute(OALProgram OALProgram)
        {
            return SuperScope.PropagateControlCommand(LoopControlStructure.Break);
        }
        public override string ToCodeSimple()
        {
            return "break";
        }
    }
}
