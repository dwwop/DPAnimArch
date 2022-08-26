using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public class EXEReferenceEvaluator
    {
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

        public Boolean SetAttributeValue(String ReferencingVariableName, String AttributeName, EXEScope Scope, CDClassPool ExecutionSpace, String NewValue, String NewValueType)
        {
            EXEReferencingVariable ReferencingVariable = Scope.FindReferencingVariableByName(ReferencingVariableName);
            if (ReferencingVariable == null) return false;

            CDClassInstance ClassInstance = ExecutionSpace.GetClassInstanceById(ReferencingVariable.ClassName, ReferencingVariable.ReferencedInstanceId);
            if (ClassInstance == null) return false;

            CDClass Class = ExecutionSpace.getClassByName(ReferencingVariable.ClassName);
            if (Class == null) return false;
            //TODO: Typ attributu nemoze byt ReferenceTypeName alebo UnitializedTypeName ci ?
            CDAttribute Attribute = Class.GetAttributeByName(AttributeName);
            if (Attribute == null) return false;

            if (!EXETypes.CanBeAssignedToAttribute(AttributeName, Attribute.Type, NewValueType))
            {
                return false;
            }

            if (EXETypes.IsPrimitive(Attribute.Type))
            {
                if (!EXETypes.IsValidValue(NewValue, Attribute.Type))
                {
                    return false;
                }

                return ClassInstance.SetAttribute(AttributeName, EXETypes.AdjustAssignedValue(Attribute.Type, NewValue));
            }
            else if ("[]".Equals(Attribute.Type.Substring(Attribute.Type.Length - 2, 2)))
            {
                CDClass AttributeClass = ExecutionSpace.getClassByName(Attribute.Type.Substring(0, Attribute.Type.Length - 2));
                if (AttributeClass == null)
                {
                    return false;
                }

                if (!EXETypes.IsValidReferenceValue(NewValue, Attribute.Type))
                {
                    return false;
                }

                long[] IDs = String.Empty.Equals(NewValue) ? new long[] { } : NewValue.Split(',').Select(id => long.Parse(id)).ToArray();

                CDClassInstance Instance;
                foreach (long ID in IDs)
                {
                    Instance = AttributeClass.GetInstanceByID(ID);
                    if (Instance == null)
                    {
                        return false;
                    }
                }

                return ClassInstance.SetAttribute(AttributeName, NewValue);
            }
            else if (!String.IsNullOrEmpty(Attribute.Type))
            {
                CDClass AttributeClass = ExecutionSpace.getClassByName(Attribute.Type);
                if (AttributeClass == null)
                {
                    return false;
                }

                CDClass NewValueClass = ExecutionSpace.getClassByName(NewValueType);
                if (NewValueClass == null)
                {
                    return false;
                }

                if (!EXETypes.IsValidReferenceValue(NewValue, AttributeClass.Name))
                {
                    return false;
                }

                long IDValue = long.Parse(NewValue);

                CDClassInstance Instance = NewValueClass.GetInstanceByID(IDValue);
                if (Instance == null)
                {
                    return false;
                }

                return ClassInstance.SetAttribute(AttributeName, NewValue);
            }

            return false;
        }     
    }
}
