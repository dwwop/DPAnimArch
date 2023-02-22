using AnimArch.Visualization.Diagrams;
using TMPro;

namespace AnimArch.Visualization.UI
{
    public class ClassPopUp : AbstractClassPopUp
    {
        public TMP_Text confirm;
        private string _formerName;

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
                var className = inp.text.Replace(" ", "_");
                var newClass = new Class(className, DiagramPool.Instance.ClassDiagram.NextClassId());
                UIEditorManager.Instance.mainEditor.CreateNode(newClass);
            }
            else
            {
                UIEditorManager.Instance.mainEditor.UpdateNodeName(className.text, inp.text);
                _formerName = null;
            }

            Deactivate();
        }
    }
}
