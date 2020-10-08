using System;
using System.Collections.Generic;
using System.Linq;

namespace OALProgramControl
{
    public class EXEReferencingSetVariable : EXEReferenceHandle
    {
        private List<EXEReferencingVariable> ReferencingVariables;
        public EXEReferencingSetVariable(String Name, String ClassName) : base(Name, ClassName)
        {
            this.ReferencingVariables = new List<EXEReferencingVariable>(); ;
        }

        public void UnsetVariables(long ID)
        {
            List<EXEReferencingVariable> ReferencingVariables = new List<EXEReferencingVariable>();

            foreach (EXEReferencingVariable Variable in this.ReferencingVariables)
            {
                if (Variable.ReferencedInstanceId != ID)
                {
                    ReferencingVariables.Add(Variable);
                }
            }

            this.ReferencingVariables = ReferencingVariables;
        }

        public void AddReferencingVariable(EXEReferencingVariable NewReferencingVariable)
        {
            this.ReferencingVariables.Add(NewReferencingVariable);
        }

        public List<EXEReferencingVariable> GetReferencingVariables()
        {
            return this.ReferencingVariables;
        }

        public override List<long> GetReferencedIds()
        {
            return this.ReferencingVariables.Select(x => { return x.ReferencedInstanceId; }).ToList().FindAll(x => x >= 0).ToList();
        }

        public bool IsNotEmpty()
        {
            foreach(EXEReferencingVariable Var in this.ReferencingVariables)
            {
                if (Var.IsInitialized())
                {
                    return true;
                }
            }
            return false;
        }
        public int ValidVariableCount()
        {
            int Result = 0;
            foreach (EXEReferencingVariable Var in this.ReferencingVariables)
            {
                if (Var.IsInitialized())
                {
                    Result++;
                }
            }
            return Result;
        }
    }
}
