using System;
using System.Collections.Generic;
using AnimArch.Extensions;
using AnimArch.Visualization.UI;
using Networking;
using OALProgramControl;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace AnimArch.Visualization.Diagrams
{
    public class ClassEditor : Singleton<ClassEditor>
    {



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
    }
}