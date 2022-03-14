using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AnimArch.Visualization.Diagrams
{
    public class Diagram : MonoBehaviour
    {
        public GameObject CreateInterGraphLine(GameObject start, GameObject end)
        {
            GameObject Line = Instantiate(DiagramPool.Instance.interGraphLinePrefab);

            Line.GetComponent<LineRenderer>().SetPositions
            (
                new Vector3[]
                {
                    start.GetComponent<RectTransform>().position,
                    end.GetComponent<RectTransform>().position
                }
            );

            Line.GetComponent<LineRenderer>().widthMultiplier = 6f;
            //Line.transform.SetParent(graph.units);

            return Line;
        }
    }
}
