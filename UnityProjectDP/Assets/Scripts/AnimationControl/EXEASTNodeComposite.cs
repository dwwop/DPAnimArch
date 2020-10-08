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
                if (EvaluatedOperands.Contains(null))
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
                Console.WriteLine("We have handle operator");
                Result = HandleEvaluator.Evaluate(this.Operation, this.Operands.Select(x => ((EXEASTNodeLeaf)x).GetNodeValue()).ToList(), Scope);
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
                if (this.Operands.Count() == 1 && Scope.FindReferenceHandleByName(((EXEASTNodeLeaf)this.Operands[0]).GetNodeValue()) != null)
                {
                    Result = true;
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
                        int ThisOperatorLevel = OALParser2.GetOperatorLevel(this.Operation);
                        int SubOperatorLevel = OALParser2.GetOperatorLevel(SubOperation);
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
    }
}
