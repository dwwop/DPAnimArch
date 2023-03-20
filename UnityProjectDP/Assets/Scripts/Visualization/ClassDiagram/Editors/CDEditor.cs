using System;
using OALProgramControl;
using Visualization.ClassDiagram.ClassComponents;
using Visualization.ClassDiagram.ComponentsInDiagram;
using Visualization.ClassDiagram.Relations;
using Attribute = Visualization.ClassDiagram.ClassComponents.Attribute;

namespace Visualization.ClassDiagram.Editors
{
    public static class CDEditor
    {
        public static CDClass CreateNode(Class newClass)
        {
            CDClass cdClass = null;
            var i = 0;
            var baseName = newClass.Name;
            while (cdClass == null)
            {
                newClass.Name = baseName + (i == 0 ? "" : i.ToString());
                cdClass = OALProgram.Instance.ExecutionSpace.SpawnClass(newClass.Name);
                i++;
                if (i > 1000) break;
            }

            return cdClass;
        }

        private static CDAttribute CreateCdAttributeFromAttribute(Attribute attribute)
        {
            return new CDAttribute(attribute.Name, EXETypes.ConvertEATypeName(attribute.Type));
        }

        public static void AddAttribute(ClassInDiagram classInDiagram, Attribute attribute)
        {
            var cdAttribute = CreateCdAttributeFromAttribute(attribute);
            classInDiagram.ClassInfo.AddAttribute(cdAttribute);
        }

        public static void UpdateAttribute(ClassInDiagram classInDiagram, string oldAttribute, Attribute newAttribute)
        {
            var index = classInDiagram.ClassInfo.Attributes.FindIndex(x => x.Name == oldAttribute);
            var newCdAttribute = CreateCdAttributeFromAttribute(newAttribute);
            classInDiagram.ClassInfo.Attributes[index] = newCdAttribute;
        }

        private static CDMethod CreateCdMethodFromMethod(CDClass cdClass, Method method)
        {
            return new CDMethod(cdClass, method.Name, EXETypes.ConvertEATypeName(method.ReturnValue));
        }

        public static void AddMethod(ClassInDiagram classInDiagram, Method method)
        {
            var cdMethod = CreateCdMethodFromMethod(classInDiagram.ClassInfo, method);

            if (method.arguments != null)
                AddParameters(method, cdMethod);

            classInDiagram.ClassInfo.AddMethod(cdMethod);
        }


        private static void AddParameters(Method method, CDMethod cdMethod)
        {
            foreach (var argument in method.arguments)
            {
                var tokens = argument.Split(' ');
                var type = tokens[0];
                var name = tokens[1];

                cdMethod.Parameters.Add(new CDParameter { Name = name, Type = EXETypes.ConvertEATypeName(type) });
            }
        }

        public static void UpdateMethod(ClassInDiagram classInDiagram, string oldMethod, Method newMethod)
        {
            var index = classInDiagram.ClassInfo.Methods.FindIndex(x => x.Name == oldMethod);
            var newCdMethod = CreateCdMethodFromMethod(classInDiagram.ClassInfo, newMethod);

            if (newMethod.arguments != null)
                AddParameters(newMethod, newCdMethod);

            classInDiagram.ClassInfo.Methods[index] = newCdMethod;
        }

        public static CDRelationship CreateRelation(Relation relation)
        {
            var cdRelationship =
                OALProgram.Instance.RelationshipSpace.SpawnRelationship(relation.FromClass, relation.ToClass)
                ?? throw new ArgumentNullException(nameof(relation));
            relation.OALName = cdRelationship.RelationshipName;

            if (!"Generalization".Equals(relation.PropertiesEaType) && !"Realisation".Equals(relation.PropertiesEaType))
                return cdRelationship;

            var fromClass = OALProgram.Instance.ExecutionSpace.getClassByName(relation.FromClass);
            var toClass = OALProgram.Instance.ExecutionSpace.getClassByName(relation.ToClass);

            if (fromClass != null && toClass != null) fromClass.SuperClass = toClass;

            return cdRelationship;
        }

        public static void DeleteAttribute(ClassInDiagram classInDiagram, string attribute)
        {
            classInDiagram.ClassInfo.Attributes.RemoveAll(x => x.Name == attribute);
        }

        public static void DeleteMethod(ClassInDiagram classInDiagram, string method)
        {
            classInDiagram.ClassInfo.Methods.RemoveAll(x => x.Name == method);
        }

        public static void DeleteNode(ClassInDiagram classInDiagram)
        {
            OALProgram.Instance.ExecutionSpace.ClassPool.Remove(classInDiagram.ClassInfo);
        }

        public static void DeleteRelation(RelationInDiagram relationInDiagram)
        {
            OALProgram.Instance.RelationshipSpace.RemoveRelationship(relationInDiagram.RelationInfo);

            if (!"Generalization".Equals(relationInDiagram.ParsedRelation.PropertiesEaType) &&
                !"Realisation".Equals(relationInDiagram.ParsedRelation.PropertiesEaType))
                return;

            var fromClass = OALProgram.Instance.ExecutionSpace.getClassByName(relationInDiagram.RelationInfo.FromClass);
            OALProgram.Instance.ExecutionSpace.getClassByName(relationInDiagram.RelationInfo.FromClass)
                .SuperClass = null;

            OALProgram.Instance.ExecutionSpace.getClassByName(relationInDiagram.RelationInfo.ToClass).SubClasses
                .Remove(fromClass);
        }
    }
}
