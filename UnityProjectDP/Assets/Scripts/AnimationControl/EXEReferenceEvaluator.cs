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
        public Boolean SetAttributeValue(String ReferencingVariableName, String AttributeName, EXEScope Scope, CDClassPool ExecutionSpace, String NewValue, String NewValueType)
        {
            EXEReferencingVariable ReferencingVariable = Scope.FindReferencingVariableByName(ReferencingVariableName);
            if (ReferencingVariable == null) return false;

            CDClassInstance ClassInstance = ExecutionSpace.GetClassInstanceById(ReferencingVariable.ClassName, ReferencingVariable.ReferencedInstanceId);
            if (ClassInstance == null) return false;

            CDClass Class = ExecutionSpace.getClassByName(ReferencingVariable.ClassName);
            if (Class == null) return false;

            //Typ attributu nemoze byt ReferenceTypeName alebo Unitiazed ci ?
            CDAttribute Attribute = Class.GetAttributeByName(AttributeName);
            if (Attribute == null) return false;

            if (!EXETypes.CanBeAssignedToAttribute(AttributeName, Attribute.Type, NewValueType)) return false;


            if (EXETypes.IsPrimitive(Attribute.Type))
            {
                if (!EXETypes.IsValidValue(NewValue, Attribute.Type))//ak vyssie comaparujeme(CanBeAssignedToAttribute), tak tu je to zbytocne
                {                                                    //zaroven to moze hodit false ak mame NewValue typu reference a value je meno
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

                if//ak vyssie comaparujeme, tak tu je to zbytocne
                (!Object.Equals(Attribute.Type, NewValueType))
                {
                    return false;
                }

                if (!EXETypes.IsValidReferenceValue(NewValue, Attribute.Type))
                {
                    return false;
                }

                long[] IDs = NewValue.Split(',').Select(id => long.Parse(id)).ToArray();

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

                if//ak vyssie comparujeme, tak tu je to zbytocne
                (!Object.Equals(AttributeClass.Name, NewValueType))
                {
                    return false;
                }

                if (!EXETypes.IsValidReferenceValue(NewValue, AttributeClass.Name))
                {
                    return false;
                }

                long IDValue = long.Parse(NewValue);

                CDClassInstance Instance = AttributeClass.GetInstanceByID(IDValue);
                if (Instance == null)
                {
                    return false;
                }

                return ClassInstance.SetAttribute(AttributeName, NewValue);
            }

            return false;



            ////////////////////////////////////////////////////////
            /*EXEReferencingVariable ReferencingVariable = Scope.FindReferencingVariableByName(ReferencingVariableName);
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

            return true;*/
            ////////////////////////////////////////////////////
        }
       
    }
}
