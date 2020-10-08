using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public class EXEReferenceEvaluator
    {
        //SetUloh1
        // We have variable name, attribute name and scope, in which to look for variable
        // We need to get the value of given attribute of given variable
        // If this does not exist, return null
        // You will use EXEScope.FindReferencingVariableByName() method, but you need to implement it first
        // user.name

        public String EvaluateAttributeValue(String ReferencingVariableName, String AttributeName, EXEScope Scope, CDClassPool ExecutionSpace)
        {
            EXEReferencingVariable ReferencingVariable = Scope.FindReferencingVariableByName(ReferencingVariableName);
            if (ReferencingVariable == null)
            {
                return null;
            }
            CDClassInstance ClassInstance = ExecutionSpace.GetClassInstanceById(ReferencingVariable.ClassName, ReferencingVariable.ReferencedInstanceId);
            if (ClassInstance == null)
            {
                return null;
            }
            return ClassInstance.GetAttributeValue(AttributeName);
        }

        //SetUloh1
        // Similar as task above, but this time we set the attribute value to "NewValue" parameter
        // But it's not that easy, you need to check if attribute type and NewValue type are the same (e.g. both are integer)
        // To do that, you need to find the referencing variable's class (via Scope) and then the attribute's type (vie ExecutionSpace)
        // When you know the type of attribute, use EXETypes.IsValidValue to see if you can or cannot assign that value to that attribute
        // You assign it in Scope
        // Return if you could assign it or not
        // EXETypes.determineVariableType()
        public Boolean SetAttributeValue(String ReferencingVariableName, String AttributeName, EXEScope Scope, CDClassPool ExecutionSpace, String NewValue)
        {
            EXEReferencingVariable ReferencingVariable = Scope.FindReferencingVariableByName(ReferencingVariableName);
            if (ReferencingVariable == null) return false;

            CDClassInstance ClassInstance = ExecutionSpace.GetClassInstanceById(ReferencingVariable.ClassName, ReferencingVariable.ReferencedInstanceId);
            if (ClassInstance == null) return false;

            CDClass Class = ExecutionSpace.getClassByName(ReferencingVariable.ClassName);
            if (Class == null) return false;

            CDAttribute Attribute = Class.GetAttributeByName(AttributeName);
            if (Attribute == null) return false;

            String NewValueType = EXETypes.DetermineVariableType(null, NewValue);
            if (!EXETypes.CanBeAssignedToAttribute(AttributeName, Attribute.Type, NewValueType)) return false;

            ClassInstance.SetAttribute(AttributeName, EXETypes.AdjustAssignedValue(Attribute.Type, NewValue));

            return true;
        }
       
    }
}
