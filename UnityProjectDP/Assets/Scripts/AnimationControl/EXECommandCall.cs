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

        public EXECommandCall(String CallerClass, String CallerMethod, String RelationshipName, String CalledClass, String CalledMethod)
        {
            this.CallerClass = CallerClass;
            this.CallerMethod = CallerMethod;
            this.RelationshipName = RelationshipName;
            this.CalledClass = CalledClass;
            this.CalledMethod = CalledMethod;
        }

        public override Boolean Execute(OALProgram OALProgram)
        {
            //Filip, ak mas null v executablecode tak vrat true inak toto dole
            EXEScopeMethod MethodCode = OALProgram.ExecutionSpace.getClassByName(this.CalledClass).getMethodByName(this.CalledMethod).ExecutableCode;
            MethodCode.SetSuperScope(this.SuperScope);
            OALProgram.CommandStack.Enqueue(MethodCode);

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
