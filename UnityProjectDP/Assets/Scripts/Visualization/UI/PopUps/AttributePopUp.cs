using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using AnimArch.Visualization.Diagrams;
using Attribute = AnimArch.Visualization.Diagrams.Attribute;

namespace AnimArch.Visualization.UI
{
    public class AttributePopUp : AbstractPopUp
    {
        public TMP_Dropdown dropdown;
        public Toggle isArray;
        public TMP_Text confirm;
        private readonly HashSet<TMP_Dropdown.OptionData> _variableData = new();
        private string _formerAttributeName;

        private void UpdateDropdown()
        {
            var classNames = DiagramPool.Instance.ClassDiagram.GetClassList().Select(x => x.Name);

            dropdown.options.RemoveAll(x => _variableData.Contains(x));
            _variableData.Clear();
            _variableData.UnionWith(classNames.Select(x => new TMP_Dropdown.OptionData(x)));
            dropdown.options.AddRange(_variableData);
        }

        public override void ActivateCreation(TMP_Text classTxt)
        {
            base.ActivateCreation(classTxt);
            UpdateDropdown();
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
                if (!ClassEditor.AddAttribute(className.text, newAttribute))
                    return;
                ClassEditor.AddAttribute(className.text, newAttribute, false);
            }
            else
            {
                if (!ClassEditor.UpdateAttribute(className.text, _formerAttributeName, newAttribute))
                    return;
                _formerAttributeName = null;
            }

            Deactivate();
        }

        public override void Deactivate()
        {
            base.Deactivate();
            isArray.isOn = false;
            dropdown.value = 0;
        }
    }
}