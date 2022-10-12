using System;
using System.Collections.Generic;
using System.Linq;

namespace OALProgramControl
{
    public class EXEScope : EXECommand
    {
        private List<EXEPrimitiveVariable> PrimitiveVariables;
        private List<EXEReferencingVariable> ReferencingVariables;
        private List<EXEReferencingSetVariable> SetReferencingVariables;

        public List<EXECommand> Commands { get; protected set; }

        public String OALCode;

        public EXEScope()
        {
            this.PrimitiveVariables = new List<EXEPrimitiveVariable>();
            this.ReferencingVariables = new List<EXEReferencingVariable>();
            this.SetReferencingVariables = new List<EXEReferencingSetVariable>();
            this.SuperScope = null;
            this.Commands = new List<EXECommand>();
        }

        public EXEScope(EXEScope SuperScope, EXECommand[] Commands)
        {
            this.PrimitiveVariables = new List<EXEPrimitiveVariable>();
            this.ReferencingVariables = new List<EXEReferencingVariable>();
            this.SetReferencingVariables = new List<EXEReferencingSetVariable>();

            this.SetSuperScope(SuperScope);

            this.Commands = new List<EXECommand>();
            foreach (EXECommand Command in Commands)
            {
                this.AddCommand(Command);
            }
        }

        protected override Boolean Execute(OALProgram OALProgram)
        {
            AddCommandsToStack(OALProgram, this.Commands);
            return true;
        }

        protected void AddCommandsToStack(OALProgram OALProgram, List<EXECommand> Commands)
        {
            Commands.ForEach(command => command.SetSuperScope(this));
            OALProgram.CommandStack.Enqueue(Commands);
        }

        public Dictionary<String, String> GetStateDictRecursive()
        {
            Dictionary<String, String> Result = new Dictionary<String, String>();
            EXEScope CurrentScope = this;
            while (CurrentScope != null)
            {
                foreach (EXEPrimitiveVariable CurrentVariable in CurrentScope.PrimitiveVariables)
                {
                    Result[CurrentVariable.Name] = CurrentVariable.Value;
                }

                CurrentScope = CurrentScope.SuperScope;
            }

            return Result;
        }

        public Dictionary<String, String> GetRefStateDictRecursive()
        {
            Dictionary<String, String> Result = new Dictionary<String, String>();
            EXEScope CurrentScope = this;
            while (CurrentScope != null)
            {
                foreach (EXEReferencingVariable CurrentVariable in CurrentScope.ReferencingVariables)
                {
                    Result[CurrentVariable.Name] = CurrentVariable.ClassName;
                }

                foreach (EXEReferencingSetVariable CurrentVariable in CurrentScope.SetReferencingVariables)
                {
                    Result[CurrentVariable.Name + "[" + CurrentVariable.ValidVariableCount() + "]"] =
                        CurrentVariable.ClassName;
                }

                CurrentScope = CurrentScope.SuperScope;
            }

            return Result;
        }

        public Dictionary<String, String> GetRefStateAttrsDictRecursive(CDClassPool ExecutionSpace, String VarName)
        {
            Dictionary<String, String> Result = new Dictionary<String, String>();
            EXEReferencingVariable Var = this.FindReferencingVariableByName(VarName);
            if (Var == null)
            {
                return Result;
            }

            CDClassInstance Inst = Var.RetrieveReferencedClassInstance(ExecutionSpace);
            if (Inst == null)
            {
                return Result;
            }

            foreach (var Attribute in Inst.GetStateWithoutID())
            {
                Result[VarName + "." + Attribute.Key] = Attribute.Value;
            }

            return Result;
        }

