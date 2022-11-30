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
                ClassEditor.SetClassName(ClassTxt.text, inp.text, false);
            }
            Deactivate();
        }
    }
}
