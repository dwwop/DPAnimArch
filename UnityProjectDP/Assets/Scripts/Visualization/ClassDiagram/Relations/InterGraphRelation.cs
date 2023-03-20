using UnityEngine;
using Visualization.ClassDiagram.ComponentsInDiagram;

namespace Visualization.ClassDiagram.Relations
{
    public class InterGraphRelation : MonoBehaviour
    {
        public ObjectInDiagram Object;
        public ClassInDiagram Class;
        private Vector3 _prevClassPos;
        private Vector3 _prevObjPos;

        private GameObject _interGraphArrow;
        private Arrow _arrow;
        
        private LineRenderer _lineRenderer;
        public LineTextureMode textureMode = LineTextureMode.RepeatPerSegment;

        public void Initialize(ObjectInDiagram Object, ClassInDiagram Class)
        {
            this.Object = Object;
            this.Class = Class;
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.textureMode = textureMode;
            _lineRenderer.material = Resources.Load<Material>("Materials/DashedLine");
            
            // _lineRenderer.material.SetTextureScale("_MainTex", new Vector2(100, 1));
             
            _interGraphArrow = Instantiate(DiagramPool.Instance.interGraphArrowPrefab);
            _arrow = _interGraphArrow.GetComponent<Arrow>();
            _arrow.GetComponent<Arrow>().Initialize();
        }

        void Update()
        {
            if (_prevClassPos != Class.VisualObject.GetComponent<RectTransform>().position
                || _prevObjPos != Object.VisualObject.GetComponent<RectTransform>().position)
            {
                _prevObjPos = Object.VisualObject.GetComponent<RectTransform>().position;
                _lineRenderer.SetPositions
                (
                    new Vector3[]
                    {
                        _prevClassPos = Class.VisualObject.GetComponent<RectTransform>().position,
                        Vector3.MoveTowards(_prevClassPos, _prevObjPos, (_prevClassPos - _prevObjPos).magnitude - 6)
                    }
                );

                Vector3 v = Vector3.MoveTowards(_prevObjPos, _prevClassPos, 18);
                _arrow.UpdatePosition(_prevObjPos, v);
                _lineRenderer.textureMode = textureMode;
            }
        }

        public void Hide()
        {
            _lineRenderer.enabled = false;
            _interGraphArrow.GetComponent<LineRenderer>().enabled = false;
        }
        
        public void Show()
        {
            _lineRenderer.enabled = true;
            _interGraphArrow.GetComponent<LineRenderer>().enabled = true;
        }

        public void Destroy()
        {
            Destroy(_lineRenderer);
            Destroy(_interGraphArrow);
        }

        public void Highlight()
        {
            _lineRenderer.startColor = Animation.Animation.Instance.relationColor;
            _lineRenderer.endColor = Animation.Animation.Instance.relationColor;
        }
        
        public void UnHighlight()
        {
            _lineRenderer.startColor = Color.white;
            _lineRenderer.endColor = Color.white;
        }
    }
}