using System;
using System.Collections.Generic;
using System.Linq;
using AnimArch.Extensions;
using UnityEngine;
using Object = System.Object;

namespace OALProgramControl
{
    public class EXECommandCall : EXECommand
    {
        public String CalledClass { get; set; }
        private String CalledMethod { get; }
        public String InstanceName { get; }
        public String AttributeName { get; }
        private List<EXEASTNode> Parameters { get; }

        public MethodCallRecord CallerMethodInfo
        {
            get
            {
                EXEScopeMethod TopScope = (EXEScopeMethod) GetTopLevelScope();
                return TopScope.MethodDefinition;
            }
        }

        public EXECommandCall(String InstanceName, String AttributeName, String MethodName, List<EXEASTNode> Parameters)
        {
            this.InstanceName = InstanceName;
            this.AttributeName = AttributeName;
            this.CalledMethod = MethodName;
            this.Parameters = Parameters;
        }

        protected override Boolean Execute(OALProgram OALProgram)
        {
            EXEReferencingVariable Reference = this.SuperScope.FindReferencingVariableByName(this.InstanceName);

            if (Reference == null)
            {
                return false;
            }

            CDClass Class = OALProgram.ExecutionSpace.getClassByName(Reference.ClassName);

            if (Class == null)
            {
                return false;
            }

            Class = Class.GetInstanceClassByIDRecursiveDownward(Reference.ReferencedInstanceId);

            if (Class == null)
            {
                return false;
            }

            long CalledID = Reference.ReferencedInstanceId;

            if (this.AttributeName != null)
            {
                CDAttribute Attribute = Class.GetAttributeByName(this.AttributeName);

                if (Attribute == null)
                {
                    return false;
                }

                CDClass AtrributeClass = OALProgram.ExecutionSpace.getClassByName(Attribute.Type);

                if (AtrributeClass == null)
                {
                    return false;
                }

                CalledID = long.Parse(Class.GetInstanceByID(Reference.ReferencedInstanceId).State[Attribute.Name]);
                Class = AtrributeClass.GetInstanceClassByIDRecursiveDownward(CalledID);
            }

            this.CalledClass = Class.Name;

            CDMethod Method = Class.getMethodByName(this.CalledMethod);

            if (Method == null)
            {
                return false;
            }

            EXEScopeMethod MethodCode = Method.ExecutableCode;

            if (MethodCode == null)
            {
                return true;
            }

            MethodCode.SetSuperScope(null);
            OALProgram.CommandStack.Enqueue(MethodCode);
            MethodCode.AddVariable(new EXEReferencingVariable("self", CalledClass, CalledID)); //

            for (int i = 0; i < this.Parameters.Count; i++)
            {
                CDParameter Parameter = Method.Parameters[i];
                if
                (
                    !(
                        Parameter.Type != null
                        &&
                        this.Parameters[i].IsReference().Implies
                        (
                            Object.Equals(Parameter.Type,
                                this.SuperScope.DetermineVariableType(this.Parameters[i].AccessChain(),
                                    OALProgram.ExecutionSpace))
                        )
                    )
                    &&
                    !(
                        OALProgram.Instance.ExecutionSpace.ClassExists(Parameter.Type)
                        ||
                        OALProgram.Instance.ExecutionSpace.ClassExists(
                            Parameter.Type.Substring(0, Parameter.Type.Length - 2))
                    )
                )
                {
                    return false;
                }

                if (EXETypes.IsPrimitive(Parameter.Type))
                {
                    String Value = this.Parameters[i].Evaluate(this.SuperScope, OALProgram.ExecutionSpace);

                    if (!EXETypes.IsValidValue(Value, Parameter.Type))
                    {
                        return false;
                    }

                    MethodCode.AddVariable(new EXEPrimitiveVariable(Parameter.Name, Value, Parameter.Type));
                }
                else if ("[]".Equals(Parameter.Type.Substring(Parameter.Type.Length - 2, 2)))
                {
                    CDClass ClassDefinition =
                        OALProgram.ExecutionSpace.getClassByName(Parameter.Type.Substring(0,
                            Parameter.Type.Length - 2));
                    if (ClassDefinition == null)
                    {
                        return false;
                    }

                    String Values = this.Parameters[i].Evaluate(this.SuperScope, OALProgram.ExecutionSpace);

                    if (!EXETypes.IsValidReferenceValue(Values, Parameter.Type))
                    {
                        return false;
                    }

                    long[] IDs = String.Empty.Equals(Values)
                        ? new long[] { }
                        : Values.Split(',').Select(id => long.Parse(id)).ToArray();

                    CDClassInstance ClassInstance;
                    foreach (long ID in IDs)
                    {
                        ClassInstance = ClassDefinition.GetInstanceByID(ID);
                        if (ClassInstance == null)
                        {
                            return false;
                        }
                    }

                    EXEReferencingSetVariable CreatedSetVariable =
                        new EXEReferencingSetVariable(Parameter.Name, ClassDefinition.Name);

                    foreach (long ID in IDs)
                    {
                        CreatedSetVariable.AddReferencingVariable(
                            new EXEReferencingVariable("", ClassDefinition.Name, ID));
                    }

                    MethodCode.AddVariable(CreatedSetVariable);
                }
                else if (!String.IsNullOrEmpty(Parameter.Type))
                {
                    CDClass ClassDefinition = OALProgram.ExecutionSpace.getClassByName(Parameter.Type);
                    if (ClassDefinition == null)
                    {
                        return false;
                    }

                    string Value = Parameters[i].Evaluate(this.SuperScope, OALProgram.ExecutionSpace);

                    if (!EXETypes.IsValidReferenceValue(Value, Parameter.Type))
                    {
                        return false;
                    }

                    long ID = long.Parse(Value);

                    CDClassInstance ClassInstance = ClassDefinition.GetInstanceByIDRecursiveDownward(ID);
                    if (ClassInstance == null)
                    {
                        return false;
                    }

                    MethodCode.AddVariable(new EXEReferencingVariable(Parameter.Name, ClassDefinition.Name, ID));
                }
            }

            return true;
        }