        public Dictionary<String, String> GetRefStateAttrsDictRecursive(CDClassPool ExecutionSpace)
        {
            Dictionary<String, String> Result = new Dictionary<String, String>();
            EXEScope CurrentScope = this;
            while (CurrentScope != null)
            {
                foreach (EXEReferencingVariable Var in CurrentScope.ReferencingVariables)
                {
                    CDClassInstance Inst = Var.RetrieveReferencedClassInstance(ExecutionSpace);
                    if (Inst == null)
                    {
                        continue;
                    }

                    foreach (var Attribute in Inst.GetStateWithoutID())
                    {
                        Result[Var.Name + "." + Attribute.Key] = Attribute.Value;
                    }
                }

                CurrentScope = CurrentScope.SuperScope;
            }

            return Result;
        }

        public Dictionary<String, String> GetSetRefStateAttrsDictRecursive(CDClassPool ExecutionSpace, String VarName)
        {
            Dictionary<String, String> Result = new Dictionary<String, String>();

            EXEReferencingSetVariable SetVar = this.FindSetReferencingVariableByName(VarName);
            if (SetVar == null)
            {
                return Result;
            }

            int i = 0;
            Console.WriteLine(VarName + " has cardinality " + SetVar.GetReferencingVariables().Count());
            foreach (EXEReferencingVariable Var in SetVar.GetReferencingVariables())
            {
                CDClassInstance Inst = Var.RetrieveReferencedClassInstance(ExecutionSpace);
                if (Inst == null)
                {
                    i++;
                    continue;
                }

                foreach (var Attribute in Inst.GetStateWithoutID())
                {
                    Result[VarName + "[" + i + "]." + Attribute.Key] = Attribute.Value;
                }

                i++;
            }

            return Result;
        }

        public Dictionary<String, String> GetAllHandleStateAttrsDictRecursive(CDClassPool ExecutionSpace)
        {
            Dictionary<String, String> Result = new Dictionary<String, String>();
            Dictionary<String, String> Temp;

            foreach (EXEReferencingVariable Var in this.ReferencingVariables)
            {
                Temp = this.GetRefStateAttrsDictRecursive(ExecutionSpace, Var.Name);
                Temp.ToList().ForEach(x => Result.Add(x.Key, x.Value));
            }

            foreach (EXEReferencingSetVariable Var in this.SetReferencingVariables)
            {
                Temp = this.GetSetRefStateAttrsDictRecursive(ExecutionSpace, Var.Name);
                Temp.ToList().ForEach(x => Result.Add(x.Key, x.Value));
            }

            if (this.SuperScope != null)
            {
                Temp = this.SuperScope.GetAllHandleStateAttrsDictRecursive(ExecutionSpace);
                Temp.ToList().ForEach(x => Result.Add(x.Key, x.Value));
            }

            return Result;
        }

        public bool AddVariable(EXEPrimitiveVariable Variable)
        {
            bool Result = false;

            if (!VariableNameExists(Variable.Name))
            {
                this.PrimitiveVariables.Add(Variable);
                Result = true;
            }

            return Result;
        }

        public bool AddVariable(EXEReferencingVariable Variable)
        {
            bool Result = false;

            if (!VariableNameExists(Variable.Name))
            {
                this.ReferencingVariables.Add(Variable);
                Result = true;
            }

            return Result;
        }

        public bool AddVariable(EXEReferencingSetVariable Variable)
        {
            bool Result = false;

            if (!VariableNameExists(Variable.Name))
            {
                this.SetReferencingVariables.Add(Variable);
                Result = true;
            }

            return Result;
        }

        public bool VariableNameExists(String VariableName)
        {
            bool Result = false;
            if (FindPrimitiveVariableByName(VariableName) != null)
            {
                Result = true;
            }
            else if (FindReferencingVariableByName(VariableName) != null)
            {
                Result = true;
            }
            else if (FindSetReferencingVariableByName(VariableName) != null)
            {
                Result = true;
            }

            return Result;
        }

