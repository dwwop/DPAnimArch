using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public class EXERelationshipSelection
    {
        public String StartingVariable { get; }
        private List<EXERelationshipLink> RelationshipSpecification { get; }
        public EXERelationshipSelection(String StartingVariable)
        {
            this.StartingVariable = StartingVariable;
            this.RelationshipSpecification = new List<EXERelationshipLink>();
        }
        public EXERelationshipSelection(String StartingVariable, EXERelationshipLink[] RelLinks)
        {
            this.StartingVariable = StartingVariable;
            this.RelationshipSpecification = RelLinks.ToList();
        }
        public void AddRelationshipLink(EXERelationshipLink RelationshipLink)
        {
            this.RelationshipSpecification.Add(RelationshipLink);
        }

        public List<long> Evaluate(CDRelationshipPool RelationshipSpace, EXEScope Scope)
        {
            List<long> Result = null;
            EXEReferenceHandle StartVariable = Scope.FindReferenceHandleByName(this.StartingVariable);
            if (StartVariable != null && this.RelationshipSpecification.Any())
            {
                List<long> CurrentIds = StartVariable.GetReferencedIds();
                if (CurrentIds == null)
                {
                    return null;
                }

                String CurrentClass = StartVariable.ClassName;
                foreach(EXERelationshipLink RelationshipLink in this.RelationshipSpecification)
                {
                    if (CurrentIds == null || !CurrentIds.Any())
                    {
                        break;
                    }
                    CurrentIds = RelationshipLink.RetrieveIds(CurrentIds, CurrentClass, RelationshipSpace);
                    CurrentClass = RelationshipLink.ClassName;
                }
                Result = CurrentIds;
            }
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
            String Result = this.StartingVariable;
            foreach (EXERelationshipLink Link in this.RelationshipSpecification)
            {
                Result += "->" + Link.ClassName + "[" + Link.RelationshipName + "]";
            }
            return Result;
        }
    }
}
