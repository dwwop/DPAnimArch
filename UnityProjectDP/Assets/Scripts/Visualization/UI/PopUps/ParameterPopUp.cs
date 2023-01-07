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
        }

        public override void Confirmation()
        {
            if (inp.text == "")
            {
                Deactivate();
                return;
            }

            var param = inp.text.Replace(" ", "_") + " " + dropdown.options[dropdown.value].text;
            ClassEditor.Instance.mtdPopUp.AddArg(param);

            Deactivate();
        }
    }
}