        public string FindVariable(String VariableName)
        {
            Object Result = FindPrimitiveVariableByName(VariableName);
            if (Result != null)
            {
                return ((EXEPrimitiveVariable) Result).Name;
            }

            Result = FindReferencingVariableByName(VariableName);
            if (Result != null)
            {
                return ((EXEReferencingVariable) Result).Name;
            }
            
            Result = FindSetReferencingVariableByName(VariableName);
            if (Result != null)
            {
                return ((EXEReferencingSetVariable) Result).Name;
            }

            return null;
        }

        // SetUloh1 - this method is done. Do the same with two similar methods below it
        public EXEPrimitiveVariable FindPrimitiveVariableByName(String Name)
        {
            EXEPrimitiveVariable Result = null;
            EXEScope CurrentScope = this;
            while (CurrentScope != null)
            {
                foreach (EXEPrimitiveVariable CurrentVariable in CurrentScope.PrimitiveVariables)
                {
                    if (CurrentVariable.Name == Name)
                    {
                        Result = CurrentVariable;
                        break;
                    }
                }

                if (Result != null)
                {
                    break;
                }

                CurrentScope = CurrentScope.SuperScope;
            }

            return Result;
        }

        public EXEReferenceHandle FindReferenceHandleByName(String Name)
        {
            EXEReferenceHandle Result = FindReferencingVariableByName(Name);
            if (Result == null)
            {
                Result = FindSetReferencingVariableByName(Name);
            }

            return Result;
        }

        public EXEReferencingVariable FindReferencingVariableByName(String Name)
        {
            EXEReferencingVariable Result = null;
            EXEScope CurrentScope = this;

            while (CurrentScope != null)
            {
                foreach (EXEReferencingVariable CurrentVariable in CurrentScope.ReferencingVariables)
                {
                    if (String.Equals(CurrentVariable.Name, Name))
                    {
                        Result = CurrentVariable;
                        break;
                    }
                }

                if (Result != null)
                {
                    break;
                }

                CurrentScope = CurrentScope.SuperScope;
            }

            return Result;
        }

        public EXEReferencingSetVariable FindSetReferencingVariableByName(String Name)
        {
            EXEScope CurrentScope = this;

            while (CurrentScope != null)
            {
                foreach (EXEReferencingSetVariable ReferencingSetVariable in CurrentScope.SetReferencingVariables)
                {
                    if (String.Equals(ReferencingSetVariable.Name, Name))
                    {
                        return ReferencingSetVariable;
                    }
                }

                CurrentScope = CurrentScope.SuperScope;
            }

            return null;
        }

        public void AddCommand(EXECommand Command)
        {
            this.Commands.Add(Command);
            if (Command.IsComposite())
            {
                ((EXEScope) Command).SetSuperScope(this);
            }
        }

        public override Boolean IsComposite()
        {
            return true;
        }

        public bool UnsetReferencingVariables(String ClassName, long InstanceID)
        {
            bool Result = true;
            foreach (EXEReferencingVariable Variable in this.ReferencingVariables)
            {
                if (Variable.ClassName == ClassName && Variable.ReferencedInstanceId == InstanceID)
                {
                    Variable.ReferencedInstanceId = -1;
                }
            }

            foreach (EXEReferencingSetVariable SetVariable in this.SetReferencingVariables)
            {
                SetVariable.UnsetVariables(InstanceID);
            }

            if (this.SuperScope != null)
            {
                Result &= this.SuperScope.UnsetReferencingVariables(ClassName, InstanceID);
            }

            return Result;
        }

        public int VariablePrimitiveCount()
        {
            return this.PrimitiveVariables.Count;
        }

        public int ValidVariableReferencingCountRecursive()
        {
            int Result = 0;
            foreach (EXEReferencingVariable Var in this.ReferencingVariables)
            {
                if (Var.IsInitialized())
                {
                    ++Result;
                }
            }

            if (this.SuperScope != null)
            {
                Result += this.SuperScope.ValidVariableReferencingCountRecursive();
            }

            return Result;
        }

