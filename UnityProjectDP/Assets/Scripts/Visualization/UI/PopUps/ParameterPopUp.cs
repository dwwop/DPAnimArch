using TMPro;

namespace Visualization.UI.PopUps
{
    public class ParameterPopUp : AbstractTypePopUp
    {
        private const string ErrorParameterNameExists = "Parameter with the same name already exists";
        
        public TMP_Text confirm;
        private string _formerParam;

        public override void ActivateCreation()
        {
            base.ActivateCreation();
            confirm.text = "Add";
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
                DisplayError(ErrorEmptyName);
                return;
            }

            var type = GetType();
            if (type == null)
                return;
            
            var parameter = type + " " + inp.text.Replace(" ", "_");
            if (_formerParam == null)
            {
                if (UIEditorManager.Instance.methodPopUp.ArgExists(parameter))
                {
                    DisplayError(ErrorParameterNameExists);
                    return;
                }

                UIEditorManager.Instance.methodPopUp.AddArg(parameter);
            }
            else
            {
                if (UIEditorManager.Instance.methodPopUp.ArgExists(parameter))
                {
                    DisplayError(ErrorParameterNameExists);
                    return;
                }

                UIEditorManager.Instance.methodPopUp.EditArg(_formerParam, parameter);
            }

            Deactivate();
        }

        public override void Deactivate()
        {
            base.Deactivate();
            UIEditorManager.Instance.methodPopUp.gameObject.SetActive(true);
            _formerParam = null;
        }
    }
}
