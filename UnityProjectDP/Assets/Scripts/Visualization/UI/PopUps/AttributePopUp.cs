using UnityEngine;
using TMPro;
using UnityEngine.UI;
using AnimArch.Visualization.Diagrams;

namespace AnimArch.Visualization.UI
{
    public class AttributePopUp : AbstractPopUp
    {
        public TMP_Dropdown dropdown;
        public Toggle isArray;

        public void ActivateCreation(TMP_Text classTxt, TMP_Text atrTxt)
        {   
            ActivateCreation(classTxt);
        }

        public override void Confirmation()
        {
            if (inp.text == "")
            {
                Deactivate();
                return;
            }

            var attribute = new Attribute
            {
                Name = inp.text,
                Type = (isArray.isOn ? "[]: " : ": ") + dropdown.options[dropdown.value].text
            };

            ClassEditor.AddAttribute(className.text, attribute, false);

            Deactivate();
        }

        public override void Deactivate()
        {   
            base.Deactivate();
            isArray.isOn = false;
        }
    }
}