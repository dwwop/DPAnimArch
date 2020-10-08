using System;
using System.Collections.Generic;
using System.Linq;

namespace OALProgramControl
{
    public class EXERelationshipLink
    {
        public String RelationshipName { get; }
        public String ClassName { get; }

        public EXERelationshipLink(String RelationshipName, String ClassName)
        {
            this.RelationshipName = RelationshipName;
            this.ClassName = ClassName;
        }
        public List<long> RetrieveIds(List<long> InputIds, String InputClass, CDRelationshipPool RelationshipSpace)
        {
            List<long> OutputIds = null;
            CDRelationship Relationship = RelationshipSpace.GetRelationship(RelationshipName, this.ClassName, InputClass);
            if (Relationship != null)
            {
                OutputIds = new List<long>();
                foreach (long Id in InputIds)
                {
                    OutputIds = OutputIds.Union(Relationship.GetRelatedInstaceIds(Id)).ToList();
                }
            }
            return OutputIds;
        }
    }
}
