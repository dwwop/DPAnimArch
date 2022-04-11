using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public class EXETypes
    {
        public static String IntegerTypeName = "integer";
        public const String RealTypeName = "real";
        public const String BooleanTypeName = "boolean";
        public const String StringTypeName = "string";
        public const String DateTypeName = "date";
        public const String UniqueIDTypeName = "unique_ID";
        public const String ReferenceTypeName = "reference";

        public const String BooleanTrue = "TRUE";
        public const String BooleanFalse = "FALSE";

        public static String UnitializedName = "UNDEFINED";
        public const String UniqueIDAttributeName = "ID";

        private static readonly List<String> IntNames = new List<string>(new String[] { "int", "integer", "long", "long int", "long integer"});
        private static readonly List<String> RealNames = new List<string>(new String[] { "real", "float", "double", "decimal", "floating", "floating point" });
        private static readonly List<String> BoolNames = new List<string>(new String[] { "bool", "boolean"});
        private static readonly List<String> StringNames = new List<string>(new String[] { "string", "char[]", "char", "List<char>" });
        private static readonly List<String> PrimitiveNames = new List<string>(new String[] { IntegerTypeName, RealTypeName, BooleanTypeName, StringTypeName, DateTypeName, UniqueIDTypeName });
        private static readonly Dictionary<Char, String> EscapeChars = new Dictionary<Char, String>() { { '\"', "\"" }, { '\'', "\'" }, { 't', "\t" }, { 'n', "\n" }, { '\\', "\\" } };

        public static bool IsPrimitive(String typeName)
        {
            return PrimitiveNames.Contains(typeName.ToLower());
        }
        public static String DetermineVariableType(String name, String value)
        {
            if (name == UniqueIDAttributeName)
            {
                return UniqueIDTypeName;
            }

            if (value == EXETypes.UnitializedName)
            {
                return UnitializedName;
            }

            if (value == "")
            {
                return null;
            }

            if (long.TryParse(value, out _))
            {
                return IntegerTypeName;
            }

            try
            {
                decimal.Parse(value, CultureInfo.InvariantCulture);
                return RealTypeName;
            }
            catch (Exception e)
            {
            }

            if (value == EXETypes.BooleanTrue || value == EXETypes.BooleanFalse)
            {
                return BooleanTypeName;
            }

            if (value[0] == '"' && value[value.Length-1] == '"')
            {
                return StringTypeName;
            }

            return ReferenceTypeName;
        }
        public static Boolean IsValidValue(String Value, String Type)
        {
            if (UnitializedName.Equals(Value))
            {
                return true;
            }

            Boolean Result = false;
            switch (Type)
            {
                case "integer":
                    Result = int.TryParse(Value, out _);
                    break;
                case "real":
                    try
                    {
                        decimal.Parse(Value, CultureInfo.InvariantCulture);
                        Result = true;
                    }
                    catch (Exception e)
                    {
                        Result = false;
                    }
                    break;
                case "boolean":
                    Result = (Value == EXETypes.BooleanTrue || Value == EXETypes.BooleanFalse);
                    break;
                case "unique_ID":
                    //Result = Name == UniqueIDTypeName;
                    Result = int.TryParse(Value, out _);
                    break;
                case "date":
                    //TODO (ak vobec)
                    break;
                case "string":
                    Result = (Value[0] == '"' && Value[Value.Length - 1] == '"');
                    break;
                default:
                    Result = false;
                    break;
            }

            return Result;
        }
        public static Boolean IsValidReferenceValue(String Value, String Type)
        {
            if
            (
                !Value.Split(',').Select(id => long.TryParse(id, out _)).Aggregate(true, (acc, x) => acc && x)
                &&
                !String.Empty.Equals(Value)
            )
            {
                return false;
            }

            long[] IDs = String.Empty.Equals(Value) ? new long[] { } : Value.Split(',').Select(id => long.Parse(id)).ToArray();

            if (IDs.Length != 1 && !"[]".Equals(Type.Substring(Type.Length - 2, 2)))
            {
                return false;
            }

            return true;
        }
        public static String ConvertEATypeName(String EAType)
        {
            String Input = EAType.ToLower();
            if (IntNames.Contains(Input))
            {
                return IntegerTypeName;
            }
            if (RealNames.Contains(Input))
            {
                return RealTypeName;
            }
            if (BoolNames.Contains(Input))
            {
                return BooleanTypeName;
            }
            if (StringNames.Contains(Input))
            {
                return StringTypeName;
            }

            return EAType;
        }
        public static bool CanBeAssignedToAttribute(String AttributeName, String AttributeType, String NewValueType)
        {
            if (AttributeName == null || AttributeType == null || NewValueType == null)
            {
                return false;
            }

            if (EXETypes.UniqueIDAttributeName.Equals(AttributeName))
            {
                return false;
            }

            if (EXETypes.UnitializedName.Equals(NewValueType) && EXETypes.IsPrimitive(AttributeType))
            {
                return true;
            }

            if (String.Equals(AttributeType, NewValueType))
            {
                return true;
            }

            CDClass NewValueTypeClass = OALProgram.Instance.ExecutionSpace.getClassByName(NewValueType);
            CDClass AttributeTypeClass = OALProgram.Instance.ExecutionSpace.getClassByName(AttributeType);

            if (NewValueTypeClass != null && AttributeTypeClass != null && NewValueTypeClass.CanBeAssignedTo(AttributeTypeClass))
            {
                return true;
            }

            if (
                EXEExecutionGlobals.AllowLossyAssignmentOfRealToInteger
                && EXETypes.IntegerTypeName.Equals(AttributeType)
                && EXETypes.RealTypeName.Equals(NewValueType)
            )
            {
                return true;
            }

            if (
               EXEExecutionGlobals.AllowPromotionOfIntegerToReal
               && EXETypes.RealTypeName.Equals(AttributeType)
               && EXETypes.IntegerTypeName.Equals(NewValueType)
            )
            {
                return true;
            }

            return false;
        }
        public static bool CanBeAssignedToVariable(String VariableType, String NewValueType)
        {
            return CanBeAssignedToAttribute("", VariableType, NewValueType);
        }
        public static String AdjustAssignedValue(String VariableType, String NewValue)
        {
            String NewValueType = DetermineVariableType("", NewValue);

            if (NewValue == null || VariableType == null || NewValueType == null)
            {
                return "";
            }

            // We're assigning real to integer
            if (
                EXEExecutionGlobals.AllowLossyAssignmentOfRealToInteger
                && EXETypes.IntegerTypeName.Equals(VariableType)
                && EXETypes.RealTypeName.Equals(NewValueType)
            )
            {
                Console.WriteLine("REAL 2 INT");
                return NewValue.Split('.')[0];
            }

            // We're assigning integer to real
            if (
               EXEExecutionGlobals.AllowPromotionOfIntegerToReal
               && EXETypes.RealTypeName.Equals(VariableType)
               && EXETypes.IntegerTypeName.Equals(NewValueType)
            )
            {
                Console.WriteLine("INT 2 REAL");
                return NewValue + ".0";
            }

            return NewValue;
        }

        public static String EvaluateEscapeSequences(String Value)
        {
            int index = Value.IndexOf('\\', 0);
            String EscChar;

            while (index != -1)
            {
                if ((index + 1) >= Value.Length)
                {
                    break;
                }

                if (EXETypes.EscapeChars.TryGetValue(Value[index + 1], out EscChar))
                {
                    Value = Value.Remove(index, 2)
                                 .Insert(index, EscChar);
                }

                index = Value.IndexOf('\\', index + 1);
            }

            return Value;
        }
    }
}
