using TMPro;

namespace AnimArch.Visualization.UI
{
    public class ParameterPopUp : TypePopUp
    {
        public TMP_Text confirm;
        private string _formerParam;

        public void ActivateCreation()
        {
            panel.SetActive(true);
            confirm.text = "Add";
            UpdateDropdown();
        }

        public override void ActivateCreation(TMP_Text parameterTxt)
        {
            ActivateCreation();
            var par = parameterTxt.text.Split(" ");
            inp.text = par[1];
            
            SetType(par[0]);
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

            var parameter = GetType() + " " + inp.text.Replace(" ", "_");
            if (_formerParam == null)
            {
                UIEditorManager.Instance.methodPopUp.AddArg(parameter);
            }
            else
            {
                UIEditorManager.Instance.methodPopUp.EditArg(_formerParam, parameter);
                _formerParam = null;
            }

            Deactivate();
        }
    }
}
