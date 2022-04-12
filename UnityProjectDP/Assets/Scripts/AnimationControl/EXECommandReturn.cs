
namespace OALProgramControl
{
    public class EXECommandReturn : EXECommand
    {
        private EXEASTNode Expression { get; }

        public EXECommandReturn(EXEASTNode Expression)
        {
            this.Expression = Expression;
        }

        protected override bool Execute(OALProgram OALProgram)
        {
            return true;
        }

        public override string ToCodeSimple()
        {
            return this.Expression == null ? "return" : ("return " + this.Expression.ToCode());
        }

        public override EXECommand CreateClone()
        {
            return new EXECommandReturn(Expression);
        }
    }
}
