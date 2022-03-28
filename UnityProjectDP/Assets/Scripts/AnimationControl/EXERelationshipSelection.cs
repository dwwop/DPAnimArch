using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public class EXERelationshipSelection
    {
        public String StartingVariableName { get; }
        public String StartingAttributeName { get; }
        private List<EXERelationshipLink> RelationshipSpecification { get; }
        public EXERelationshipSelection(String StartingVariableName, String StartingAttributeName)
        {
            this.StartingVariableName = StartingVariableName;
            this.StartingAttributeName = StartingAttributeName;
            this.RelationshipSpecification = new List<EXERelationshipLink>();
        }
        public EXERelationshipSelection(String StartingVariableName, String StartingAttributeName, EXERelationshipLink[] RelLinks)
        {
            this.StartingVariableName = StartingVariableName;
            this.StartingAttributeName = StartingAttributeName;
            this.RelationshipSpecification = RelLinks.ToList();
        }
        public void AddRelationshipLink(EXERelationshipLink RelationshipLink)
        {
            this.RelationshipSpecification.Add(RelationshipLink);
        }

        public List<long> Evaluate(OALProgram OALProgram, EXEScope Scope)
        {
            List<long> Result = null;
            String CurrentClass = "";
            List<long> CurrentIds = new List<long>(new long[] { });

            if (this.StartingAttributeName == null)
            {
                EXEReferenceHandle StartVariable = Scope.FindReferenceHandleByName(this.StartingVariableName);
                if (StartVariable == null || !this.RelationshipSpecification.Any())
                {
                    return null;
                }

                CurrentIds = StartVariable.GetReferencedIds();
                if (CurrentIds == null)
                {
                    return null;
                }

                CurrentClass = StartVariable.ClassName;
            }
            else
            {
                EXEReferencingVariable Variable = Scope.FindReferencingVariableByName(this.StartingVariableName);
                if (Variable == null || !this.RelationshipSpecification.Any())
                {
                    return null;
                }

                CDClass VariableClass = OALProgram.ExecutionSpace.getClassByName(Variable.ClassName);
                if (VariableClass == null)
                {
                    return null;
                }

                CDAttribute Attribute = VariableClass.GetAttributeByName(this.StartingAttributeName);
                if (Attribute == null)
                {
                    return null;
                }

                CDClassInstance ClassInstance = VariableClass.GetInstanceByID(Variable.ReferencedInstanceId);
                if (ClassInstance == null)
                {
                    return null;
                }

                String IDValue = ClassInstance.GetAttributeValue(this.StartingAttributeName);

                if (!EXETypes.IsValidReferenceValue(IDValue, Attribute.Type))
                {
                    return null;
                }

                if ("[]".Equals(Attribute.Type.Substring(Attribute.Type.Length - 2, 2)))
                {
                    CurrentClass = Attribute.Type.Substring(0, Attribute.Type.Length - 2);

                    CDClass AttributeClass = OALProgram.ExecutionSpace.getClassByName(CurrentClass);
                    if (AttributeClass == null)
                    {
                        return null;
                    }

                    if (!String.Empty.Equals(IDValue))
                    {
                        CurrentIds = IDValue.Split(',').Select(id => long.Parse(id)).ToList().FindAll(x => x >= 0);
                    }

                    CDClassInstance Instance;
                    foreach (long ID in CurrentIds)
                    {
                        Instance = AttributeClass.GetInstanceByID(ID);
                        if (Instance == null)
                        {
                            return null;
                        }
                    }
                }
                else
                {
                    CurrentClass = Attribute.Type;

                    CDClass AttributeClass = OALProgram.ExecutionSpace.getClassByName(CurrentClass);
                    if (AttributeClass == null)
                    {
                        return null;
                    }

                    long ID = long.Parse(IDValue);
                    if (ID >= 0)
                    {
                        CDClassInstance Instance = AttributeClass.GetInstanceByID(ID);
                        if (Instance == null)
                        {
                            return null;
                        }

                        CurrentIds.Add(ID);
                    }
                }
            }

            foreach (EXERelationshipLink RelationshipLink in this.RelationshipSpecification)
            {
                if (CurrentIds == null || !CurrentIds.Any())
                {
                    break;
                }
                CurrentIds = RelationshipLink.RetrieveIds(CurrentIds, CurrentClass, OALProgram.RelationshipSpace);
                CurrentClass = RelationshipLink.ClassName;
            }
            Result = CurrentIds;

            return Result;  
        }
        public String GetLastClassName()
        {
            String Result = null;
            if (this.RelationshipSpecification != null && this.RelationshipSpecification.Any())
            {
                Result = this.RelationshipSpecification.Last().ClassName;
            }
            return Result;
        }
        public String ToCode()
        {
            String Result = (this.StartingAttributeName == null ? this.StartingVariableName : (this.StartingVariableName + "." + this.StartingAttributeName));
            foreach (EXERelationshipLink Link in this.RelationshipSpecification)
            {
                Result += "->" + Link.ClassName + "[" + Link.RelationshipName + "]";
            }
            return Result;
        }
    }
}
