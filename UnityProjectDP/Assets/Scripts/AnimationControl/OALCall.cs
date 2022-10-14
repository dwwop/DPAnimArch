using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OALProgramControl
{
    public class OALCall
    {
        public String CallerClassName { get; }
        public String CallerMethodName { get; }
        public String RelationshipName { get; }
        public String CalledClassName { get; }
        public String CalledMethodName { get; }
        public long CalledInstanceId { get; }
        public Boolean NoRelationship { get; }

        public OALCall(String CallerClassName, String CallerMethodName, String RelationshipName, String CalledClassName, String CalledMethodName, Boolean NoRelationship)
        {
            this.CallerClassName = CallerClassName;
            this.CallerMethodName = CallerMethodName;
            this.RelationshipName = RelationshipName;
            this.CalledClassName = CalledClassName;
            this.CalledMethodName = CalledMethodName;
            this.NoRelationship = NoRelationship;
        }
        public OALCall(String CallerClassName, String CallerMethodName, String RelationshipName, String CalledClassName, String CalledMethodName, long CalledInstanceId, Boolean NoRelationship)
        {
            this.CallerClassName = CallerClassName;
            this.CallerMethodName = CallerMethodName;
            this.RelationshipName = RelationshipName;
            this.CalledClassName = CalledClassName;
            this.CalledMethodName = CalledMethodName;
            this.CalledInstanceId = CalledInstanceId;
            this.NoRelationship = NoRelationship;
        }
    }
}
