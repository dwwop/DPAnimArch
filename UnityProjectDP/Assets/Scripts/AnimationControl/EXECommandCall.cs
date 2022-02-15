using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using Assets.Scripts.Animation;

namespace OALProgramControl
{
    public class EXECommandCall : EXECommand
    {
        private String CalledClass { get; set; }
        private String CalledMethod { get; }
        private String InstanceName { get; }////
        private String AttributeName { get; }////
        private List<EXEASTNode> Parameters { get; }////
        private MethodCallRecord CallerMethodInfo
        {
            get
            {
                EXEScopeMethod TopScope = (EXEScopeMethod)GetTopLevelScope();
                return TopScope.MethodDefinition;
            }
        }

        public EXECommandCall(String InstanceName, String AttributeName, String MethodName, List<EXEASTNode> Parameters)////
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

            for (int i = 0; i < this.Parameters.Count; i++)
            {
                CDParameter Parameter = Method.Parameters[i];

                //TODO: Skontrolovat ci typ sedi
                if (EXETypes.IsPrimitive(Parameter.Type))
                {
                    String Value = Parameters[i].Evaluate(this.SuperScope, OALProgram.ExecutionSpace);
                    MethodCode.AddVariable(new EXEPrimitiveVariable(Parameter.Name, Value));
                }
                //TODO: Ak je referencny typ
            }

            return true;
        }

        public OALCall CreateOALCall()
        {
            MethodCallRecord _CallerMethodInfo = this.CallerMethodInfo;
            CDRelationship _RelationshipInfo = CallRelationshipInfo(_CallerMethodInfo.ClassName, this.CalledClass);
            return new OALCall
            (
                _CallerMethodInfo.ClassName,
                _CallerMethodInfo.MethodName,
                _RelationshipInfo.RelationshipName,
                this.CalledClass,
                this.CalledMethod
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
    }
}
