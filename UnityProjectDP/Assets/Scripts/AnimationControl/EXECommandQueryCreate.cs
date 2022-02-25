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
        private String ReferencingAttributeName { get; }
        private String ClassName { get; }

        public EXECommandQueryCreate(String ClassName, String ReferencingVariableName, String ReferencingAttributeName)
        {
            this.ReferencingVariableName = ReferencingVariableName;
            this.ReferencingAttributeName = ReferencingAttributeName;
            this.ClassName = ClassName;
        }

        public EXECommandQueryCreate(String ClassName)
        {
            this.ReferencingVariableName = "";
            this.ReferencingAttributeName = null;
            this.ClassName = ClassName;
        }

        // SetUloh2
        protected override bool Execute(OALProgram OALProgram)
        {
            //Create an instance of given class -> will affect ExecutionSpace.
            //If ReferencingVariableName is provided (is not ""), create a referencing variable pointing to this instance -> will affect scope
            CDClass Class = OALProgram.ExecutionSpace.getClassByName(this.ClassName);
            if (Class == null)
            {
                return false;
            }

            EXEReferencingVariable Variable = SuperScope.FindReferencingVariableByName(this.ReferencingVariableName);

            if (this.ReferencingAttributeName == null)
            {
                if (Variable != null)
                {
                    if (!String.Equals(this.ClassName, Variable.ClassName))
                    {
                        return false;
                    }
                }

                CDClassInstance NewInstance = Class.CreateClassInstance();
                if (NewInstance == null)
                {
                    return false;
                }

                if (!"".Equals(this.ReferencingVariableName))
                {
                    if (Variable != null)
                    {

                        Variable.ReferencedInstanceId = NewInstance.UniqueID;
                    }
                    else
                    {
                        Variable = new EXEReferencingVariable(this.ReferencingVariableName, Class.Name, NewInstance.UniqueID);
                        return SuperScope.AddVariable(Variable);
                    }
                }
            }
            else
            {
                if (Variable == null)
                {
                    return false;
                }

                CDClass VariableClass = OALProgram.ExecutionSpace.getClassByName(Variable.ClassName);
                if (VariableClass == null)
                {
                    return false;
                }

                CDAttribute Attribute = VariableClass.GetAttributeByName(this.ReferencingAttributeName);
                if (Attribute == null)
                {
                    return false;
                }

                if (!String.Equals(this.ClassName, Attribute.Type))
                {
                    return false;
                }
                
                CDClassInstance ClassInstance = VariableClass.GetInstanceByID(Variable.ReferencedInstanceId);
                if (ClassInstance == null)
                {
                    return false;
                }

                CDClassInstance NewInstance = Class.CreateClassInstance();
                if (NewInstance == null)
                {
                    return false;
                }

                return ClassInstance.SetAttribute(this.ReferencingAttributeName, NewInstance.UniqueID.ToString());
            }

            return true;
        }
        public override string ToCodeSimple()
        {
            return "create object instance "
                + ("".Equals(this.ReferencingVariableName) ? "" : this.ReferencingAttributeName == null ? (this.ReferencingVariableName + " ") : (this.ReferencingVariableName + "." + this.ReferencingAttributeName + " "))
                + "of " + this.ClassName;
        }
    }
}
