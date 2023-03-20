using System;
using System.Collections.Generic;
using System.Linq;

namespace Visualization.Animation
{
    class OALScriptBuilder
    {
        private static OALScriptBuilder Instance = null;
        private Dictionary<String,String> InstantiatedClasses { get; }
        private List<String> InstantiatedRelationships { get; }

        private OALScriptBuilder()
        {
            this.InstantiatedClasses = new Dictionary<string, string>();
            this.InstantiatedRelationships = new List<string>();
        }
        public static OALScriptBuilder GetInstance()
        {
            if (Instance == null)
            {
                Instance = new OALScriptBuilder();
            }
            return Instance;
        }
        public void Clear()
        {
            this.InstantiatedClasses.Clear();
            this.InstantiatedRelationships.Clear();
        }
        public String AddCall(String FromClass, String FromMethod, String Relationship, String ToClass, String ToMethod)
        {
            String Result = "";

            /*if (!this.InstantiatedClasses.ContainsKey(FromClass))
            {
                String VarName = ProduceVariableName(FromClass);
                Result += "create object instance " + VarName + " of " + FromClass + ";\n";
                InstantiatedClasses.Add(FromClass, VarName);
            }

            if (!this.InstantiatedClasses.ContainsKey(ToClass))
            {
                String VarName = ProduceVariableName(ToClass);
                Result += "create object instance " + VarName + " of " + ToClass + ";\n";
                InstantiatedClasses.Add(ToClass, VarName);
            }

            if (!this.InstantiatedRelationships.Contains(Relationship))
            {
                Result += "relate " + this.InstantiatedClasses[FromClass] + " to " + this.InstantiatedClasses[ToClass] + " across "  + Relationship + ";\n";
                this.InstantiatedRelationships.Add(Relationship);
            }*/

            /*Result += "call from " + FromClass + "::" + FromMethod + " to "
            + ToClass + "::" + ToMethod + " across " + Relationship + ";\n";*/

            //
            String InstanceName = ToClass.ToLower() + "_1";
            Result += "create object instance " + InstanceName + " of " + ToClass + ";\n"
                + InstanceName + "." + ToMethod + ";\n";
            //

            return Result;
        }
        private String ProduceVariableName(String ClassName)
        {
            String Result = string.Concat(ClassName.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
            return Result;
        }
    }
}
