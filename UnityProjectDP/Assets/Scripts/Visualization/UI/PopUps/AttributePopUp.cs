using AnimArch.Visualization.Diagrams;
using TMPro;

namespace AnimArch.Visualization.UI
{
    public class AttributePopUp : AbstractTypePopUp
    {
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
            SetType(attributeType);

            inp.text = formerName;
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
                Type = GetType()
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
    }
}
