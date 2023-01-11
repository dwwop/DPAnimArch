using System.Collections.Generic;
using System.Linq;
using AnimArch.Visualization.Diagrams;
using OALProgramControl;
using TMPro;
using UnityEngine.UI;

namespace AnimArch.Visualization.UI
{
    public class ParameterPopUp : AbstractPopUp
    {
        public TMP_Dropdown dropdown;
        private readonly HashSet<TMP_Dropdown.OptionData> _variableData = new();
        public TMP_Text confirm;
        private string _formerParam;
        
        private void UpdateDropdown()
        {
            var classNames = DiagramPool.Instance.ClassDiagram.GetClassList().Select(x => x.Name);
            
            dropdown.options.RemoveAll(x => _variableData.Contains(x));
            _variableData.Clear();
            _variableData.UnionWith(classNames.Select(x => new TMP_Dropdown.OptionData(x)));
            dropdown.options.AddRange(_variableData);
        }


        public void ActivateCreation()
        {
            panel.SetActive(true);
            UpdateDropdown();
            confirm.text = "Add";
        }

        public override void ActivateCreation(TMP_Text parameterTxt)
        {
            ActivateCreation();
            var par = parameterTxt.text.Split(" ");
            inp.text = par[1];
            
            dropdown.value = dropdown.options.FindIndex(x => x.text == par[0]);
            confirm.text = "Edit";
            _formerParam = parameterTxt.text;
        }
        public override void Confirmation()
        {
            if (inp.text == "")
            {
                Deactivate();
                return;
            }

            var param = dropdown.options[dropdown.value].text + " " + inp.text.Replace(" ", "_");
            if (_formerParam == null)
                ClassEditor.Instance.mtdPopUp.AddArg(param);
            else
            {
                ClassEditor.Instance.mtdPopUp.EditArg(_formerParam, param);
                _formerParam = null;
            }
            Deactivate();
        }
    }
}