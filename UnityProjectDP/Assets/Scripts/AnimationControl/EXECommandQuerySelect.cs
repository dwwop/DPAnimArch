using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public class EXECommandQuerySelect : EXECommand
    {
        public const String CardinalityAny = "any";
        public const String CardinalityMany = "many";

        public String Cardinality { get; set; }
        public String ClassName { get; set; }
        public String VariableName { get; set; }
        public EXEASTNode WhereCondition { get; set; }

        public EXECommandQuerySelect(String Cardinality, String ClassName, String VariableName)
        {
            this.Cardinality = Cardinality;
            this.ClassName = ClassName;
            this.VariableName = VariableName;
            this.WhereCondition = null;
        }
        public EXECommandQuerySelect(String Cardinality, String ClassName, String VariableName, EXEASTNode WhereCondition)
        {
            this.Cardinality = Cardinality;
            this.ClassName = ClassName;
            this.VariableName = VariableName;
            this.WhereCondition = WhereCondition;
        }

        // SetUloh2
        public override bool Execute(OALProgram OALProgram, EXEScope Scope)
        {
            //Select instances of given class that match the criteria and assign them to variable with given name
            // ClassName tells us which class we are interested in
            // Cardinality tells us whether we want one random instance (matching the criteria) or all of them
            // "Many" - we create variable EXEReferencingSetVariable, "Any" - we create variable EXEReferencingVariable
            // Variable name tells us how to name the newly created referencing variable
            // Where condition tells us which instances to select from all instances of the class (just do EXEASTNode.Evaluate and return true if the result "true" and false for "false")
            // When making unit tests, do not use the "where" causule yet, because its evaluation is not yet implemented

            CDClass Class = OALProgram.ExecutionSpace.getClassByName(this.ClassName);
            if (Class == null)
            {
                return false;
            }

            // We need to check, if the variable already exists, it must be of corresponding type
            if (Scope.VariableNameExists(this.VariableName))
            {
                if
                (
                    !(
                        (
                            EXECommandQuerySelect.CardinalityAny.Equals(this.Cardinality)
                            &&
                            this.ClassName == Scope.FindReferencingVariableByName(this.VariableName).ClassName
                        )
                        ||
                        (
                            EXECommandQuerySelect.CardinalityMany.Equals(this.Cardinality)
                            &&
                            this.ClassName == Scope.FindSetReferencingVariableByName(this.VariableName).ClassName
                        )
                    )
                )
                {
                    return false;
                }
            }

            // Evaluate relationship selection. If it fails, execution fails too
            List<long> SelectedIds = Class.GetAllInstanceIDs();
            if (SelectedIds == null)
            {
                return false;
            }

            // If class has no instances, command may execute successfully, but we better verify references in the WHERE condition
            if (SelectedIds.Count() == 0 && this.WhereCondition != null)
            {
                return this.WhereCondition.VerifyReferences(Scope, OALProgram.ExecutionSpace);
            }

            //Console.WriteLine("Select has " + SelectedIds.Count + " potential results");

            // Now let's evaluate the condition
            if (this.WhereCondition != null && SelectedIds.Any())
            {
                String TempSelectedVarName = "selected";

                //Console.WriteLine("creating selected var");
                EXEReferencingVariable SelectedVar = new EXEReferencingVariable(TempSelectedVarName, this.ClassName, -1);
                if (!Scope.AddVariable(SelectedVar))
                {
                    return false;
                }
               // Console.WriteLine("created selected var");
                List<long> ResultIds = new List<long>();
                foreach (long Id in SelectedIds)
                {
                    //Console.WriteLine("id check iteration start");
                    SelectedVar.ReferencedInstanceId = Id;
                    String ConditionResult = this.WhereCondition.Evaluate(Scope, OALProgram.ExecutionSpace);

                   /* Console.WriteLine("cond evaluated");
                    Console.WriteLine(Id + " : " + ConditionResult == null ? "null" : ConditionResult);*/

                    if (!EXETypes.IsValidValue(ConditionResult, EXETypes.BooleanTypeName))
                    {
                        Scope.DestroyReferencingVariable(TempSelectedVarName);
                        return false;
                    }

                    if (EXETypes.BooleanTrue.Equals(ConditionResult))
                    {
                        ResultIds.Add(Id);
                    }
                }
                //Console.WriteLine("Select has " + SelectedIds.Count + " results");
                //foreach (long id in ResultIds) Console.WriteLine("RES: " + id);
                SelectedIds = ResultIds;
                Scope.DestroyReferencingVariable(TempSelectedVarName);
            }

            // Now we have ids of selected instances. Let's assign them to a variable
            if (EXECommandQuerySelect.CardinalityMany.Equals(this.Cardinality))
            {
                EXEReferencingSetVariable Variable = Scope.FindSetReferencingVariableByName(this.VariableName);
                if (Variable == null)
                {
                    Variable = new EXEReferencingSetVariable(this.VariableName, this.ClassName);
                    if (!Scope.AddVariable(Variable))
                    {
                        return false;
                    }
                }
                foreach (long Id in SelectedIds)
                {
                    Variable.AddReferencingVariable(new EXEReferencingVariable("", Variable.ClassName, Id));
                }
            }
            else if (EXECommandQuerySelect.CardinalityAny.Equals(this.Cardinality))
            {
                EXEReferencingVariable Variable = Scope.FindReferencingVariableByName(this.VariableName);
                long ResultId = SelectedIds.Any() ? SelectedIds[new Random().Next(SelectedIds.Count)] : -1;
                if (Variable == null)
                {
                    //Console.WriteLine("Final 'any' id is " + ResultId);
                    Variable = new EXEReferencingVariable(this.VariableName, this.ClassName, ResultId);
                    if (!Scope.AddVariable(Variable))
                    {
                        return false;
                    }
                }
                else
                {
                    Variable.ReferencedInstanceId = ResultId;
                }
            }
            else
            {
                return false;
            }

            return true;
        }
        public override string ToCodeSimple()
        {
            return "select " + this.Cardinality + " " + this.VariableName + " from instances of " + this.ClassName +
                (this.WhereCondition == null ? "" : (" where ") + this.WhereCondition.ToCode());
        }
    }
}
