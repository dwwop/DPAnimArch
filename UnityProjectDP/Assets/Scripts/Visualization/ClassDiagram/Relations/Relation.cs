using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AnimArch.Visualization.Diagrams
{
    public class Relation
    {
        public string ConnectorXmiId { get; set; }
        public string SourceModelType { get; set; }
        public string SourceModelName { get; set; }
        public string SourceTypeAggregation { get; set; }
        public string TargetModelType { get; set; }
        public string TargetModelName { get; set; }
        public string PropertiesEaType { get; set; }
        public string PropertiesDirection { get; set; }
        public string ExtendedPropertiesVirtualInheritance { get; set; }


        public GameObject PrefabType { get; set; }
        public string FromClass { get; set; }
        public string ToClass { get; set; }

        public string OALName;
    }
}