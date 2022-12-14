using System;
using System.Collections.Generic;

namespace OALProgramControl
{
    public class CDRelationship
    {
        public String FromClass { set; get; }
        public String ToClass { set; get; }
        public String RelationshipName { get; }

        private List<(long, long)> InstanceRelationships { get; }

        public CDRelationship(String FromClass, String ToClass, String Name)
        {
            this.FromClass = FromClass;
            this.ToClass = ToClass;
            this.RelationshipName = Name;
            this.InstanceRelationships = new List<(long, long)>();
        }
        //SetUloh3
        //return list of all ids related to given id
        //do not forget that relationships are bi-directional
        //if no instances are related, return empty list
        //create unit tests for this one (keep the naming convention as it is in other tests). Do not forget about edge cases
        public List<long> GetRelatedInstaceIds(long Id)
        {
            List<long> Result = new List<long>();
            foreach ((long, long) IdTupple in this.InstanceRelationships)
            {
                if (IdTupple.Item1 == Id)
                {
                    Result.Add(IdTupple.Item2);
                }
                else if (IdTupple.Item2 == Id)
                {
                    Result.Add(IdTupple.Item1);
                }
            }
            return Result;
        }
        public Boolean CreateRelationship(long Instance1Id, long Instance2Id)
        {
            Boolean Result = false;

            if (InstanceRelationshipExists(Instance1Id, Instance2Id))
            {
                Result = false;
            }
            else
            {
                this.InstanceRelationships.Add((Instance1Id, Instance2Id));
                Result = true;
            }

            return Result;
        }
        public Boolean InstanceRelationshipExists(long Instance1Id, long Instance2Id)
        {
            Boolean Result = false;
            foreach ((long, long) Tupple in this.InstanceRelationships)
            {
                if (Tupple == (Instance1Id, Instance2Id) || Tupple == (Instance2Id, Instance1Id))
                {
                    Result = true;
                    break;
                }
            }
            return Result;
        }
        public Boolean DestroyRelationship(long Instance1Id, long Instance2Id)
        {
            Boolean Result = false;

            for (int i = 0; i < this.InstanceRelationships.Count; i++)
            {
                if ((Instance1Id, Instance2Id) == this.InstanceRelationships[i] || (Instance2Id, Instance1Id) == this.InstanceRelationships[i])
                {
                    this.InstanceRelationships.RemoveAt(i);
                    Result = true;
                    break;
                }
            }

            return Result;
        }
        public void ClearRelationships()
        {
            this.InstanceRelationships.Clear();
        }

        public List<(String, long, long)> GetTupples()
        {
            List<(String, long, long)> Tupples = new List<(String, long, long)>();
            foreach ((long, long) Tupple in this.InstanceRelationships)
            {
                Tupples.Add((this.RelationshipName, Tupple.Item1, Tupple.Item2));
            }
            if (Tupples.Count == 0)
            {
                Tupples.Add((this.RelationshipName, int.MinValue, int.MinValue));
            }
            return Tupples;
        }
    }
}
