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
            Boolean Result = false;
            switch (Type)
            {
                case "integer":
                    Result = int.TryParse(Value, out _);
                    break;
                case "real":
                    try
                    {
                        double.Parse(Value, CultureInfo.InvariantCulture);
                        Result = true;
                    }
                    catch (Exception e)
                    {
                        Result = false;
                    }
                    break;
                case "boolean":
                    Result = Boolean.TryParse(Value, out _);
                    break;
                case "unique_ID":
                    //Result = Name == UniqueIDTypeName;
                    Result = int.TryParse(Value, out _);
                    break;
                case "date":
                    //TODO (ak vobec)
                    break;
                default:
                    Result = false;
                    break;
            }

            return Result;
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

            return null;
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

            if (String.Equals(AttributeType, NewValueType))
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
            if (VariableType == null || NewValueType == null)
            {
                return false;
            }

            if (String.Equals(VariableType, NewValueType))
            {
                return true;
            }

            if (
                EXEExecutionGlobals.AllowLossyAssignmentOfRealToInteger
                && EXETypes.IntegerTypeName.Equals(VariableType)
                && EXETypes.RealTypeName.Equals(NewValueType)
            )
            {
                return true;
            }

            if (
               EXEExecutionGlobals.AllowPromotionOfIntegerToReal
               && EXETypes.RealTypeName.Equals(VariableType)
               && EXETypes.IntegerTypeName.Equals(NewValueType)
            )
            {
                return true;
            }

            return false;
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
    }
}
