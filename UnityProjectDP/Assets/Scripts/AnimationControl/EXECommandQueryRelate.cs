using System;

namespace OALProgramControl
{
    public class EXECommandQueryRelate : EXECommand
    {
        public String Variable1Name { get; }
        public String Attribute1Name { get; }
        public String Variable2Name { get; }
        public String Attribute2Name { get; }
        public String RelationshipName { get; }

        public EXECommandQueryRelate(String Variable1Name, String Attribute1Name, String Variable2Name, String Attribute2Name, String RelationshipName)
        {
            this.Variable1Name = Variable1Name;
            this.Attribute1Name = Attribute1Name;
            this.Variable2Name = Variable2Name;
            this.Attribute2Name = Attribute2Name;
            this.RelationshipName = RelationshipName;
        }
        // Create a relationship instance (between two variables pointing to class instances)
        // Based on class names get the CDRelationship from RelationshipSpace
        // Based on variable names get the instance ids from Scope.ReferencingVariables
        // Create relationship between the given instance ids (CDRelationship.CreateRelationship) and return result of it
        protected override bool Execute(OALProgram OALProgram)
        {
            EXEReferencingVariable Variable1 = SuperScope.FindReferencingVariableByName(this.Variable1Name);
            if (Variable1 == null)
            {
                return false;
            }

            String Variable1ClassName = Variable1.ClassName;
            long Variable1InstanceId = Variable1.ReferencedInstanceId;
            if (this.Attribute1Name != null)
            {
                CDClass Variable1Class = OALProgram.ExecutionSpace.getClassByName(Variable1.ClassName);
                if (Variable1Class == null)
                {
                    return false;
                }

                CDAttribute Attribute1 = Variable1Class.GetAttributeByName(this.Attribute1Name);
                if (Attribute1 == null)
                {
                    return false;
                }

                Variable1ClassName = Attribute1.Type;

                CDClassInstance ClassInstance = Variable1Class.GetInstanceByID(Variable1.ReferencedInstanceId);
                if (ClassInstance == null)
                {
                    return false;
                }

                if (!long.TryParse(ClassInstance.GetAttributeValue(this.Attribute1Name), out Variable1InstanceId))
                {
                    return false;
                }
            }

            EXEReferencingVariable Variable2 = SuperScope.FindReferencingVariableByName(this.Variable2Name);
            if (Variable2 == null)
            {
                return false;
            }

            String Variable2ClassName = Variable2.ClassName;
            long Variable2InstanceId = Variable2.ReferencedInstanceId;
            if (this.Attribute2Name != null)
            {
                CDClass Variable2Class = OALProgram.ExecutionSpace.getClassByName(Variable2.ClassName);
                if (Variable2Class == null)
                {
                    return false;
                }

                CDAttribute Attribute2 = Variable2Class.GetAttributeByName(this.Attribute2Name);
                if (Attribute2 == null)
                {
                    return false;
                }

                Variable2ClassName = Attribute2.Type;

                CDClassInstance ClassInstance = Variable2Class.GetInstanceByID(Variable2.ReferencedInstanceId);
                if (ClassInstance == null)
                {
                    return false;
                }

                if (!long.TryParse(ClassInstance.GetAttributeValue(this.Attribute2Name), out Variable2InstanceId))
                {
                    return false;
                }
            }

            CDRelationship Relationship = OALProgram.RelationshipSpace.GetRelationship(this.RelationshipName, Variable1ClassName, Variable2ClassName);
            if (Relationship == null)
            {
                return false;
            }
            bool Success = Relationship.CreateRelationship(Variable1InstanceId, Variable2InstanceId);

            return Success;
        }
        public override string ToCodeSimple()
        {
            return "relate " + (this.Attribute1Name == null ? this.Variable1Name : (this.Variable1Name + "." + this.Attribute1Name))
                + " to " + (this.Attribute2Name == null ? this.Variable2Name : (this.Variable2Name + "." + this.Attribute2Name))
                + " across " + this.RelationshipName;
        }

        public override EXECommand CreateClone()
        {
            return new EXECommandQueryRelate(Variable1Name, Attribute1Name, Variable2Name, Attribute2Name, RelationshipName);
        }
    }
}
