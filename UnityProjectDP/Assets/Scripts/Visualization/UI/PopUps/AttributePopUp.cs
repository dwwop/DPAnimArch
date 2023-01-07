using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly HashSet<TMP_Dropdown.OptionData> _variableData = new();

        private void UpdateDropdown()
        {
            var classNames = DiagramPool.Instance.ClassDiagram.GetClassList().Select(x => x.Name);

            dropdown.options.RemoveAll(x => _variableData.Contains(x));
            _variableData.Clear();
            _variableData.UnionWith(classNames.Select(x => new TMP_Dropdown.OptionData(x)));
            dropdown.options.AddRange(_variableData);
        }


        public void ActivateCreation(TMP_Text classTxt, TMP_Text atrTxt)
        {
            ActivateCreation(classTxt);
            UpdateDropdown();
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
                Type = (isArray.isOn ? "[]" : "") + dropdown.options[dropdown.value].text
            };
            if (ClassEditor.AddAttribute(className.text, attribute))
            {
            }

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