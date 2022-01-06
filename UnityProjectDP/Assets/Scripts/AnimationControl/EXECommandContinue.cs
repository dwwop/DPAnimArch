namespace OALProgramControl
{
    public class EXECommandContinue : EXECommand
    {
        public override bool Execute(OALProgram OALProgram)
        {
            return true;//return SuperScope.PropagateControlCommand(LoopControlStructure.Continue);
        }
        public override string ToCodeSimple()
        {
            return "continue";
        }
    }
}
