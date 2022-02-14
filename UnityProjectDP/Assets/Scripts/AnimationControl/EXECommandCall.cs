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
        private String CallerClass { get; }
        private String CalledClass { get; }
        private String CallerMethod { get; }
        private String CalledMethod { get; }
        private String RelationshipName { get; }
        private String InstanceName { get; }////
        private String AttributeName { get; }////
        private String MethodName { get; }////
        private List<EXEASTNode> Parameters { get; }////

        public EXECommandCall(String CallerClass, String CallerMethod, String RelationshipName, String CalledClass, String CalledMethod)
        {
            this.CallerClass = CallerClass;
            this.CallerMethod = CallerMethod;
            this.RelationshipName = RelationshipName;
            this.CalledClass = CalledClass;
            this.CalledMethod = CalledMethod;
        }

        public EXECommandCall(String InstanceName, String AttributeName, String MethodName, List<EXEASTNode> Parameters)////
        {
            this.InstanceName = InstanceName;
            this.AttributeName = AttributeName;
            this.MethodName = MethodName;
            this.Parameters = Parameters;
        }

        public override Boolean Execute(OALProgram OALProgram)
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

            CDMethod Method = Class.getMethodByName(this.MethodName);

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
            return new OALCall(this.CallerClass, this.CallerMethod, this.RelationshipName, this.CalledClass, this.CalledMethod);
        }

        public override String ToCodeSimple()
        {
            return "call from " + this.CallerClass + "::" + this.CallerMethod + "() to "
                + this.CalledClass + "::" + this.CalledMethod + "() across " + this.RelationshipName;
        }
    }
}
