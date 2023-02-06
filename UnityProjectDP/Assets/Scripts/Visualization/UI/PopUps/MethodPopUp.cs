using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using AnimArch.Visualization.Diagrams;

namespace AnimArch.Visualization.UI
{
    public class MethodPopUp : DropdownPopUp
    {
        public TMP_Text confirm;
        [SerializeField] Transform parameterContent;
        private List<string> _parameters = new();
        private string _formerName;
        


        public override void ActivateCreation(TMP_Text classTxt)
        {
            base.ActivateCreation(classTxt);
            confirm.text = "Add";
        }
        
        
        private static Method GetMethodFromString(string str)
        {
            var method = new Method();

            var parts = str.Split(new[] { ": ", "\n" }, StringSplitOptions.None);

            var nameAndArguments = parts[0].Split(new[] { "(", ")" }, StringSplitOptions.None);
            method.Name = nameAndArguments[0];
            method.ReturnValue = parts[1];


            method.arguments = nameAndArguments[1].Split(", ").Where(x => x != "").ToList();

            return method;
        }
        
        
        public void ActivateCreation(TMP_Text classTxt, TMP_Text methodTxt)
        {
            ActivateCreation(classTxt);

            var formerMethod = GetMethodFromString(methodTxt.text);
            inp.text = formerMethod.Name;
            
            dropdown.value = dropdown.options.FindIndex(x => x.text == formerMethod.ReturnValue);
            formerMethod.arguments.ForEach(AddArg);
            _formerName = formerMethod.Name;
            confirm.text = "Edit";
        }


        public override void Confirmation()
        {
            if (inp.text == "")
            {
                Deactivate();
                return;
            }

            var newMethod = new Method
            {
                Name = inp.text,
                ReturnValue = dropdown.options[dropdown.value].text,
                arguments = _parameters
            };
            if (_formerName == null)
            {
                MainEditor.AddMethod(className.text, newMethod, MainEditor.Source.Editor);
            }
            else
            {
                MainEditor.UpdateMethod(className.text, _formerName, newMethod);
                _formerName = null;
            }
            
            Deactivate();
        }

        public override void Deactivate()
        {
            base.Deactivate();
            _parameters = new List<string>();
            parameterContent.DetachChildren();
        }

        public void AddArg(string parameter)
        {
            _parameters.Add(parameter);
            var instance = Instantiate(DiagramPool.Instance.parameterMethodPrefab, parameterContent, false);
            instance.name = parameter;
            instance.transform.Find("ParameterText").GetComponent<TextMeshProUGUI>().text += parameter;
        }

        public void EditArg(string formerParam, string newParam)
        {
            var index = _parameters.FindIndex(x => x == formerParam);
            _parameters[index] = newParam;
            parameterContent.GetComponentsInChildren<ParameterPopUpManager>()
                .First(x => x.parameterTxt.text == formerParam)
                .parameterTxt.text = newParam;

        }
    }
}
