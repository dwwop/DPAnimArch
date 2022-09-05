using System;

namespace OALProgramControl
{
    public class EXECommandQueryCreate : EXECommand
    {
        public String ReferencingVariableName { get; }
        private String ReferencingAttributeName { get; }
        public String ClassName { get; }

        public EXECommandQueryCreate(String ClassName, String ReferencingVariableName, String ReferencingAttributeName)
        {
            this.ReferencingVariableName = ReferencingVariableName;
            this.ReferencingAttributeName = ReferencingAttributeName;
            this.ClassName = ClassName;
        }

        public EXECommandQueryCreate(String ClassName)
        {
            ReferencingVariableName = "";
            ReferencingAttributeName = null;
            this.ClassName = ClassName;
        }

        // SetUloh2
        protected override bool Execute(OALProgram OALProgram)
        {
            //Create an instance of given class -> will affect ExecutionSpace.
            //If ReferencingVariableName is provided (is not ""), create a referencing variable pointing to this instance -> will affect scope
            CDClass Class = OALProgram.ExecutionSpace.getClassByName(ClassName);
            if (Class == null)
            {
                return false;
            }

            EXEReferencingVariable Variable = SuperScope.FindReferencingVariableByName(ReferencingVariableName);

            if (ReferencingAttributeName == null)
            {
                if (Variable != null && !String.Equals(ClassName, Variable.ClassName))
                {
                    return false;
                }

                CDClassInstance NewInstance = Class.CreateClassInstance();
                if (NewInstance == null)
                {
                    return false;
                }

                if (!"".Equals(ReferencingVariableName))
                {
                    if (Variable != null)
                    {
                        Variable.ReferencedInstanceId = NewInstance.UniqueID;
                    }
                    else
                    {
                        Variable = new EXEReferencingVariable(ReferencingVariableName, Class.Name,
                            NewInstance.UniqueID);
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

                CDAttribute Attribute = VariableClass.GetAttributeByName(ReferencingAttributeName);
                if (Attribute == null)
                {
                    return false;
                }

                if (!String.Equals(ClassName, Attribute.Type))
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

                return ClassInstance.SetAttribute(ReferencingAttributeName, NewInstance.UniqueID.ToString());
            }

            return true;
        }

        public override string ToCodeSimple()
        {
            return "create object instance "
                   + ("".Equals(ReferencingVariableName) ? "" :
                       ReferencingAttributeName == null ? (ReferencingVariableName + " ") :
                       (ReferencingVariableName + "." + ReferencingAttributeName + " "))
                   + "of " + ClassName;
        }

        public override EXECommand CreateClone()
        {
            return new EXECommandQueryCreate(ClassName, ReferencingVariableName, ReferencingAttributeName);
        }
    }
}