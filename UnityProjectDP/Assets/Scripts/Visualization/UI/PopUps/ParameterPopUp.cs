using AnimArch.Visualization.Diagrams;
using OALProgramControl;
using TMPro;
using UnityEngine.UI;

namespace AnimArch.Visualization.UI
{
    public class ParameterPopUp : AbstractPopUp
    {
        public TMP_Dropdown dropdown;

        public void ActivateCreation()
        {
            panel.SetActive(true);
        }

        public override void Confirmation()
        {
            if (inp.text == "")
            {
                Deactivate();
                return;
            }

            var param = inp.text.Replace(" ", "_") + " " + dropdown.options[dropdown.value].text;
            ClassEditor.Instance.mtdPopUp.AddArg(param);

            Deactivate();
        }
    }
}