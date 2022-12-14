using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AnimArch.Visualization.Diagrams
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

        [NonSerialized] 
        public GameObject PrefabType;
        public string FromClass;
        public string ToClass;

        public string OALName;
    }
}