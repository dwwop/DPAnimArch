using System.Text.RegularExpressions;
using AnimArch.Visualization.Diagrams;
using TMPro;
using UnityEngine.UI;

namespace AnimArch.Visualization.UI
{
    public class AttributePopUp : DropdownPopUp
    {
        public Toggle isArray;
        public TMP_Text confirm;
        private string _formerAttributeName;

        public override void ActivateCreation(TMP_Text classTxt)
        {
            base.ActivateCreation(classTxt);
            confirm.text = "Add";
        }


        public void ActivateCreation(TMP_Text classTxt, TMP_Text attributeTxt)
        {
            ActivateCreation(classTxt);
            var text = attributeTxt.text.Split(": ");
            var formerName = text[0];

            var attributeType = text[1];
            var formerArray = attributeType.Contains("[]");
            attributeType = Regex.Replace(attributeType, "[\\[\\]\\n]", "");
            var formerType = attributeType;

            isArray.isOn = formerArray;
            inp.text = formerName;
            dropdown.value = dropdown.options.FindIndex(x => x.text == formerType);

            _formerAttributeName = formerName;

            confirm.text = "Edit";
        }

        public override void Confirmation()
        {
            if (inp.text == "")
            {
                Deactivate();
                return;
            }

            var newAttribute = new Attribute
            {
                Name = inp.text,
                Type = (isArray.isOn ? "[]" : "") + dropdown.options[dropdown.value].text
            };
            if (_formerAttributeName == null)
            {
                MainEditor.AddAttribute(className.text, newAttribute, MainEditor.Source.Editor);
            }
            else
            {
                MainEditor.UpdateAttribute(className.text, _formerAttributeName, newAttribute);
                _formerAttributeName = null;
            }

            Deactivate();
        }

        public override void Deactivate()
        {
            base.Deactivate();
            isArray.isOn = false;
        }
    }
}
