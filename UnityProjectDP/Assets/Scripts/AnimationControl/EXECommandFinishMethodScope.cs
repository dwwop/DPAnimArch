using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OALProgramControl
{
    public class EXECommandFinishMethodScope : EXECommand
    {
        public readonly MethodCallRecord MethodInfo;

        public EXECommandFinishMethodScope(MethodCallRecord methodInfo)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException();
            }

            this.MethodInfo = methodInfo;
        }

        public override EXECommand CreateClone()
        {
            return new EXECommandFinishMethodScope(this.MethodInfo);
        }

        protected override bool Execute(OALProgram OALProgram)
        {
            int lastIndex = OALProgram.CommandStack.CallStack.Count - 1;

            if (!OALProgram.CommandStack.CallStack[lastIndex].Matches(this.MethodInfo))
            {
                throw new Exception("Method info mismatch");
            }

            OALProgram.CommandStack.CallStack.RemoveAt(lastIndex);
            return true;
        }
    }
}