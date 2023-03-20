using UnityEngine;

namespace Visualization.ClassDiagram.Relations
{
    public class Arrow : MonoBehaviour
    {
        public void Initialize()
        {
            GetComponent<LineRenderer>().material = Resources.Load<Material>("Materials/Arrow");
            GetComponent<LineRenderer>().widthMultiplier = 20f;
        }

        public void UpdatePosition(Vector3 obj, Vector3 vectorToClass)
        {
            GetComponent<LineRenderer>().SetPositions(new[]
                {
                    obj, vectorToClass
                }
            );
        }
    }
}