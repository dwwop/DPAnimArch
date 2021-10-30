using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimArch.Visualization.ClassDiagram
{
    public class DiagramElement<T>
    {
        public readonly T DataObject;
        public readonly GameObject VisualObject;

        public DiagramElement(T DataObject, GameObject VisualObject)
        {
            this.DataObject = DataObject;
            this.VisualObject = VisualObject;
        }
    }
}