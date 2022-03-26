using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OALProgramControl
{
    public class EXEExpressionEvaluator
    {

        private static List<String> SimpleOperators = new List<String>(new String[] { "-", "+", "*", "/", "%", "==", "!=", "<", ">", "<=", ">=", "and", "or", "not"});

        public bool IsSimpleOperator(String Operator)
        {
            return SimpleOperators.Contains(Operator);
        }
        // SetUloh1
        // Here you get operator and operands, and you need to return the result. Check the unit tests to see what this is about
        public String Evaluate(String Operator, List<String> InOperands)
        {
            List<String> Operands = PromoteIntegers(InOperands);

            //TODO Lukas: Better check all operands
            if (!this.CanBeEvaluated(Operator, Operands)) return null;

            //Console.WriteLine("Can be evaluated");

            //get variable type
            String VariableType = EXETypes.DetermineVariableType(null, Operands[0]);


            switch (Operator)
            {
                case "+": //int, real, string
                    //true if Operands are integers
                    if (String.Equals(EXETypes.IntegerTypeName, VariableType))
                    {
                        //convert to list of int
                        List<int> IntList = Operands.Select(int.Parse).ToList();
                        //add numbers in list and return result
                        return IntList.Aggregate((a, x) => a + x).ToString();
                    }
                    //true if Operands are real numbers
                    if (String.Equals(EXETypes.RealTypeName, VariableType))
                    {
                        //convert to list of real numbers
                        // List<double> DoubleList = Operands.Select(double.Parse.).ToList();
                        List<decimal> DoubleList = new List<decimal>();
                        foreach (String Operand in Operands)
                        {
                            try
                            {
                                DoubleList.Add(decimal.Parse(Operand, CultureInfo.InvariantCulture));
                            }
                            catch (Exception e)
                            {
                                return EXETypes.BooleanFalse;
                            }
                        }
                        //add numbers in list and return result 
                        return EXETypes.AdjustAssignedValue(EXETypes.RealTypeName, DoubleList.Aggregate((a, x) => a + x).ToString());
                    }
                    if (String.Equals(EXETypes.StringTypeName, VariableType))
                    {
                        //add string in list and return result 
                        //remove \" from Operands  
                        List<String> StringList = Operands.Select(s => s.Substring(1, s.Length - 2)).ToList();

                        //add strings in list and return result
                        return CreateEXETypeString(StringList.Aggregate((a, x) => a + x));
                    }
                    break;
                case "-": //int, real
                    //true if Operands are integers
                    if (String.Equals(EXETypes.IntegerTypeName, VariableType))
                    {
                        //convert to list of int
                        List<int> IntList = Operands.Select(int.Parse).ToList();
                        int first = IntList[0];
                        IntList.RemoveAt(0);
                        if (!IntList.Any())
                        {
                            return (-first).ToString();
                        }
                        //sub numbers in list and return result 
                        return IntList.Aggregate(first, (a, x) => a - x).ToString();
                    }
                    //true if Operands are real numbers
                    if (String.Equals(EXETypes.RealTypeName, VariableType))
                    {
                        //convert to list of real numbers
                        // List<double> DoubleList = Operands.Select(double.Parse.).ToList();
                        List<decimal> DoubleList = new List<decimal>();
                        foreach (String Operand in Operands)
                        {
                            try
                            {
                                DoubleList.Add(decimal.Parse(Operand, CultureInfo.InvariantCulture));
                            }
                            catch (Exception e)
                            {
                                return EXETypes.BooleanFalse;
                            }
                        }
                        decimal first = DoubleList[0];
                        DoubleList.RemoveAt(0);
                        if (!DoubleList.Any())
                        {
                            return (-first).ToString();
                        }
                        //sub numbers in list and return result
                        return EXETypes.AdjustAssignedValue(EXETypes.RealTypeName, DoubleList.Aggregate(first, (a, x) => a - x).ToString());
                    }
                    break;
                case "*": //int, real
                    //true if Operands are integers
                    if (String.Equals(EXETypes.IntegerTypeName, VariableType))
                    {
                        //convert to list of int
                        List<int> IntList = Operands.Select(int.Parse).ToList();
                        //multiply numbers in list and return result
                        return IntList.Aggregate((a, x) => a * x).ToString();
                    }
                    //true if Operands are real numbers
                    if (String.Equals(EXETypes.RealTypeName, VariableType))
                    {
                        //convert to list of real numbers
                        // List<double> DoubleList = Operands.Select(double.Parse.).ToList();
                        List<decimal> DoubleList = new List<decimal>();
                        foreach (String Operand in Operands)
                        {
                            try
                            {
                                DoubleList.Add(decimal.Parse(Operand, CultureInfo.InvariantCulture));
                            }
                            catch (Exception e)
                            {
                                return EXETypes.BooleanFalse;
                            }
                        }
                        //multiply numbers in list and return result
                        return EXETypes.AdjustAssignedValue(EXETypes.RealTypeName, DoubleList.Aggregate((a, x) => a * x).ToString());
                    }

                    break;
                case "/": //int, real
                    //true if Operands are integers
                    if (String.Equals(EXETypes.IntegerTypeName, VariableType))
                    {
                        //convert to list of int
                        List<int> IntList = Operands.Select(int.Parse).ToList();
                        for (int i = 1; i < IntList.Count; i++)
                        {
                            if (IntList[i] == 0)
                            {
                                return null;
                            }
                        }
                        //divide numbers in list and return result
                        return IntList.Aggregate((a, x) => a / x).ToString();
                    }
                    //true if Operands are real numbers
                    if (String.Equals(EXETypes.RealTypeName, VariableType))
                    {
                        //convert to list of real numbers
                        // List<double> DoubleList = Operands.Select(double.Parse.).ToList();
                        List<decimal> DoubleList = new List<decimal>();
                        foreach (String Operand in Operands)
                        {
                            try
                            {
                                DoubleList.Add(decimal.Parse(Operand, CultureInfo.InvariantCulture));
                                for (int i = 1; i < DoubleList.Count; i++)
                                {
                                    if (DoubleList[i] == 0)
                                    {
                                        return EXETypes.BooleanFalse;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                return EXETypes.BooleanFalse;
                            }
                        }
                        //divide numbers in list and return result 
                        return EXETypes.AdjustAssignedValue(EXETypes.RealTypeName, DoubleList.Aggregate((a, x) => a / x).ToString());
                    }

                    break;
                case "%": //int, real
                    //true if Operands are integers
                    if (String.Equals(EXETypes.IntegerTypeName, VariableType))
                    {
                        //convert to list of int
                        List<int> IntList = Operands.Select(int.Parse).ToList();
                        for (int i = 1; i < IntList.Count; i++)
                        {
                            if (IntList[i] == 0)
                            {
                                return null;
                            }
                        }
                        //divide numbers in list and return result
                        return IntList.Aggregate((a, x) => a % x).ToString();
                    }
                    //true if Operands are real numbers
                    if (String.Equals(EXETypes.RealTypeName, VariableType))
                    {
                        //convert to list of real numbers
                        // List<double> DoubleList = Operands.Select(double.Parse.).ToList();
                        List<decimal> DoubleList = new List<decimal>();
                        foreach (String Operand in Operands)
                        {
                            try
                            {
                                DoubleList.Add(decimal.Parse(Operand, CultureInfo.InvariantCulture));
                                for (int i = 1; i < DoubleList.Count; i++)
                                {
                                    if (DoubleList[i] == 0)
                                    {
                                        return EXETypes.BooleanFalse;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                return EXETypes.BooleanFalse;
                            }
                        }
                        //divide numbers in list and return result 
                        return EXETypes.AdjustAssignedValue(EXETypes.RealTypeName, DoubleList.Aggregate((a, x) => a % x).ToString());
                    }

                    break;
                case "==": //int, real, string, booelan, unique ID 
                    //return bool
                    //true if Operands are integers
                    if (String.Equals(EXETypes.IntegerTypeName, VariableType))
                    {
                        //convert to list of int
                        List<int> IntList = Operands.Select(int.Parse).ToList();
                        //compare 2 numbers in list (equals) and return result 
                        return int.Equals(IntList[0], IntList[1]) ? EXETypes.BooleanTrue : EXETypes.BooleanFalse;

                    }
                    //true if Operands are real numbers
                    if (String.Equals(EXETypes.RealTypeName, VariableType))
                    {
                        //convert to list of real numbers
                        // List<double> DoubleList = Operands.Select(double.Parse.).ToList();
                        List<decimal> DoubleList = new List<decimal>();
                        foreach (String Operand in Operands)
                        {
                            try
                            {
                                DoubleList.Add(decimal.Parse(Operand, CultureInfo.InvariantCulture));
                            }
                            catch (Exception e)
                            {
                                return EXETypes.BooleanFalse;
                            }
                        }
                        //compare 2 numbers in list (equals) and return result 
                        return Double.Equals(DoubleList[0], DoubleList[1]) ? EXETypes.BooleanTrue : EXETypes.BooleanFalse;
                    }
                    if (String.Equals(EXETypes.StringTypeName, VariableType))
                    {
                        //compare 2 string in list (equals) and return result 
                        return String.Equals(Operands[0], Operands[1]) ? EXETypes.BooleanTrue : EXETypes.BooleanFalse;
                    }

                    //only 2 operands
                    if (String.Equals(EXETypes.BooleanTypeName, VariableType))
                    {
                        //convert to list of int
                        List<bool> BooleanList = Operands.Select(bool.Parse).ToList();
                        //compare 2 boolean in list (equals) and return result 
                        return Boolean.Equals(BooleanList[0], BooleanList[1]) ? EXETypes.BooleanTrue : EXETypes.BooleanFalse;
                    }
                    break;
                case "!=": //int, real, string, booelan, unique ID
                    //true if Operands are integers
                    if (String.Equals(EXETypes.IntegerTypeName, VariableType))
                    {
                        //convert to list of int
                        List<int> IntList = Operands.Select(int.Parse).ToList();
                        //compare 2 numbers in list (equals) and return result 
                        return !int.Equals(IntList[0], IntList[1]) ? EXETypes.BooleanTrue : EXETypes.BooleanFalse;
                    }
                    //true if Operands are real numbers
                    if (String.Equals(EXETypes.RealTypeName, VariableType))
                    {
                        //convert to list of real numbers
                        // List<double> DoubleList = Operands.Select(double.Parse.).ToList();
                        List<decimal> DoubleList = new List<decimal>();
                        foreach (String Operand in Operands)
                        {
                            try
                            {
                                DoubleList.Add(decimal.Parse(Operand, CultureInfo.InvariantCulture));
                            }
                            catch (Exception e)
                            {
                                return EXETypes.BooleanFalse;
                            }
                        }
                        //compare 2 numbers in list (equals) and return result
                        return !Double.Equals(DoubleList[0], DoubleList[1]) ? EXETypes.BooleanTrue : EXETypes.BooleanFalse;
                    }
                    if (String.Equals(EXETypes.StringTypeName, VariableType))
                    {
                        //compare 2 string in list (equals) and return result 
                        return !String.Equals(Operands[0], Operands[1]) ? EXETypes.BooleanTrue : EXETypes.BooleanFalse;
                    }
                    //true if Operands are boolean
                    if (String.Equals(EXETypes.BooleanTypeName, VariableType))
                    {
                        //convert to list of boolean
                        List<bool> BooleanList = Operands.Select(bool.Parse).ToList();
                        //compare 2 boolean in list (not equals) and return result 
                        return !Boolean.Equals(BooleanList[0], BooleanList[1]) ? EXETypes.BooleanTrue : EXETypes.BooleanFalse;
                    }
                    break;
                //only 2 operands
                case "<": //int, real, string
                    //true if Operands are integers
                    if (String.Equals(EXETypes.IntegerTypeName, VariableType))
                    {
                        //convert to list of int
                        List<int> IntList = Operands.Select(int.Parse).ToList();
                        //compare numbers in list (less then) and return result 
                        return IntList[0] < IntList[1] ? EXETypes.BooleanTrue : EXETypes.BooleanFalse;

                    }
                    //true if Operands are real numbers
                    if (String.Equals(EXETypes.RealTypeName, VariableType))
                    {
                        //convert to list of real numbers
                        // List<double> DoubleList = Operands.Select(double.Parse.).ToList();
                        List<decimal> DoubleList = new List<decimal>();
                        foreach (String Operand in Operands)
                        {
                            try
                            {
                                DoubleList.Add(decimal.Parse(Operand, CultureInfo.InvariantCulture));
                            }
                            catch (Exception e)
                            {
                                return EXETypes.BooleanFalse;
                            }
                        }
                        //compare numbers in list (less then) and return result
                        return DoubleList[0] < DoubleList[1] ? EXETypes.BooleanTrue : EXETypes.BooleanFalse;

                    }
                    if (String.Equals(EXETypes.StringTypeName, VariableType))
                    {
                        //compare 2 strings in list (less then) and return result
                        return String.Compare(Operands[0], Operands[1]) < 0 ? EXETypes.BooleanTrue : EXETypes.BooleanFalse;
                    }
                    break;
                case ">": //int, real, string
                    //true if Operands are integers
                    if (String.Equals(EXETypes.IntegerTypeName, VariableType))
                    {
                        //convert to list of int
                        List<int> IntList = Operands.Select(int.Parse).ToList();
                        //compare numbers in list (greater then) and return result 
                        return IntList[0] > IntList[1] ? EXETypes.BooleanTrue : EXETypes.BooleanFalse;

                    }
                    //true if Operands are real numbers
                    if (String.Equals(EXETypes.RealTypeName, VariableType))
                    {
                        //convert to list of real numbers
                        // List<double> DoubleList = Operands.Select(double.Parse.).ToList();
                        List<decimal> DoubleList = new List<decimal>();
                        foreach (String Operand in Operands)
                        {
                            try
                            {
                                DoubleList.Add(decimal.Parse(Operand, CultureInfo.InvariantCulture));
                            }
                            catch (Exception e)
                            {
                                return EXETypes.BooleanFalse;
                            }
                        }
                        //compare numbers in list (greater then) and return result 
                        return DoubleList[0] > DoubleList[1] ? EXETypes.BooleanTrue : EXETypes.BooleanFalse;
                    }
                    if (String.Equals(EXETypes.StringTypeName, VariableType))
                    {
                        //add string in list and return result 
                        return String.Compare(Operands[0], Operands[1]) > 0 ? EXETypes.BooleanTrue : EXETypes.BooleanFalse;
                    }
                    break;
                case "<=": //int, real, string
                    //true if Operands are integers
                    if (String.Equals(EXETypes.IntegerTypeName, VariableType))
                    {
                        //convert to list of int
                        List<int> IntList = Operands.Select(int.Parse).ToList();
                        //compare numbers in list (less then or equals to) and return result 
                        return IntList[0] <= IntList[1] ? EXETypes.BooleanTrue : EXETypes.BooleanFalse;
                    }
                    //true if Operands are real numbers
                    if (String.Equals(EXETypes.RealTypeName, VariableType))
                    {
                        //convert to list of real numbers
                        // List<double> DoubleList = Operands.Select(double.Parse.).ToList();
                        List<decimal> DoubleList = new List<decimal>();
                        foreach (String Operand in Operands)
                        {
                            try
                            {
                                DoubleList.Add(decimal.Parse(Operand, CultureInfo.InvariantCulture));
                            }
                            catch (Exception e)
                            {
                                return EXETypes.BooleanFalse;
                            }
                        }
                        //compare numbers in list (less then or equals to) and return result
                        return DoubleList[0] <= DoubleList[1] ? EXETypes.BooleanTrue : EXETypes.BooleanFalse;
                    }
                    if (String.Equals(EXETypes.StringTypeName, VariableType))
                    {
                        //compare strings in list (less then or equals to) and return result
                        return String.Compare(Operands[0], Operands[1]) <= 0 ? EXETypes.BooleanTrue : EXETypes.BooleanFalse;

                    }
                    break;
                case ">=": //int, real, string
                    //true if Operands are integers
                    if (String.Equals(EXETypes.IntegerTypeName, VariableType))
                    {
                        //convert to list of int
                        List<int> IntList = Operands.Select(int.Parse).ToList();
                        //compare numbers in list (greater then or equals to) and return result 
                        return IntList[0] >= IntList[1] ? EXETypes.BooleanTrue : EXETypes.BooleanFalse;
                    }
                    //true if Operands are real numbers
                    if (String.Equals(EXETypes.RealTypeName, VariableType))
                    {
                        //convert to list of real numbers
                        // List<double> DoubleList = Operands.Select(double.Parse.).ToList();
                        List<decimal> DoubleList = new List<decimal>();
                        foreach (String Operand in Operands)
                        {
                            try
                            {
                                DoubleList.Add(decimal.Parse(Operand, CultureInfo.InvariantCulture));
                            }
                            catch (Exception e)
                            {
                                return EXETypes.BooleanFalse;
                            }
                        }
                        //compare numbers in list (greater then or equals to) and return result 
                        return DoubleList[0] >= DoubleList[1] ? EXETypes.BooleanTrue : EXETypes.BooleanFalse;
                    }
                    if (String.Equals(EXETypes.StringTypeName, VariableType))
                    {
                        //compare numbers in list (greater then or equals to) and return result
                        return String.Compare(Operands[0], Operands[1]) >= 0 ? EXETypes.BooleanTrue : EXETypes.BooleanFalse;

                    }
                    break;
                case "and": //only boolean
                    //true if Operands are boolean
                    if (String.Equals(EXETypes.BooleanTypeName, VariableType))
                    {
                        //convert to list of int
                        List<bool> BooleanList = Operands.Select(bool.Parse).ToList();
                        foreach (bool bool_var in BooleanList)
                        {
                            if (!bool_var)
                            {
                                return EXETypes.BooleanFalse;
                            }
                        }
                        return EXETypes.BooleanTrue;
                    }
                    break;
                case "or": //only boolean
                    //true if Operands are boolean
                    if (String.Equals(EXETypes.BooleanTypeName, VariableType))
                    {
                        //convert to list of int
                        List<bool> BooleanList = Operands.Select(bool.Parse).ToList();
                        foreach (bool bool_var in BooleanList)
                        {
                            if (bool_var)
                            {
                                return EXETypes.BooleanTrue;
                            }
                        }
                        return EXETypes.BooleanFalse;
                    }
                    break;
                //only 1 operand
                case "not": //only boolean
                    //true if Operands are boolean
                    if (String.Equals(EXETypes.BooleanTypeName, VariableType))
                    {
                        //return TRUE or FALSE
                        return String.Equals(Operands[0], EXETypes.BooleanTrue) ? EXETypes.BooleanFalse : EXETypes.BooleanTrue;
                    }
                    break;
            }

            return null;
        }
        //What's the difference between "FAIL" and "false"?
        public Boolean CanBeEvaluated(String Operator, List<String> Params)
        {
            String Result = this.IsValid(Operator, Params.ToArray());
            return String.Equals(EXETypes.BooleanTrue, Result);
        }

        // Potentially refactor this
        public String IsValid(string oper, string[] param)
        {
            if (String.IsNullOrEmpty(oper) || param == null) return EXETypes.BooleanFalse;

            //get the first element type
            String ParamType = EXETypes.DetermineVariableType(null, param[0]);

            //check if it is int or real number
            if (String.Equals(ParamType, EXETypes.IntegerTypeName) || String.Equals(ParamType, EXETypes.RealTypeName))
            {
                if (oper == "+" || oper == "*" || oper == "/" || oper == "%")
                {
                    if (param.Length < 2) return EXETypes.BooleanFalse;
                }
                else if (oper == "-")
                {
                    if (param.Length < 1) return EXETypes.BooleanFalse;
                }
                else if (oper == "==" || oper == "!=" || oper == "<" || oper == ">" || oper == "<=" || oper == ">=")
                {
                    if (param.Length != 2) return EXETypes.BooleanFalse;
                }
                else if (oper == "and" || oper == "or" || oper == "not") return EXETypes.BooleanFalse;

                //chcek if all parameter is integer
                if (String.Equals(ParamType, EXETypes.IntegerTypeName))
                {

                    foreach (var value in param)
                    {
                        if (!EXETypes.IsValidValue(value, EXETypes.IntegerTypeName)) return EXETypes.BooleanFalse;
                    }

                    return EXETypes.BooleanTrue;
                }
                //chcek if all parameter is double
                else if (String.Equals(ParamType, EXETypes.RealTypeName))
                {
                    foreach (var value in param)
                    {
                        if (!EXETypes.IsValidValue(value, EXETypes.RealTypeName)) return EXETypes.BooleanFalse;
                    }

                    return EXETypes.BooleanTrue;
                }

                return EXETypes.BooleanFalse;

            }

            //check if it is boolean type
            if (String.Equals(ParamType, EXETypes.BooleanTypeName))
            {
                if (oper.ToLower() == "not")
                {
                    if (param.Length != 1) return EXETypes.BooleanFalse;
                    return (Boolean.TryParse(param[0], out _)) ? EXETypes.BooleanTrue : EXETypes.BooleanFalse;
                }

                if (((oper == "==" || oper == "!=") && param.Length == 2) || ((oper.ToLower() == "and" || oper.ToLower() == "or") && param.Length >= 2))
                {
                    foreach (var value in param)
                    {
                        if (!Boolean.TryParse(value, out _)) return EXETypes.BooleanFalse;
                    }
                    return EXETypes.BooleanTrue;
                }

                return EXETypes.BooleanFalse;

            }

            //check if it is string type
            if (String.Equals(ParamType, EXETypes.StringTypeName))
            {
                if (oper == ">" || oper == "<" || oper == "<=" || oper == ">=" || oper == "==" || oper == "!=")
                {
                    if (param.Length != 2) return EXETypes.BooleanFalse;

                    foreach (String value in param)
                    {
                        if (!IsValidEXETypeString(value)) return EXETypes.BooleanFalse;
                    }

                    return EXETypes.BooleanTrue;
                }

                if (oper == "+")
                {
                    if (param.Length < 2) return EXETypes.BooleanFalse;

                    foreach (String value in param)
                    {
                        if (!IsValidEXETypeString(value)) return EXETypes.BooleanFalse;
                    }

                    return EXETypes.BooleanTrue;

                }

                return EXETypes.BooleanFalse;

            }

            return EXETypes.BooleanFalse;
        }

        private List<String> PromoteIntegers(List<String> Operands)
        {
            if (!EXEExecutionGlobals.AllowPromotionOfIntegerToReal)
            {
                return Operands;
            }

            bool ContainsReal = false;
            bool ContainsInteger = false;
            foreach (String Operand in Operands)
            {
                ContainsReal |= EXETypes.RealTypeName.Equals(EXETypes.DetermineVariableType("", Operand));
                ContainsInteger |= EXETypes.IntegerTypeName.Equals(EXETypes.DetermineVariableType("", Operand));

                if (ContainsReal && ContainsInteger)
                {
                    break;
                }
            }

            if (!ContainsReal || !ContainsInteger)
            {
                return Operands;
            }

            return Operands.Select(x => EXETypes.AdjustAssignedValue(EXETypes.RealTypeName, x)).ToList();
        }

        //return "clear" string without \" 
        public String GetClearStringFromEXETypeString(String str)
        {
            return str.Substring(1, str.Length - 2);
        }

        //create EXEType string with \" from clear string
        public String CreateEXETypeString(String str)
        {
            return '\"' + str + '\"';
        }

        //check if string is valid EXETypeString
        public Boolean IsValidEXETypeString(String str)
        {
            return (str.Substring(0, 1) == "\"") && (str.Substring(str.Length - 1, 1) == "\"");
        }
    }
}