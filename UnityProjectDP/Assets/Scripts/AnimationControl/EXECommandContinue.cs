namespace OALProgramControl
{
    public class EXECommandContinue : EXECommand
    {
        protected override bool Execute(OALProgram OALProgram)
        {
            return true;//return SuperScope.PropagateControlCommand(LoopControlStructure.Continue);
        }
        public override string ToCodeSimple()
        {
            return "continue";
        }
    }
}
