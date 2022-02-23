using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AnimArch.Extensions.Unity
{
    public static class RectTransformExtension
    {
        public static void Shift(this RectTransform rectTransform, int xShift = 0, int yShift = 0, int zShift = 0)
        {
            rectTransform.position
                = new Vector3
                (
                    rectTransform.position.x + xShift,
                    rectTransform.position.y + yShift,
                    rectTransform.position.z + zShift
                );
        }
    }
}
