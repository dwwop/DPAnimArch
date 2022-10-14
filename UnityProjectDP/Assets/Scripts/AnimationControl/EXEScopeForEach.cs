using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public class EXEScopeForEach : EXEScope
    {
        public String IteratorName { get; set; }
        public String IterableName { get; set; }
        public String IterableAttributeName { get; set; }
        private int IterableIndex;

        public EXEScopeForEach(String Iterator, String Iterable, String IterableAttribute) : base()
        {
            this.IteratorName = Iterator;
            this.IterableName = Iterable;
            this.IterableAttributeName = IterableAttribute;
            int IterableIndex = 0;
        }

        public EXEScopeForEach(EXEScope SuperScope, EXECommand[] Commands, String Iterator, String Iterable,
            String IterableAttribute) : base(SuperScope, Commands)
        {
            this.IteratorName = Iterator;
            this.IterableName = Iterable;
            this.IterableAttributeName = IterableAttribute;
            int IterableIndex = 0;
        }

        protected override Boolean Execute(OALProgram OALProgram)
        {
            EXEReferencingVariable IteratorVariable = this.FindReferencingVariableByName(this.IteratorName);
            EXEReferencingSetVariable IterableVariable = this.FindSetReferencingVariableByName(this.IterableName);

            String IterableVariableClassName = "";
            int ReferencingVariablesCount = 0;
            List<long> ReferencingVariablesIDs = new List<long>(); // This is important if we have IterableAttributeName

            if (this.IterableAttributeName == null)
            {
                // We cannot iterate over not existing reference set
                if (IterableVariable == null)
                {
                    UnityEngine.Debug.Log("e1"); //
                    return false;
                }

                IterableVariableClassName = IterableVariable.ClassName;
                ReferencingVariablesCount = IterableVariable.GetReferencingVariables().Count;
            }
            else
            {
                // If we have IterableAttributeName, IterableName must be reference and not reference set 
                EXEReferencingVariable Variable = SuperScope.FindReferencingVariableByName(this.IterableName);
                if (Variable == null)
                {
                    UnityEngine.Debug.Log("e2"); //
                    return false;
                }

                CDClass VariableClass = OALProgram.ExecutionSpace.getClassByName(Variable.ClassName);
                if (VariableClass == null)
                {
                    UnityEngine.Debug.Log("e3"); //
                    return false;
                }

                CDAttribute Attribute = VariableClass.GetAttributeByName(this.IterableAttributeName);
                if (Attribute == null)
                {
                    UnityEngine.Debug.Log("e4"); //
                    return false;
                }

                // We cannot iterate over reference that is not a set
                if (!"[]".Equals(Attribute.Type.Substring(Attribute.Type.Length - 2, 2)))
                {
                    UnityEngine.Debug.Log("e5"); //
                    return false;
                }

                IterableVariableClassName = Attribute.Type.Substring(0, Attribute.Type.Length - 2);

                // Get instance representing IterableName
                CDClassInstance ClassInstance = VariableClass.GetInstanceByID(Variable.ReferencedInstanceId);
                if (ClassInstance == null)
                {
                    UnityEngine.Debug.Log("e6"); //
                    return false;
                }

                String Values = ClassInstance.GetAttributeValue(this.IterableAttributeName);
                if (!EXETypes.IsValidReferenceValue(Values, Attribute.Type))
                {
                    UnityEngine.Debug.Log("e7"); //
                    return false;
                }

                if (!String.Empty.Equals(Values))
                {
                    ReferencingVariablesIDs = Values.Split(',').Select(id => long.Parse(id)).ToList();
                }

                ReferencingVariablesCount = ReferencingVariablesIDs.Count;
            }

            // If iterator already exists and its class does not match the iterable class, we cannot do this
            if (IteratorVariable != null && !IteratorVariable.ClassName.Equals(IterableVariableClassName))
            {
                UnityEngine.Debug.Log("e8"); //
                return false;
            }

            // If iterator name is already taken for another variable, we quit again. Otherwise we create the iterator variable
            if (IteratorVariable == null)
            {
                IteratorVariable = new EXEReferencingVariable(this.IteratorName, IterableVariableClassName, -1);
                if (!this.GetSuperScope().AddVariable(IteratorVariable))
                {
                    UnityEngine.Debug.Log("e9"); //
                    return false;
                }
            }

            if (IterableIndex < ReferencingVariablesCount)
            {
                if (this.IterableAttributeName == null)
                {
                    EXEReferencingVariable CurrentItem = IterableVariable.GetReferencingVariables()[IterableIndex];
                    IteratorVariable.ReferencedInstanceId = CurrentItem.ReferencedInstanceId;
                }
                else
                {
                    IteratorVariable.ReferencedInstanceId = ReferencingVariablesIDs[IterableIndex];
                }

                IterableIndex++;
                OALProgram.CommandStack.Enqueue(this);
                AddCommandsToStack(OALProgram, this.Commands);
                this.ClearVariables();
            }

            return true;
        }

        public override String ToCode(String Indent = "")
        {
            String Result = Indent + "for each " + this.IteratorName + " in " + (this.IterableAttributeName == null
                ? this.IterableName
                : (this.IterableName + "." + this.IterableAttributeName)) + "\n";
            foreach (EXECommand Command in this.Commands)
            {
                Result += Command.ToCode(Indent + "\t");
            }

            Result += Indent + "end for;\n";
            return Result;
        }

        protected override EXEScope CreateDuplicateScope()
        {
            return new EXEScopeForEach(IteratorName, IterableName, IterableAttributeName);
        }
    }
}