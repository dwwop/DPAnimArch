using AnimArch.Visualization.Diagrams;
using TMPro;

namespace AnimArch.Visualization.UI
{
    public class ClassPopUp : AbstractClassPopUp
    {
        public TMP_Text confirm;
        private string _formerName;
        private int _id;

        public override void ActivateCreation()
        {
            base.ActivateCreation();
            confirm.text = "Add";
        }

        public override void ActivateCreation(TMP_Text classTxt)
        {
            base.ActivateCreation(classTxt);
            inp.text = classTxt.text;
            _formerName = classTxt.text;
            confirm.text = "Edit";
        }

        public override void Confirmation()
        {
            if (inp.text == "")
            {
                Deactivate();
                return;
            }

            if (_formerName == null)
            {
                var newClass = new Class(inp.text, _id.ToString());
                MainEditor.CreateNode(newClass, MainEditor.Source.Editor);
                _id++;
            }
            else
            {
                MainEditor.UpdateNodeName(className.text, inp.text, false);
                _formerName = null;
            }

            Deactivate();
        }
    }
}
