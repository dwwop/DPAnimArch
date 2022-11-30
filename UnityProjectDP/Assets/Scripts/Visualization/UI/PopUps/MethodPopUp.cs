using System.Collections.Generic;
using UnityEngine;
using TMPro;
using AnimArch.Visualization.Diagrams;

namespace AnimArch.Visualization.UI
{
    public class MethodPopUp : AbstractPopUp
    {
        public TMP_Dropdown dropdown;
        private TMP_Text _methodText;
        [SerializeField] Transform parameterContent;
        private List<string> _parameters = new();

        public void ActivateCreation(TMP_Text classTxt, TMP_Text mtdTxt)
        {
            ActivateCreation(classTxt);
            _methodText = mtdTxt;
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
            ClassEditor.AddMethod(ClassTxt.text, method, ClassEditor.Source.editor);

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