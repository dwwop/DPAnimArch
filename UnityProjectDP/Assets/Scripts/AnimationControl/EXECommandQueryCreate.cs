using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public class EXECommandQueryCreate : EXECommand
    {
        private String ReferencingVariableName { get; }
        private String ClassName { get; }

        public EXECommandQueryCreate(String ClassName, String ReferencingVariableName)
        {
            this.ReferencingVariableName = ReferencingVariableName;
            this.ClassName = ClassName;
        }

        public EXECommandQueryCreate(String ClassName)
        {
            this.ReferencingVariableName = "";
            this.ClassName = ClassName;
        }

        // SetUloh2
        public override bool Execute(OALProgram OALProgram, EXEScope Scope)
        {
            //Create an instance of given class -> will affect ExecutionSpace.
            //If ReferencingVariableName is provided (is not ""), create a referencing variable pointing to this instance -> will affect scope
            CDClass Class = OALProgram.ExecutionSpace.getClassByName(this.ClassName);
            if (Class == null)
            {
                return false;
            }

            EXEReferencingVariable Variable = Scope.FindReferencingVariableByName(this.ReferencingVariableName);
            if (Variable != null)
            {
                if (!String.Equals(this.ClassName, Variable.ClassName))
                {
                    return false;
                }
            }

            CDClassInstance Instance = Class.CreateClassInstance();
            if (Instance == null)
            {
                return false;
            }

            if (!"".Equals(this.ReferencingVariableName))
            {
                if (Variable != null)
                {
                    
                    Variable.ReferencedInstanceId = Instance.UniqueID;
                }
                else
                {
                    Variable = new EXEReferencingVariable(this.ReferencingVariableName, Class.Name, Instance.UniqueID);
                    return Scope.AddVariable(Variable);
                }
            }

            return true;
        }
        public override string ToCodeSimple()
        {
            return "create object instance "
                + ("".Equals(this.ReferencingVariableName) ? "" : (this.ReferencingVariableName + " "))
                + "of " + this.ClassName;
        }
    }
}