        public OALCall CreateOALCall()
        {
            MethodCallRecord _CallerMethodInfo = this.CallerMethodInfo;
            CDRelationship _RelationshipInfo = CallRelationshipInfo(_CallerMethodInfo.ClassName, this.CalledClass);
            bool IsSelfCall = string.Equals(this.CalledClass, _CallerMethodInfo.ClassName);
            return new OALCall
            (
                _CallerMethodInfo.ClassName,
                _CallerMethodInfo.MethodName,
                IsSelfCall ? null : _RelationshipInfo.RelationshipName,
                this.CalledClass,
                this.CalledMethod,
                SuperScope.FindReferencingVariableByName(InstanceName).ReferencedInstanceId,
                IsSelfCall
            );
        }

        public override String ToCodeSimple()
        {
            MethodCallRecord _CallerMethodInfo = this.CallerMethodInfo;
            CDRelationship _RelationshipInfo = CallRelationshipInfo(_CallerMethodInfo.ClassName, this.CalledClass);
            return "call from " + _CallerMethodInfo.ClassName + "::" + _CallerMethodInfo.MethodName + "() to "
                   + this.CalledClass + "::" + this.CalledMethod + "() across " + _RelationshipInfo.RelationshipName;
        }

        private CDRelationship CallRelationshipInfo(string CallerMethod, string CalledMethod)
        {
            return OALProgram.Instance.RelationshipSpace.GetRelationshipByClasses(CallerMethod, CalledMethod);
        }

        public override EXECommand CreateClone()
        {
            return new EXECommandCall(InstanceName, AttributeName, CalledMethod, Parameters);
        }
    }
}
