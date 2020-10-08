using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public class EXECommandQueryUnrelate : EXECommand
    {
        public String Variable1Name { get; }
        public String Variable2Name { get; }
        public String RelationshipName { get; }

        public EXECommandQueryUnrelate(String Variable1Name, String Variable2Name, String RelationshipName)
        {
            this.Variable1Name = Variable1Name;
            this.Variable2Name = Variable2Name;
            this.RelationshipName = RelationshipName;
        }
        // Create a relationship instance (between two variables pointing to class instances)
        // Based on class names get the CDRelationship from RelationshipSpace
        // Based on variable names get the instance ids from Scope.ReferencingVariables
        // Create relationship between the given instance ids (CDRelationship.CreateRelationship) and return result of it
        public override bool Execute(OALProgram OALProgram, EXEScope Scope)
        {
            EXEReferencingVariable Variable1 = Scope.FindReferencingVariableByName(this.Variable1Name);
            if (Variable1 == null)
            {
                return false;
            }
            EXEReferencingVariable Variable2 = Scope.FindReferencingVariableByName(this.Variable2Name);
            if (Variable2 == null)
            {
                return false;
            }
            CDRelationship Relationship = OALProgram.RelationshipSpace.GetRelationship(this.RelationshipName, Variable1.ClassName, Variable2.ClassName);
            if (Relationship == null)
            {
                return false;
            }
            bool Success = Relationship.DestroyRelationship(Variable1.ReferencedInstanceId, Variable2.ReferencedInstanceId);

            return Success;
        }
        public override string ToCodeSimple()
        {
            return "unrelate " + this.Variable1Name + " from " + this.Variable2Name + " across " + this.RelationshipName;
        }
    }
}
