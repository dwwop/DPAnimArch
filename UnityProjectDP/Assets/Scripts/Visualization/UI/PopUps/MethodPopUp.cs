using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using AnimArch.Visualization.Diagrams;
using Microsoft.Msagl.Core.DataStructures;

namespace AnimArch.Visualization.UI
{
    public class MethodPopUp : AbstractPopUp
    {
        public TMP_Dropdown dropdown;
        private TMP_Text _methodText;
        [SerializeField] Transform parameterContent;
        private List<string> _parameters = new();
        private readonly HashSet<TMP_Dropdown.OptionData> _variableData = new();
        
        private void UpdateDropdown()
        {
            var classNames = DiagramPool.Instance.ClassDiagram.GetClassList().Select(x => x.Name);
            
            dropdown.options.RemoveAll(x => _variableData.Contains(x));
            _variableData.Clear();
            _variableData.UnionWith(classNames.Select(x => new TMP_Dropdown.OptionData(x)));
            dropdown.options.AddRange(_variableData);
        }


        public void ActivateCreation(TMP_Text classTxt, TMP_Text mtdTxt)
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

            var method = new Method
            {
                Name = inp.text,
                ReturnValue = dropdown.options[dropdown.value].text,
                arguments = _parameters
            };
            ClassEditor.AddMethod(className.text, method, ClassEditor.Source.editor);

            Deactivate();
        }

        public override void Deactivate()
        {
            base.Deactivate();
            _parameters = new List<string>();
            parameterContent.GetComponentInChildren<TMP_Text>().text = "";
        }

        public void AddArg(string parameter)
        {
            _parameters.Add(parameter);
            parameterContent.GetComponentInChildren<TMP_Text>().text =
                parameterContent.GetComponentInChildren<TMP_Text>().text == ""
                    ? parameter
                    : parameterContent.GetComponentInChildren<TMP_Text>().text + ",\n" + parameter;
        }
    }
}