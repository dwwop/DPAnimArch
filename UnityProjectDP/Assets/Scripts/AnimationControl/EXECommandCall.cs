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
        public override Boolean SynchronizedExecute(OALProgram OALProgram, EXEScope Scope)
        {
            Boolean Success = this.Execute(OALProgram, Scope);
            return Success;
        }

        public override Boolean Execute(OALProgram OALProgram, EXEScope Scope)
        {
            //OALProgram.RequestNextStep();

            OALProgram.ThreadSyncer.RequestStep(this, Scope, null);

            return true;
        }

        public override Boolean PreExecute(AnimationCommandStorage ACS, OALProgram OALProgram, EXEScope Scope)
        {
            OALProgram.ThreadSyncer.RequestStep(this, Scope, ACS);
            //ACS.AddAnimationStep(new AnimationCommand(Scope, this));
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
