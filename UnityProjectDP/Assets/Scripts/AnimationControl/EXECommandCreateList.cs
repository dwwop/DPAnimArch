using System;
using System.Collections.Generic;
using System.Linq;

namespace OALProgramControl
{
    public class EXECommandCreateList : EXECommand
    {
        private String VariableName { get; }
        private String AttributeName { get; }
        private String ClassName { get; }
        private List<EXEASTNode> Items { get; }

        public EXECommandCreateList(String VariableName, String AttributeName, String ClassName, List<EXEASTNode> Items)
        {
            this.VariableName = VariableName;
            this.AttributeName = AttributeName;
            this.ClassName = ClassName;
            this.Items = Items;
        }

        protected override Boolean Execute(OALProgram OALProgram)
        {
            CDClass Class = OALProgram.ExecutionSpace.getClassByName(this.ClassName);
            if (Class == null)
            {
                return false;
            }

            EXEReferencingSetVariable SetVariable = null; // Important if we do not have AttributeName
            CDClassInstance ClassInstance = null; // Important if we have AttributeName

            if (this.AttributeName == null)
            {
                SetVariable = SuperScope.FindSetReferencingVariableByName(this.VariableName);

                if (SetVariable != null)
                {
                    if (!String.Equals(this.ClassName, SetVariable.ClassName))
                    {
                        return false;
                    }

                    SetVariable.ClearVariables();
                }
                else
                {
                    SetVariable = new EXEReferencingSetVariable(this.VariableName, Class.Name);
                    if (!SuperScope.AddVariable(SetVariable))
                    {
                        return false;
                    }
                }
            }
            else
            {
                EXEReferencingVariable Variable = SuperScope.FindReferencingVariableByName(this.VariableName);
                if (Variable == null)
                {
                    return false;
                }

                CDClass VariableClass = OALProgram.ExecutionSpace.getClassByName(Variable.ClassName);
                if (VariableClass == null)
                {
                    return false;
                }

                CDAttribute Attribute = VariableClass.GetAttributeByName(this.AttributeName);
                if (Attribute == null)
                {
                    return false;
                }
                
                String AttributeClass = Attribute.Type.Substring(0, Attribute.Type.Length - 2);
                if (!String.Equals(this.ClassName, AttributeClass))
                {
                    return false;
                }

                ClassInstance = VariableClass.GetInstanceByID(Variable.ReferencedInstanceId);
                if (ClassInstance == null)
                {
                    return false;
                }

                if (!ClassInstance.SetAttribute(this.AttributeName, ""))
                {
                    return false;
                }
            }

            if (this.Items.Any())
            {
                String Result = "";

                foreach (EXEASTNode item in this.Items)
                {
                    if
                    (
                        !(
                            item.IsReference()
                            &&
                            Object.Equals(Class.Name, this.SuperScope.DetermineVariableType(item.AccessChain(), OALProgram.ExecutionSpace))    
                        )
                    )
                    {
                        return false;
                    }

                    String IDValue = item.Evaluate(SuperScope, OALProgram.ExecutionSpace);

                    if (!EXETypes.IsValidReferenceValue(IDValue, Class.Name))
                    {
                        return false;
                    }

                    long ID = long.Parse(IDValue);

                    CDClassInstance Instance = Class.GetInstanceByID(ID);
                    if (Instance == null)
                    {
                        return false;
                    }

                    if (this.AttributeName == null)
                    {
                        SetVariable.AddReferencingVariable(new EXEReferencingVariable("", Class.Name, ID));
                    }
                    else
                    {
                        Result += IDValue + ",";
                    }
                }

                if (this.AttributeName != null)
                {
                    Result = Result.Remove(Result.Length - 1, 1); // Remove last ","
                    ClassInstance.SetAttribute(this.AttributeName, Result);
                }
            }
            
            return true;
        }

        public override string ToCodeSimple()
        {
            String Result = "create list " + (this.AttributeName == null ? this.VariableName : (this.VariableName + "." + this.AttributeName))
                + " of " + this.ClassName;

            if (this.Items.Any())
            {
                Result += " { " + this.Items[0].ToCode();

                for (int i = 1; i < this.Items.Count; i++)
                {
                    Result += ", " + this.Items[i].ToCode();
                }
                Result += " }";
            }
            
            return Result;
        }

        public override EXECommand CreateClone()
        {
            return new EXECommandCreateList(VariableName, AttributeName, ClassName, Items);
        }
    }
}
