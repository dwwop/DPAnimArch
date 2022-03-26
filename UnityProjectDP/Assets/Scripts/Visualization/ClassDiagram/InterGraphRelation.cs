using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AnimArch.Visualization.Diagrams
{
    public class InterGraphRelation : MonoBehaviour
    {
        public ObjectInDiagram Object;
        public ClassInDiagram Class;
        private Vector3 _prevClassPos;
        private Vector3 _prevObjPos;
        public void Initialize(ObjectInDiagram Object, ClassInDiagram Class)
        {
            this.Object = Object;
            this.Class = Class;
        }

        void Update()
        {
            if (_prevClassPos != Class.VisualObject.GetComponent<RectTransform>().position
                || _prevObjPos != Object.VisualObject.GetComponent<RectTransform>().position)
            {
                GetComponent<LineRenderer>().SetPositions
                (
                    new Vector3[]
                    {
                        _prevClassPos = Class.VisualObject.GetComponent<RectTransform>().position,
                        _prevObjPos = Object.VisualObject.GetComponent<RectTransform>().position
                    }
                );
            }
        }
    }
}
