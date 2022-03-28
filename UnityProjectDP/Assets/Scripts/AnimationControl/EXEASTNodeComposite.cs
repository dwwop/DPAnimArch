using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public class EXEASTNodeComposite : EXEASTNode
    {
        public String Operation { get; set; }
        public List<EXEASTNode> Operands { get; }

        public static List<List<String>> LeveledOperators = new List<List<String>>(new List<String>[] {
            new List<String> (new String[] { "and", "or"}),
            new List<String> (new String[] { "not"}),
            new List<String> (new String[] { "==", "!="}),
            new List<String> (new String[] { "empty", "not_empty"}),
            new List<String> (new String[] { "<=", ">=", "<", ">" }),
            new List<String> (new String[] { "+", "-"}),
            new List<String> (new String[] { "*", "/", "%"}),
            new List<String> (new String[] { "cardinality"}),
            new List<String> (new String[] { "."})
        });

        public EXEASTNodeComposite(String Operation)
        {
            this.Operation = Operation;
            this.Operands = new List<EXEASTNode>();
        }
        public EXEASTNodeComposite(String Operation, EXEASTNode[] Operands)
        {
            this.Operation = Operation;
            this.Operands = new List<EXEASTNode>(Operands);
        }

        public void AddOperand(EXEASTNode Operand)
        {
            if (Operand == null)
            {
                return;
            }

            this.Operands.Add(Operand);
        }

        public String GetNodeValue()
        {
            return this.Operation;
        }
        public String Evaluate(EXEScope Scope, CDClassPool ExecutionSpace)
        {
            String Result = null;
            EXEExpressionEvaluator Evaluator = new EXEExpressionEvaluator();
            EXEEvaluatorHandleOperators HandleEvaluator = new EXEEvaluatorHandleOperators();
            EXEReferenceEvaluator AccessEvaluator = new EXEReferenceEvaluator();

            // If we just calculate with ints, reals, bools, strings
            if (Evaluator.IsSimpleOperator(this.Operation))
            {
                List<String> EvaluatedOperands = new List<String>();
                foreach (EXEASTNode Operand in this.Operands)
                {
                    EvaluatedOperands.Add(Operand.Evaluate(Scope, ExecutionSpace));
                }
                if (EvaluatedOperands.Contains(null) || EvaluatedOperands.Contains(EXETypes.UnitializedName))
                {
                    return Result;
                }
                //If we are returning real number, let's format it so that we don't have trouble with precision
                Result = Evaluator.Evaluate(this.Operation, EvaluatedOperands);
                /*Console.Write("AST Composite operation " + this.Operation + " has result ");
                Console.Write(Result == null ? "null" : Result);
                Console.WriteLine();*/
                if (Result == null)
                {
                    return Result;
                }
               /* Console.WriteLine("Operation: " + this.Operation);
                Console.WriteLine("Result of operation" + (Result == null ? "null" : Result));*/
                if (EXETypes.RealTypeName.Equals(EXETypes.DetermineVariableType("", Result)))
                {
                    //Console.WriteLine("is real and needs formatting");
                    if (!Result.Contains("."))
                    {
                        Result = FormatDouble(Result);
                    }
                    if (!Result.Contains("."))
                    {
                        Result += ".0";
                    }
                }
            }
            // If we have handle operators
            else if (HandleEvaluator.IsHandleOperator(this.Operation))
            {
                if (this.Operands.Count == 1)
                {     
                    if (this.Operands[0].IsReference() || EXETypes.UnitializedName.Equals(this.Operands[0].GetNodeValue()))
                    {
                        String OperandType = EXETypes.UnitializedName.Equals(this.Operands[0].GetNodeValue())
                                             ?
                                             EXETypes.UnitializedName
                                             :
                                             Scope.DetermineVariableType(this.Operands[0].AccessChain(), ExecutionSpace);

                        if
                        (
                            !EXETypes.IsPrimitive(OperandType)
                            &&
                            !EXETypes.ReferenceTypeName.Equals(OperandType)
                        )
                        {
                            String OperandValue = null;

                            if (EXETypes.UnitializedName.Equals(OperandType))
                            {
                                OperandValue = "";
                            }
                            else if ("[]".Equals(OperandType.Substring(OperandType.Length - 2, 2)))
                            {
                                CDClass Class = ExecutionSpace.getClassByName(OperandType.Substring(0, OperandType.Length - 2));
                                if (Class == null)
                                {
                                    return Result;
                                }

                                OperandValue = this.Operands[0].Evaluate(Scope, ExecutionSpace);

                                if (!EXETypes.IsValidReferenceValue(OperandValue, OperandType))
                                {
                                    return Result;
                                }

                                long[] IDs = String.Empty.Equals(OperandValue) ? new long[] { } : OperandValue.Split(',').Select(id => long.Parse(id)).ToArray();

                                CDClassInstance ClassInstance;
                                foreach (long ID in IDs)
                                {
                                    ClassInstance = Class.GetInstanceByID(ID);
                                    if (ClassInstance == null)
                                    {
                                        return Result;
                                    }
                                }
                            }
                            else
                            {
                                CDClass Class = ExecutionSpace.getClassByName(OperandType);
                                if (Class == null)
                                {
                                    return Result;
                                }

                                OperandValue = this.Operands[0].Evaluate(Scope, ExecutionSpace);

                                if (!EXETypes.IsValidReferenceValue(OperandValue, OperandType))
                                {
                                    return Result;
                                }

                                long ID = long.Parse(OperandValue);

                                CDClassInstance ClassInstance = Class.GetInstanceByID(ID);
                                if (ClassInstance == null)
                                {
                                    return Result;
                                }
                            }

                            Result = HandleEvaluator.Evaluate(this.Operation, OperandValue);
                        }
                    }
                }
            }
            // If we have access operator - we either access attribute or have decimal number. There are always 2 operands
            else if (".".Equals(this.Operation) && this.Operands.Count == 2)
            {
                if (EXETypes.IntegerTypeName.Equals(EXETypes.DetermineVariableType("", this.Operands[0].GetNodeValue()))
                    && EXETypes.IntegerTypeName.Equals(EXETypes.DetermineVariableType("", this.Operands[1].GetNodeValue()))
                )
                {
                    Result = this.Operands[0].GetNodeValue() + "." + this.Operands[1].GetNodeValue();
                }
                else if (EXETypes.ReferenceTypeName.Equals(EXETypes.DetermineVariableType("", this.Operands[0].GetNodeValue()))
                    && EXETypes.ReferenceTypeName.Equals(EXETypes.DetermineVariableType("", this.Operands[1].GetNodeValue()))
                )
                {
                    Result = AccessEvaluator.EvaluateAttributeValue(this.Operands[0].GetNodeValue(), this.Operands[1].GetNodeValue(), Scope, ExecutionSpace);
                }
            }
            return Result;
        }

        public bool VerifyReferences(EXEScope Scope, CDClassPool ExecutionSpace)
        {
            bool Result = false;
            EXEExpressionEvaluator Evaluator = new EXEExpressionEvaluator();
            EXEEvaluatorHandleOperators HandleEvaluator = new EXEEvaluatorHandleOperators();
            EXEReferenceEvaluator AccessEvaluator = new EXEReferenceEvaluator();

            // If we just calculate with ints, reals, bools, strings
            if (Evaluator.IsSimpleOperator(this.Operation))
            {
                foreach (EXEASTNode Operand in this.Operands)
                {
                    if (!Operand.VerifyReferences(Scope, ExecutionSpace))
                    {
                        return false;
                    }
                }
                Result = true;
            }
            // If we have handle operators
            else if (HandleEvaluator.IsHandleOperator(this.Operation))
            {
                if (this.Operands.Count() == 1)
                {
                    String OperandType = Scope.DetermineVariableType(this.Operands[0].AccessChain(), ExecutionSpace);

                    if (OperandType != null)
                    {
                        if (!EXETypes.IsPrimitive(OperandType) && !EXETypes.ReferenceTypeName.Equals(OperandType))
                        {
                            Result = true;
                        }
                    }
                }
            }
            // If we have access operator - we either access attribute or have decimal number. There are always 2 operands
            else if (".".Equals(this.Operation) && this.Operands.Count == 2)
            {
                if (EXETypes.IntegerTypeName.Equals(EXETypes.DetermineVariableType("", this.Operands[0].GetNodeValue()))
                    && EXETypes.IntegerTypeName.Equals(EXETypes.DetermineVariableType("", this.Operands[1].GetNodeValue()))
                )
                {
                    Result = true;
                }
                else if (EXETypes.ReferenceTypeName.Equals(EXETypes.DetermineVariableType("", this.Operands[0].GetNodeValue()))
                    && EXETypes.ReferenceTypeName.Equals(EXETypes.DetermineVariableType("", this.Operands[1].GetNodeValue()))
                )
                {
                    EXEReferencingVariable Variable = Scope.FindReferencingVariableByName(this.Operands[0].GetNodeValue());
                    if (Variable == null)
                    {
                        return false;
                    }
                    CDClass Class = ExecutionSpace.getClassByName(Variable.ClassName);
                    if (Class == null)
                    {
                        return false;
                    }
                    CDAttribute Attribute = Class.GetAttributeByName(this.Operands[1].GetNodeValue());
                    if (Attribute == null)
                    {
                        return false;
                    }
                    CDClassInstance Instance = Variable.RetrieveReferencedClassInstance(ExecutionSpace);
                    if (Instance == null)
                    {
                        return false;
                    }
                    Result = true;
                }
            }
            return Result;
        }
        public void PrintPretty(string indent, bool last)
        {
            Console.Write(indent);
            if (last)
            {
                Console.Write("\\+");
                indent += "  ";
            }
            else
            {
                Console.Write("|+");
                indent += "| ";
            }
            Console.WriteLine(this.Operation);

            for (int i = 0; i < this.Operands.Count; i++)
                this.Operands[i].PrintPretty(indent, i == this.Operands.Count - 1);
        }

        private String FormatDouble(String Double)
        {
            String Temp = decimal.Parse(Double).ToString("G29");
            String Result = "";
            for (int i = 0; i < Temp.Length; i++)
            {
                Result += Temp[i] == ',' ? '.' : Temp[i];
            }

            return Result;
        }

        public string ToCode()
        {
            string Result = null;
            if (this.Operands.Count == 1)
            {
                Result = this.Operation + " " + this.Operands[0].ToCode();
            }
            else if (".".Equals(this.Operation))
            {
                Result = this.Operands[0].GetNodeValue() + "." + this.Operands[1].GetNodeValue();
            }
            else if (this.Operands.Count > 1)
            {
                Result = "";
                foreach (EXEASTNode Operand in this.Operands)
                {
                    if (!"".Equals(Result))
                    {
                        Result += " " + this.Operation;
                    }
                    Boolean UseBrackets = false;
                    if (Operand is EXEASTNodeComposite)
                    {
                        String SubOperation = ((EXEASTNodeComposite) Operand).Operation;
                        int ThisOperatorLevel = GetOperatorLevel(this.Operation);
                        int SubOperatorLevel = GetOperatorLevel(SubOperation);
                        if (ThisOperatorLevel != -1 && SubOperatorLevel != -1)
                        {
                            if (ThisOperatorLevel >= SubOperatorLevel && !this.Operation.Equals(SubOperation))
                            {
                                    UseBrackets = true;
                            }
                            if (UseBrackets && ThisOperatorLevel == SubOperatorLevel && (ThisOperatorLevel == 5 || ThisOperatorLevel == 6))
                            {
                                UseBrackets = false;
                            }
                        }
                    }
                    if (UseBrackets)
                    {
                        Result += ("".Equals(Result) ? "" : " ") + "(" + Operand.ToCode() + ")";
                    }
                    else
                    {
                        Result += ("".Equals(Result) ? "" : " ") + Operand.ToCode();
                    }
                }
            }
            return Result;
        }

        public static int GetOperatorLevel(String Operator)
        {
            for (int i = 0; i < LeveledOperators.Count; i++)
            {
                if (LeveledOperators[i].Contains(Operator))
                {
                    return i;
                }
            }
            return -1;
        }

        public List<string> AccessChain()
        {
            if (!".".Equals(this.Operation))
            {
                return null;
            }

            List<List<String>> SubLists = this.Operands.Select(x => x.AccessChain()).ToList();

            if (SubLists.Contains(null))
            {
                return null;
            }

            return SubLists.Aggregate(new List<String>(), (acc,x) => acc.Concat(x).ToList());
        }

        public bool IsReference()
        {
            return ".".Equals(this.Operation) && this.Operands.Select(x => x.IsReference()).Aggregate(true, (acc, x) => acc && x);
        }
    }
}
