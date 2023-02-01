using AnimArch.Visualization.Diagrams;
using UnityEngine;
using TMPro;

namespace AnimArch.Visualization.UI
{
    public class ClassPopUp : AbstractPopUp
    {
        public override void Confirmation()
        {
            if (inp.text != "")
            {
                MainEditor.UpdateNodeName(className.text, inp.text, false);
            }
            Deactivate();
        }
    }
}
