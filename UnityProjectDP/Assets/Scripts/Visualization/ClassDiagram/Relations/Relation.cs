using System;

namespace Visualization.ClassDiagram.Relations
{
    [Serializable]
    public class Relation
    {
        public string ConnectorXmiId;
        public string SourceModelType;
        public string SourceModelName;
        public string SourceTypeAggregation;
        public string TargetModelType;
        public string TargetModelName;
        public string PropertiesEaType;
        public string PropertiesDirection;
        public string ExtendedPropertiesVirtualInheritance;

        public string FromClass;
        public string ToClass;

        public string OALName;
    }
}