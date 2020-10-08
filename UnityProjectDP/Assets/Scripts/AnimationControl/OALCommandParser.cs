using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public class OALCommandParser
    {
        public static List<String> ArithmeticOperatorList = new List<String>(new String[] { "+", "-", "*", "/", "%" });
        // Do not forget that each operator ("<=") needs to be sooner than its substrings ("<")
        public static List<String> LogicalOperatorList = new List<String>(new String[] { "==", "!=", "<=", ">=", "and", "or", "not", "<", ">" });
        public static String AccessOperator = ".";
        public static String AssignmentOperator = "=";
        public static List<String> AllOperators = new List<String>(new String[] { "-", "+", "*", "/", "%", "==", "!=", "<", ">", "<=", ">=", "and", "or", "not", ".", "=" });
        public static List<List<String>> LeveledOperators = new List<List<String>>(new List<String>[] {
            new List<String> (new String[] { "and", "or"}),
            new List<String> (new String[] { "not"}),
            new List<String> (new String[] { "==", "!="}),
            new List<String> (new String[] { "<=", ">=", "<", ">" }),
            new List<String> (new String[] { "+", "-"}),
            new List<String> (new String[] { "*", "/", "%"}),
            new List<String> (new String[] { AccessOperator})
        });
        public static List<String> UnaryOperators = new List<String>(new String[] { "not" });
        public static int LongestOperatorLength = 3;

        public static String VariableNameChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789_#";

        public static List<String> ControlKeywords = new List<String>(new String[] { "break", "continue", "return" });

        public EXEASTNode ConstructAST(String ExpressionCommand)
        {
            throw new NotImplementedException();

            //Console.WriteLine("ConstrAST:" + ExpressionCommand + "EOL");

            EXEASTNode AST = null;
           // EXEQueryChecker QueryChecker = new EXEQueryChecker();

            String ClearedExpressionCommand = ParseUtil.SqueezeWhiteSpace(ExpressionCommand);
            // Bollock - think about (5 + 6) * (5 + 7)
            /*while ("(".Equals(ClearedExpressionCommand[0]) && ")".Equals(ClearedExpressionCommand[ExpressionCommand.Length - 1]))
            {
                ClearedExpressionCommand = ClearedExpressionCommand.Substring(1, ClearedExpressionCommand.Length - 2);
            }*/

           /* if ("\"".Equals(ClearedExpressionCommand[0]) && "\"".Equals(ClearedExpressionCommand[ExpressionCommand.Length - 1]))
            {
               // AST = new EXEASTNodeLeaf(ExpressionCommand, false, false, false);
            }
            //First go for query
            else if (QueryChecker.IsQuery(ClearedExpressionCommand))
            {
               // Console.WriteLine("OALCP B4Constructing Query AST:" + ExpressionCommand);
                AST = QueryChecker.ConstructQueryAST(ClearedExpressionCommand);
               // Console.WriteLine("OALCP AfterConstructing Query AST:" + ExpressionCommand);
            }
            //Then go for top level commands -> control (return, break, continue)
            else if (IsControlCommand(ClearedExpressionCommand))
            {
                AST = ConstructControlCommandAST(ClearedExpressionCommand);
            }
            //Then go for top level commands -> assign
            else if (IsAssignment(ClearedExpressionCommand))
            {
               // Console.WriteLine("OALCP B4Constructing Assignment AST:" + ExpressionCommand);
                AST = ConstructAssignmentCommandAST(ClearedExpressionCommand);
               // Console.WriteLine("OALCP AfterConstructing Assignment AST:" + ExpressionCommand);
            }
            //Then if it has operator, it is an expression and needs to be treated as such
            else if (ContainsOperator(ClearedExpressionCommand))
            {
                AST = ConstructExprASTAlt(ClearedExpressionCommand);
            }
            //If we got here, we have leaf node -> variable/attribute/method name or literal value
            else
            {
                while (ClearedExpressionCommand[0] == '(' && ClearedExpressionCommand[ClearedExpressionCommand.Length - 1] == ')')
                {
                    ClearedExpressionCommand = ClearedExpressionCommand.Substring(1, ClearedExpressionCommand.Length - 2);
                    ClearedExpressionCommand = EXEParseUtil.SqueezeWhiteSpace(ClearedExpressionCommand);
                }

                AST = new EXEASTNodeLeaf(EXEParseUtil.SqueezeWhiteSpace(ClearedExpressionCommand));
            }*/

            //Console.WriteLine("OALCP:" + ExpressionCommand);
            return AST;
        }

        private EXEASTNode ConstructControlCommandAST(String Command)
        {
            throw new NotImplementedException();

            EXEASTNode AST = null;
            if (!IsControlCommand(Command))
            {
                return AST;
            }

            String SanitizedCommand = ParseUtil.SqueezeWhiteSpace(Command);
            String[] CommandTokens = SanitizedCommand.Split(' ');

            if (CommandTokens[0] == "continue" || CommandTokens[0] == "break")
            {
                //AST = new EXEASTNodeLeaf(CommandTokens[0], false, false, false);
            }
            else if (CommandTokens[0] == "return")
            {
                EXEASTNodeComposite TempAST = new EXEASTNodeComposite(CommandTokens[0]);
                TempAST.AddOperand(ConstructAST(CommandTokens[1]));
            }

            return AST;
        }

        private EXEASTNodeComposite ConstructAssignmentCommandAST(String Command)
        {
            EXEASTNodeComposite AST = null;
            if (!IsAssignment(Command))
            {
                return AST;
            }

            String SanitizedCommand = ParseUtil.SqueezeWhiteSpace(Command);
           // Console.WriteLine("ConstrAssign:" + SanitizedCommand + "EOL");

            int SplitIndex = SanitizedCommand.IndexOf('=');
            String Storage = SanitizedCommand.Substring(0, SplitIndex);
            String Value = SanitizedCommand.Substring(SplitIndex + 1);

          //  Console.WriteLine("Assigning " + Value + " to " + Storage);
            /*String[] CommandTokens = SanitizedCommand.Split('=');


            AST = new EXEASTNodeComposite("=");
            AST.AddOperand(ConstructAST(CommandTokens[0]));
            AST.AddOperand(ConstructAST(String.Join("", CommandTokens.Skip(2).ToArray())));*/

            AST = new EXEASTNodeComposite("=");
            AST.AddOperand(ConstructAST(Storage));
            AST.AddOperand(ConstructAST(Value));
           // Console.WriteLine("Assigned");
            return AST;
        }

        public static Boolean IsValidName(String Name)
        {
            Boolean Result = true;
            if (Name == null)
            {
                return false;
            }
            foreach (char c in Name)
            {
                if (!VariableNameChars.Contains(c))
                {
                    Result = false;
                }
            }
            if (Name.Length == 0)
            {
                Result = false;
            }
            return Result;
        }

        public static Boolean IsValidRelationshipName(String Name)
        {
            if (Name == null || Name.Length == 0)
            {
                return false;
            }

            if (!'R'.Equals(Name[0]))
            {
                return false;
            }

            if ('0'.Equals(Name[1]))
            {
                return false;
            }

            if (!int.TryParse(Name.Substring(1), out _))
            {
                return false;
            }

            return true;
        }
        public Boolean CanBeOperator(String Operator)
        {
            Boolean Result = IsOperator(Operator);
            if (Result)
            {
                return Result;
            }

            foreach (String ExistingOperator in AllOperators)
            {
                if (ExistingOperator.Length >= Operator.Length)
                {
                    if (ExistingOperator.Substring(0, Operator.Length).Equals(Operator))
                    {
                        Result = true;
                        break;
                    }
                }
            }

            return Result;
        }
        public Boolean IsOperator(String String)
        {
            Boolean Result = false;
            foreach (String Operator in AllOperators)
            {
                if (String.Contains(Operator))
                {
                    Result = true;
                    break;
                }
            }
            return Result;
        }

        public Boolean IsControlCommand (String Command)
        {
            String SanitizedCommand = ParseUtil.SqueezeWhiteSpace(Command);
            String[] CommandTokens = SanitizedCommand.Split(' ');
            Boolean result = false;
            if (ControlKeywords.Contains(CommandTokens[0]))
            {
                result = true;
            }
            return result;
        }
        private Boolean IsAssignment(String Command)
        {
            Boolean Result = false;
            //This means we have assignment
            if (!Command.Contains("==") && Command.Contains("="))
            {
                Result = true;
            }
            return Result;
        }
        // Beware of strings
        private Boolean ContainsOperator(String ExpressionCommand)
        {
            Boolean Result = false;
            Boolean InString = false;

            foreach (char c in ExpressionCommand)
            {
                if (c == '"')
                {
                    InString = !InString;
                    continue;
                }

                if (InString)
                {
                    continue;
                }

                if (IsOperator(ExpressionCommand))
                {
                    Result = true;
                    break;
                }
            }

            return Result;
        }

        private Boolean ContainsBrackets(String ExpressionCommand)
        {
            Boolean Result = false;
            Boolean InString = false;

            foreach (char c in ExpressionCommand)
            {
                if (c == '"')
                {
                    InString = !InString;
                    continue;
                }

                if (InString)
                {
                    continue;
                }

                if (c == '(' || c == ')')
                {
                    Result = true;
                    break;
                }
            }

            return Result;
        }

        private Boolean ContainsString(String Command)
        {
            Boolean Result = false;
            if (Command.Contains("\""))
            {
                Result = true;
            }
            return Result;
        }

        // https://stackoverflow.com/questions/2641326/finding-all-positions-of-substring-in-a-larger-string-in-c-sharp
        private List<int> AllIndexesOf(string str, string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return null;
            }
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }

        private Boolean IsUnaryOperator(String Operator)
        {
            String SanitizedCommand = ParseUtil.SqueezeWhiteSpace(Operator);
            Boolean Result = UnaryOperators.Contains(SanitizedCommand);
            return Result;
        }

        public EXEASTNode ConstructExprASTAlt(String Expression)
        {
            //Console.WriteLine("ConsASTAlt:" + Expression + "EOL");

            String ModifiedExpression = ParseUtil.SqueezeWhiteSpace(Expression);
            (String, int) TopLevelOperator = IdentifyFirstTopLevelOperator(ModifiedExpression);
            Boolean ExprContainsOperator = ContainsOperator(ModifiedExpression);
            while (TopLevelOperator.Item2 == -1 && ExprContainsOperator)
            {
                ModifiedExpression = ModifiedExpression.Substring(1, ModifiedExpression.Length - 2);

                if (ModifiedExpression.Length == 0)
                {
                    break;
                }

                TopLevelOperator = IdentifyFirstTopLevelOperator(ModifiedExpression);
                ExprContainsOperator = ContainsOperator(ModifiedExpression);
            }

            //Console.WriteLine("ConsASTAlt,unbracketed:" + ModifiedExpression + "EOL");

            EXEASTNodeComposite AST = new EXEASTNodeComposite(TopLevelOperator.Item1);
            if (TopLevelOperator.Item2 != 0)
            {
                AST.AddOperand(ConstructAST(ModifiedExpression.Substring(0, TopLevelOperator.Item2)));
            }
            AST.AddOperand(ConstructAST(ModifiedExpression.Substring(
                TopLevelOperator.Item2 + TopLevelOperator.Item1.Length
            )));

            return AST;
        }
        public (String, int) IdentifyFirstTopLevelOperator(String Expression)
        {
            (String, int) Result = ("", -1);

            if (Expression == null)
            {
                return Result;
            }
            String SanitizedExpression = ParseUtil.SqueezeWhiteSpace(Expression);
            if (Expression == null || Expression.Length == 0 || !ContainsOperator(Expression))
            {
                return Result;
            }

            Boolean InString = false;
            int CurrentDepthLevel = 0;
            String AccumulatedPossibleOperator = "";

            int MaximumOperatorLevel = -1;
            int MaximumOperatorIndex = -1;
            int i = -1;
            String MaximumOperator = "";
            foreach (char c in SanitizedExpression)
            {
                i++;

                if (c == '"')
                {
                    InString = !InString;
                    continue;
                }

                if (InString)
                {
                    continue;
                }

                if (c == '(')
                {
                    CurrentDepthLevel++;
                    continue;
                }

                if (c == ')')
                {
                    CurrentDepthLevel--;
                    continue;
                }

                if (CurrentDepthLevel > 0)
                {
                    continue;
                }

                /*if (char.IsWhiteSpace(c))
                {
                    continue;
                }*/

                AccumulatedPossibleOperator += c;

                int CurrentOperatorLevel = OperatorLevel(AccumulatedPossibleOperator);

                if (CurrentOperatorLevel > MaximumOperatorLevel)
                {
                    MaximumOperatorIndex = i - AccumulatedPossibleOperator.Length + 1;
                    MaximumOperatorLevel = CurrentOperatorLevel;
                    MaximumOperator = AccumulatedPossibleOperator;
                }

                if (AccumulatedPossibleOperator.Length >= LongestOperatorLength || CurrentOperatorLevel == -1)
                {
                    AccumulatedPossibleOperator = "";
                }

                if(MaximumOperatorLevel == LeveledOperators.Count - 1)
                {
                    break;
                }
            }

            if (MaximumOperatorLevel != -1)
            {
                Result = (MaximumOperator, MaximumOperatorIndex);
            }

            return Result;
        }
        public int OperatorLevel(String InputOperator)
        {
            String SanitizedOperator = ParseUtil.SqueezeWhiteSpace(InputOperator);

            int OperatorLevel = -1;
            for (int i = 0; i < LeveledOperators.Count; i++)
            {
                foreach (String Operator in LeveledOperators[i])
                {
                    if (SanitizedOperator == Operator)
                    {
                        OperatorLevel = LeveledOperators.Count  -  i;
                        break;
                    }
                }
            }
            return OperatorLevel;
        }
        public int MaxPossibleOperatorLevel(String InputOperator)
        {
            int OperatorLevel = this.OperatorLevel(InputOperator);
            if (OperatorLevel != - 1)
            {
                return OperatorLevel;
            }

            String SanitizedOperator = ParseUtil.SqueezeWhiteSpace(InputOperator);
            for (int i = LeveledOperators.Count - 1; i >= 0; i--)
            {
                foreach (String Operator in LeveledOperators[i])
                {
                    if (SanitizedOperator.Length > Operator.Length)
                    {
                        continue;
                    }

                    if (SanitizedOperator == Operator.Substring(0, SanitizedOperator.Length))
                    {
                        OperatorLevel = i;
                        break;
                    }
                }
            }
            return OperatorLevel;
        }
    }
}
