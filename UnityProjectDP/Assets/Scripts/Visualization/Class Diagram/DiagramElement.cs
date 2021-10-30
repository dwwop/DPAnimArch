using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimArch.Visualization.ClassDiagram
{
    public class DiagramElement<T>
    {
        public readonly T DataObject;
        public GameObject VisualObject;

        public DiagramElement(T DataObject)
        {
            this.DataObject = DataObject;
            this.VisualObject = null;
        }
        public DiagramElement(T DataObject, GameObject VisualObject)
        {
            this.DataObject = DataObject;
            this.VisualObject = VisualObject;
        }

        internal void ManipulateGameObject(Action<GameObject> ManipulationFunction)
        {
            ManipulationFunction(VisualObject);
        }
    }
}