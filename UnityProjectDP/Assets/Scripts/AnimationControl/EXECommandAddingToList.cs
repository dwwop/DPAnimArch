using System;
using System.Collections;
using System.Collections.Generic;

namespace OALProgramControl
{
    public class EXECommandAddingToList : EXECommand
    {
        private String VariableName { get; }
        private String AttributeName { get; }
        private EXEASTNode Item { get; }

        public EXECommandAddingToList(String VariableName, String AttributeName, EXEASTNode Item)
        {
            this.VariableName = VariableName;
            this.AttributeName = AttributeName;
            this.Item = Item;
        }

        protected override bool Execute(OALProgram OALProgram)
        {
            String SetVariableClassName;
            EXEReferencingSetVariable SetVariable = null; // This is important if we do not have AttributeName
            CDClassInstance ClassInstance = null; // This is important if we have AttributeName

            if (this.AttributeName == null)
            {
                // If we do not have AttributeName, VariableName must be set variable reference
                SetVariable = SuperScope.FindSetReferencingVariableByName(this.VariableName);
                if (SetVariable == null)
                {
                    return false;
                }

                SetVariableClassName = SetVariable.ClassName;
            }
            else
            {
                // If we have AttributeName, VariableName must be reference (not set variable)
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

                // We need to check if it is list
                if (!"[]".Equals(Attribute.Type.Substring(Attribute.Type.Length - 2, 2)))
                {
                    return false; 
                }

                SetVariableClassName = Attribute.Type.Substring(0, Attribute.Type.Length - 2);

                ClassInstance = VariableClass.GetInstanceByID(Variable.ReferencedInstanceId);
                if (ClassInstance == null)
                {
                    return false;
                }
            }

            // We need to compare class types
            if
            (
                !(
                    this.Item.IsReference()
                    &&
                    Object.Equals(SetVariableClassName, this.SuperScope.DetermineVariableType(this.Item.AccessChain(), OALProgram.ExecutionSpace))
                )
            )
            {
                return false;
            }

            String IDValue = this.Item.Evaluate(SuperScope, OALProgram.ExecutionSpace);

            if (!EXETypes.IsValidReferenceValue(IDValue, SetVariableClassName))
            {
                return false;
            }

            long ItemInstanceID = long.Parse(IDValue);

            CDClass SetVariableClass = OALProgram.ExecutionSpace.getClassByName(SetVariableClassName);
            if (SetVariableClass == null)
            {
                return false;
            }

            CDClassInstance Instance = SetVariableClass.GetInstanceByIDRecursiveDownward(ItemInstanceID);
            if (Instance == null)
            {
                return false;
            }


            if (this.AttributeName == null)
            {
                SetVariable.AddReferencingVariable(new EXEReferencingVariable("", SetVariableClassName, ItemInstanceID));
            }
            else
            {
                String Values = ClassInstance.GetAttributeValue(this.AttributeName);

                if (Values.Length > 0 && !EXETypes.UnitializedName.Equals(Values))
                {
                    Values += "," + ItemInstanceID.ToString();
                    ClassInstance.SetAttribute(this.AttributeName, Values);
                }
                else
                {
                    ClassInstance.SetAttribute(this.AttributeName, ItemInstanceID.ToString());
                }
            }

            return true;
        }

        public override string ToCodeSimple()
        {
            return "add " + this.Item.ToCode()
                + " to " + (this.AttributeName == null ? this.VariableName : (this.VariableName + "." + this.AttributeName));
        }

        public override EXECommand CreateClone()
        {
            return new EXECommandAddingToList(VariableName, AttributeName, Item);
        }
    }
}
