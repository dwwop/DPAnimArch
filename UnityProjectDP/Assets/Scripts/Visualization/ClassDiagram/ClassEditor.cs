using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AnimArch.Extensions;
using AnimArch.Parsing;
using AnimArch.Visualization.Animating;
using AnimArch.Visualization.UI;
using Networking;
using OALProgramControl;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace AnimArch.Visualization.Diagrams
{
    public class ClassEditor : Singleton<ClassEditor>
    {
        public enum Source
        {
            RPC,
            editor,
            loader
        }

        public static void ReverseClassesGeometry()
        {
            foreach (var parsedClass in DiagramPool.Instance.ClassDiagram.GetClassList())
            {
                parsedClass.Top *= -1;
            }
        }


        // Parser used to parse data from XML to C# data structures

        public static void CreateRelation(Relation relation)
        {
            Spawner.Instance.AddRelation(relation.FromClass, relation.ToClass, relation.PropertiesEaType);
        }

        public void CreateRelation(string sourceClass, string destinationClass, string relationType, bool fromRpc,
            bool noDirection = false)
        {
            if (!fromRpc)
                Spawner.Instance.AddRelation(sourceClass, destinationClass, relationType);
            var relation = new Relation
            {
                SourceModelName = sourceClass,
                TargetModelName = destinationClass,
                PropertiesEaType = relationType,
                PropertiesDirection = noDirection ? "none" : "Source -> Destination"
            };

            var relInDiag = CreateRelationEdge(relation);
            var sourceClassGo = DiagramPool.Instance.ClassDiagram.FindClassByName(sourceClass).VisualObject;
            var destinationClassGo = DiagramPool.Instance.ClassDiagram.FindClassByName(destinationClass).VisualObject;
            var edge = DiagramPool.Instance.ClassDiagram.graph
                .AddEdge(sourceClassGo, destinationClassGo, relation.PrefabType);
            relInDiag.VisualObject = edge;
        }

        public static RelationInDiagram CreateRelationEdge(Relation relation)
        {
            relation.FromClass = relation.SourceModelName.Replace(" ", "_");
            relation.ToClass = relation.TargetModelName.Replace(" ", "_");

            relation.PrefabType = relation.PropertiesEaType switch
            {
                "Association" => relation.PropertiesDirection switch
                {
                    "Source -> Destination" => DiagramPool.Instance.associationSDPrefab,
                    "Destination -> Source" => DiagramPool.Instance.associationDSPrefab,
                    "Bi-Directional" => DiagramPool.Instance.associationFullPrefab,
                    _ => DiagramPool.Instance.associationNonePrefab
                },
                "Generalization" => DiagramPool.Instance.generalizationPrefab,
                "Dependency" => DiagramPool.Instance.dependsPrefab,
                "Realisation" => DiagramPool.Instance.realisationPrefab,
                _ => DiagramPool.Instance.associationNonePrefab
            };

            var tempCdRelationship =
                OALProgram.Instance.RelationshipSpace.SpawnRelationship(relation.FromClass, relation.ToClass)
                ?? throw new ArgumentNullException(nameof(relation));
            relation.OALName = tempCdRelationship.RelationshipName;

            if ("Generalization".Equals(relation.PropertiesEaType) || "Realisation".Equals(relation.PropertiesEaType))
            {
                var fromClass = OALProgram.Instance.ExecutionSpace.getClassByName(relation.FromClass);
                var toClass = OALProgram.Instance.ExecutionSpace.getClassByName(relation.ToClass);

                if (fromClass != null && toClass != null)
                {
                    fromClass.SuperClass = toClass;
                }
            }

            var relInDiag = new RelationInDiagram { ParsedRelation = relation, RelationInfo = tempCdRelationship };
            DiagramPool.Instance.ClassDiagram.Relations.Add(relInDiag);
            return relInDiag;
        }

        private static Transform GetAttributeLayoutGroup(GameObject classGo)
        {
            return classGo.transform
                .Find("Background")
                .Find("Attributes")
                .Find("AttributeLayoutGroup");
        }
        


        private static Transform GetClassHeader(GameObject classGo)
        {
            return classGo.transform.Find("Background").Find("HeaderLayout").Find("Header");
        }
        


        public static bool AddAttribute(string targetClass, Attribute attributeToAdd)
        {
            var c = DiagramPool.Instance.ClassDiagram.FindClassByName(targetClass);
            if (c == null)
            {
                return false;
            }

            c.ParsedClass.Attributes ??= new List<Attribute>();

            attributeToAdd.Id = (c.ParsedClass.Attributes.Count + 1).ToString();
            if (DiagramPool.Instance.ClassDiagram.FindAttributeByName(targetClass, attributeToAdd.Name) != null)
                return false;
            c.ParsedClass.Attributes.Add(attributeToAdd);
            return true;
        }

        public static void AddAttribute(string targetClass, Attribute attribute, bool fromRpc)
        {
            if (!fromRpc)
                Spawner.Instance.AddAttribute(targetClass, attribute.Name, attribute.Type);
            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(targetClass);
            var attributeText = attribute.Name + ": " + attribute.Type + "\n";
            AddTmProAttribute(classInDiagram.VisualObject, attributeText);
        }

        public static bool UpdateAttribute(string targetClass, string oldAttribute, Attribute newAttribute)
        {
            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(targetClass);
            if (classInDiagram == null)
                return false;

            if (DiagramPool.Instance.ClassDiagram.FindAttributeByName(targetClass, newAttribute.Name) != null)
                return false;

            var index = classInDiagram.ParsedClass.Attributes.FindIndex(x => x.Name == oldAttribute);
            var formerName = classInDiagram.ParsedClass.Attributes[index].Name;
            var formerType = classInDiagram.ParsedClass.Attributes[index].Type;
            newAttribute.Id = classInDiagram.ParsedClass.Attributes[index].Id;
            classInDiagram.ParsedClass.Attributes[index] = newAttribute;

            var oldAttributeText = formerName + ": " + formerType + "\n";
            var newAttributeText = newAttribute.Name + ": " + newAttribute.Type + "\n";
            UpdateTmProAttribute(classInDiagram.VisualObject, oldAttributeText, newAttributeText);
            return true;
        }

        // called at manual layout
        public static void SetPosition(string className, Vector3 position, bool fromRpc)
        {
            if (!fromRpc)
                Spawner.Instance.SetPosition(className, position);
            var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(className);
            if (classInDiagram != null)
            {
                classInDiagram
                    .VisualObject
                    .GetComponent<RectTransform>()
                    .position = position;
            }
        }


        private static void AddTmProAttribute(GameObject classGo, string attribute)
        {
            var attributesTransform = GetAttributeLayoutGroup(classGo);

            var instance = Instantiate(DiagramPool.Instance.classAttributePrefab, attributesTransform, false);
            instance.name = attribute;
            instance.transform.Find("AttributeText").GetComponent<TextMeshProUGUI>().text += attribute;

            instance.GetComponent<AttributePopUpManager>().classTxt =
                GetClassHeader(classGo).GetComponent<TextMeshProUGUI>();

            if (UIEditorManager.Instance.active)
                instance.GetComponentsInChildren<Button>(includeInactive: true)
                    .ForEach(x => x.gameObject.SetActive(true));
        }

        private static void UpdateTmProAttribute(GameObject classGo, string oldAttributeText, string newAttributeText)
        {
            var oldAttribute = GetAttributeLayoutGroup(classGo).Find(oldAttributeText);

            oldAttribute.name = newAttributeText;
            oldAttribute.Find("AttributeText").GetComponent<TextMeshProUGUI>().text = newAttributeText;
        }
    }
}