using System;
using TMPro;
using Visualization.ClassDiagram;
using Attribute = Visualization.ClassDiagram.ClassComponents.Attribute;

namespace Visualization.UI.PopUps
{
    public class AttributePopUp : AbstractTypePopUp
    {
        private const string ErrorAttributeNameExists = "Attribute with the same name already exists";
        
        public TMP_Text confirm;
        private Attribute _formerAttribute;

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
            _formerAttribute = DiagramPool.Instance.ClassDiagram.FindAttributeByName(className.text, formerName);

            confirm.text = "Edit";
        }

        public override void Confirmation()
        {
            if (inp.text == "")
            {
                DisplayError(ErrorEmptyName);
                return;
            }

            var type = GetType();
            if (type == null)
                return;
            var newAttribute = new Attribute
            {
                Name = inp.text,
                Type = type
            };
            if (_formerAttribute == null)
            {
                if (DiagramPool.Instance.ClassDiagram.FindAttributeByName(className.text, newAttribute.Name) != null)
                {
                    DisplayError(ErrorAttributeNameExists);
                    return;
                }

                newAttribute.Id = Guid.NewGuid().ToString();
                UIEditorManager.Instance.mainEditor.AddAttribute(className.text, newAttribute);
            }
            else
            {
                var attributeInDiagram =
                    DiagramPool.Instance.ClassDiagram.FindAttributeByName(className.text, newAttribute.Name);
                if (attributeInDiagram != null && !_formerAttribute.Equals(attributeInDiagram))
                {
                    DisplayError(ErrorAttributeNameExists);
                    return;
                }

                newAttribute.Id = _formerAttribute.Id;
                UIEditorManager.Instance.mainEditor.UpdateAttribute(className.text, _formerAttribute.Name, newAttribute);
            }

            Deactivate();
        }

        public override void Deactivate()
        {
            base.Deactivate();
            _formerAttribute = null;
        }
    }
}
