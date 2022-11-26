using UnityEngine;
using TMPro;
using AnimArch.Visualization.Diagrams;
using OALProgramControl;

namespace AnimArch.Visualization.UI
{
    public class MethodPopUp : AbstractPopUp
    {
        public TMP_Dropdown dropdown;
        private TMP_Text _methodText;

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

            //TODO: Method args
            var method = new Method
            {
                Name = inp.text,
                ReturnValue = dropdown.options[dropdown.value].text
            };
            ClassEditor.AddMethod(ClassTxt.text, method, ClassEditor.Source.editor);

            Deactivate();
        }
    }
}