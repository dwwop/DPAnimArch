using System;
using System.Collections;
using System.Collections.Generic;

namespace OALProgramControl
{
    public class EXECommandAddingToList : EXECommand
    {
        private String ItemName { get; }
        private String ItemAttributeName { get; }
        private String VariableName { get; }
        private String AttributeName { get; }

        public EXECommandAddingToList(String ItemName, String ItemAttributeName, String VariableName, String AttributeName)
        {
            this.ItemName = ItemName;
            this.ItemAttributeName = ItemAttributeName;
            this.VariableName = VariableName;
            this.AttributeName = AttributeName;
        }

        protected override bool Execute(OALProgram OALProgram)
        {
            // ItemName must be reference (not set variable)
            EXEReferencingVariable ItemVariable = SuperScope.FindReferencingVariableByName(this.ItemName);
            if (ItemVariable == null)
            {
                return false;
            }

            String ItemClassName = ItemVariable.ClassName;
            long ItemInstanceId = ItemVariable.ReferencedInstanceId;
            if (this.ItemAttributeName != null)
            {
                CDClass ItemClass = OALProgram.ExecutionSpace.getClassByName(ItemVariable.ClassName);
                if (ItemClass == null)
                {
                    return false;
                }

                CDAttribute ItemAttribute = ItemClass.GetAttributeByName(this.ItemAttributeName);
                if (ItemAttribute == null)
                {
                    return false;
                }

                ItemClassName = ItemAttribute.Type;

                CDClassInstance ItemClassInstance = ItemClass.GetInstanceByID(ItemVariable.ReferencedInstanceId);
                if (ItemClassInstance == null)
                {
                    return false;
                }

                // ItemAttributeName must be reference (not set variable)
                if (!long.TryParse(ItemClassInstance.GetAttributeValue(this.ItemAttributeName), out ItemInstanceId))
                {
                    return false;
                }
            }

          
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
            if (!ItemClassName.Equals(SetVariableClassName))
            {
                return false;
            }

            if (this.AttributeName == null)
            {
                SetVariable.AddReferencingVariable(new EXEReferencingVariable("", SetVariableClassName, ItemInstanceId));
            }
            else
            {
                String Values = ClassInstance.GetAttributeValue(this.AttributeName);

                if (Values.Length > 0 && !EXETypes.UnitializedName.Equals(Values))
                {
                    Values += "," + ItemInstanceId.ToString();
                    ClassInstance.SetAttribute(this.AttributeName, Values);
                }
                else
                {
                    ClassInstance.SetAttribute(this.AttributeName, ItemInstanceId.ToString());
                }
            }

            return true;
        }

        public override string ToCodeSimple()
        {
            return "add " + (this.ItemAttributeName == null ? this.ItemName : (this.ItemName + "." + this.ItemAttributeName))
                + " to " + (this.AttributeName == null ? this.VariableName : (this.VariableName + "." + this.AttributeName));
        }
    }
}
