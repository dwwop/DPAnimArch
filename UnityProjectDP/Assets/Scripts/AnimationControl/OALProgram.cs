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
                CurrentCommandPointer = new EXEProgramPointer(_SuperScope);
            }
        }
        private EXEProgramPointer CurrentCommandPointer { get; set; }

        public OALProgram()
        {
            this.ExecutionSpace = new CDClassPool();
            this.RelationshipSpace = new CDRelationshipPool();
            this.SuperScope = new EXEScope();
            this.CurrentCommandPointer = new EXEProgramPointer(SuperScope);
        }

        public bool NextStep()
        {
            bool Result = false;

            EXECommand NextCommand = CurrentCommandPointer.NextCommand();
            if (NextCommand != null)
            {
                Result = true;
            }

            return Result;
        }
        public List<EXECommand> CurrentCommands()
        {
            List<EXECommand> Result = new EXECommand[] { CurrentCommandPointer.CurrentCommand }.ToList();

            return Result;
        }
        /*public bool Execute()
        {
            bool Result = this.SuperScope.Execute(this, null);
            this.SuperScope.ClearVariablesRecursive();

            return Result;
        }*/
    }
}
