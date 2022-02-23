using System;
using System.Linq;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OALProgramControl
{
    public class OALProgram : Singleton<OALProgram>
    {
        public CDClassPool ExecutionSpace { get; set; }
        public CDRelationshipPool RelationshipSpace { get; set; }

        private EXEScope _SuperScope;
        public EXEScope SuperScope
        {
            get
            {
                return _SuperScope;
            }
            set
            {
                _SuperScope = value;
                CommandStack = new EXEExecutionStack();
                CommandStack.Enqueue(_SuperScope);
            }
        }
        public EXEExecutionStack CommandStack { get; private set; }

        public OALProgram()
        {
            Reset();
        }

        public void Reset()
        {
            this.ExecutionSpace = new CDClassPool();
            this.RelationshipSpace = new CDRelationshipPool();
            this.SuperScope = new EXEScope();
        }
    }
}
