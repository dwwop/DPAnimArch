namespace OALProgramControl
{
    public class EXECommandContinue : EXECommand
    {
        public override bool Execute(OALProgram OALProgram)
        {
            return SuperScope.PropagateControlCommand(LoopControlStructure.Continue);
        }
        public override string ToCodeSimple()
        {
            return "continue";
        }
    }
}
