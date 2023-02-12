using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AnimArch.Visualization.Diagrams;
using TMPro;
using UnityEngine.UI;

namespace AnimArch.Visualization.UI
{
    public abstract class AbstractTypePopUp : AbstractClassPopUp
    {
        private const string CUSTOM = "custom";
        public TMP_Dropdown dropdown;
        public TMP_Text customType;
        public TMP_InputField customTypeField;
        public Toggle isArray;
        private readonly HashSet<TMP_Dropdown.OptionData> _variableData = new();

        private void Awake()
        {
            dropdown.onValueChanged.AddListener(delegate
            {
                if (dropdown.options[dropdown.value].text == CUSTOM)
                {
                    customType.transform.gameObject.SetActive(true);
                    customTypeField.transform.gameObject.SetActive(true);
                }
                else
                {
                    customType.transform.gameObject.SetActive(false);
                    customTypeField.transform.gameObject.SetActive(false);
                    customTypeField.text = "";
                }
            });
        }

        protected void SetType(string attributeType)
        {
            var formerArray = attributeType.Contains("[]");
            attributeType = Regex.Replace(attributeType, "[\\[\\]\\n]", "");
            isArray.isOn = formerArray;

            var typeIndex = dropdown.options.FindIndex(x => x.text == attributeType);
            if (typeIndex == -1)
            {
                dropdown.value = dropdown.options.FindIndex(x => x.text == CUSTOM);
                customTypeField.text = attributeType;
            }
            else
            {
                dropdown.value = typeIndex;
            }
        }

        protected new string GetType()
        {
            if (dropdown.options[dropdown.value].text == CUSTOM)
                return (isArray.isOn ? "[]" : "") + customTypeField.text.Replace(" ", "_");

            return (isArray.isOn ? "[]" : "") + dropdown.options[dropdown.value].text;
        }

        private void UpdateDropdown()
        {
            var classNames = DiagramPool.Instance.ClassDiagram.GetClassList().Select(x => x.Name);

            dropdown.options.RemoveAll(x => _variableData.Contains(x));
            _variableData.Clear();
            _variableData.UnionWith(classNames.Select(x => new TMP_Dropdown.OptionData(x)));
            dropdown.options.AddRange(_variableData);
        }

        public override void ActivateCreation()
        {
            base.ActivateCreation();
            UpdateDropdown();
        }

        public override void Deactivate()
        {
            base.Deactivate();
            dropdown.value = 0;
            customTypeField.text = "";
            isArray.isOn = false;
        }
    }
}