        public int VariableReferencingCount()
        {
            return this.ReferencingVariables.Count;
        }

        public int NonEmptyVariableSetReferencingCountRecursive()
        {
            int Result = 0;
            foreach (EXEReferencingSetVariable Var in this.SetReferencingVariables)
            {
                if (Var.IsNotEmpty())
                {
                    ++Result;
                }
            }

            if (this.SuperScope != null)
            {
                Result += this.SuperScope.ValidVariableReferencingCountRecursive();
            }

            return Result;
        }

        public int VariableSetReferencingCount()
        {
            return this.SetReferencingVariables.Count;
        }

        public Boolean DestroyReferencingVariable(String VariableName)
        {
            Boolean Result = false;

            int i = 0;
            foreach (EXEReferencingVariable CurrentVariable in this.ReferencingVariables)
            {
                if (String.Equals(CurrentVariable.Name, VariableName))
                {
                    this.ReferencingVariables.RemoveAt(i);
                    Result = true;
                    break;
                }

                i++;
            }

            return Result;
        }

        public List<(String, String)> GetReferencingVariablesByIDRecursive(long ID)
        {
            List<(String, String)> Vars = new List<(String, String)>();
            foreach (EXEReferencingVariable Var in this.ReferencingVariables)
            {
                if (Var.ReferencedInstanceId == ID)
                {
                    Vars.Add((Var.ClassName, Var.Name));
                }
            }

            if (this.SuperScope != null)
            {
                Vars = Vars.Concat(this.SuperScope.GetReferencingVariablesByIDRecursive(ID)).ToList();
            }

            return Vars;
        }

        public void ClearVariables()
        {
            this.PrimitiveVariables.Clear();
            this.ReferencingVariables.Clear();
            this.SetReferencingVariables.Clear();
        }

        public void ClearVariablesRecursive()
        {
            this.ClearVariables();

            foreach (EXECommand Command in this.Commands)
            {
                if (Command is EXEScope)
                {
                    ((EXEScope) Command).ClearVariablesRecursive();
                }
            }
        }

        public String DetermineVariableType(List<String> AccesChain, CDClassPool ClassPool)
        {
            if (AccesChain == null || !AccesChain.Any())
            {
                return null;
            }

            EXEPrimitiveVariable PrimitiveVariable = FindPrimitiveVariableByName(AccesChain[0]);
            if (PrimitiveVariable != null)
            {
                return AccesChain.Count > 1 ? null : PrimitiveVariable.Type;
            }

            EXEReferencingSetVariable SetVariable = FindSetReferencingVariableByName(AccesChain[0]);
            if (SetVariable != null)
            {
                return AccesChain.Count > 1 ? null : SetVariable.Type;
            }

            EXEReferencingVariable ReferencingVariable = FindReferencingVariableByName(AccesChain[0]);
            if (ReferencingVariable == null)
            {
                return null;
            }

            if (AccesChain.Count == 1)
            {
                return ReferencingVariable.ClassName;
            }

            CDClass Class = ClassPool.getClassByName(ReferencingVariable.ClassName);
            if (Class == null)
            {
                return null;
            }

            CDAttribute Attribute = Class.GetAttributeByName(AccesChain[1]);
            if (Attribute == null)
            {
                return null;
            }

            return Attribute.Type;
        }

        public override String ToCode(String Indent = "")
        {
            String Result = "";
            foreach (EXECommand Command in this.Commands)
            {
                Result += Command.ToCode(Indent);
            }

            return Result;
        }

        public override EXECommand CreateClone()
        {
            EXEScope Clone = CreateDuplicateScope();
            Clone.OALCode = this.OALCode;
            Clone.Commands = this.Commands.Select(x => x.CreateClone()).ToList();

            return Clone;
        }

        protected virtual EXEScope CreateDuplicateScope()
        {
            return new EXEScope();
        }
    }
}