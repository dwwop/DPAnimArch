using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OALProgramControl
{
    public class EXECommandWrite : EXECommand
    {
        private List<EXEASTNode> Arguments { get; }

        public EXECommandWrite(List<EXEASTNode> Arguments)
        {
            this.Arguments = Arguments;
        }

        protected override bool Execute(OALProgram OALProgram)
        {
            Debug.Log("Write");//
            Debug.Log(Arguments.Count);//
            for (int i = 0; i < this.Arguments.Count; i++)
            {
                Debug.Log(Arguments[i].ToCode());//
            }
            Debug.Log("End Write");//

            String Result = "";
            String ArgumentValue;
            String ArgumentType;

            for (int i = 0; i < this.Arguments.Count; i++)
            {
                ArgumentValue = this.Arguments[i].Evaluate(SuperScope, OALProgram.ExecutionSpace);

                if (this.Arguments[i].IsReference())
                {
                    ArgumentType = SuperScope.DetermineVariableType(this.Arguments[i].AccessChain(), OALProgram.ExecutionSpace);
                    if (ArgumentType == null)
                    {
                        return false;
                    }

                    //treba toto overit ci potrebujeme tu metodu FindPrimitives alebo to budeme riesit inak
                    // Check if AssignedType is ReferenceTypeName, it means it is primitive
                    if (EXETypes.ReferenceTypeName.Equals(ArgumentType))
                    {
                        //ArgumentType = FindPrimitiveType(AssignedValue); //V tomto pripade by malo byt AssignedValue asi meno inej premennej
                    }
                }
                // It must be primitive, not reference
                else
                {
                    ArgumentType = EXETypes.DetermineVariableType("", ArgumentValue);
                    if (ArgumentType == null)
                    {
                        return false;
                    }
                }

                //nemoze to byt instancia, a ani pole instancii
                /*if (EXETypes.UnitializedName.Equals(ArgumentType))
                {//neviem co treba tu

                }*/
                if (EXETypes.IsPrimitive(ArgumentType) || EXETypes.UnitializedName.Equals(ArgumentType))
                {
                    if (EXETypes.StringTypeName.Equals(ArgumentType))
                    {
                        // Remove double quotes
                        ArgumentValue = ArgumentValue.Substring(1, ArgumentValue.Length - 2);
                    }

                    Result += ArgumentValue;
                }
                else
                {
                    return false;
                }
            }

            ConsolePanel.Instance.YieldOutput(Result);

            return true;
        }

        public override string ToCodeSimple()
        {
            String Result = "write(";

            if (this.Arguments.Any())
            {
                Result += this.Arguments[0].ToCode();

                for (int i = 1; i < this.Arguments.Count; i++)
                {
                    Result += ", " + this.Arguments[i].ToCode();
                }
            }
            Result += ")";

            return Result;
        }
    }
}
