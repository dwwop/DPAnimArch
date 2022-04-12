namespace OALProgramControl
{
    public class EXECommandBreak : EXECommand
    {
        protected override bool Execute(OALProgram OALProgram)
        {
            return true;//return SuperScope.PropagateControlCommand(LoopControlStructure.Break);
        }
        public override string ToCodeSimple()
        {
            return "break";
        }

        public override EXECommand CreateClone()
        {
            return new EXECommandBreak();
        }
    }
}
