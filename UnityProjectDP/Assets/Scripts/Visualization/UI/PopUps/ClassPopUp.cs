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

            var inpClassName = inp.text.Replace(" ", "_");
            if (_formerName == null)
            {
                var newClass = new Class(inpClassName, DiagramPool.Instance.ClassDiagram.NextClassId());

                if (DiagramPool.Instance.ClassDiagram.FindClassByName(newClass.Name) != null)
                {
                    errorMessage.gameObject.SetActive(true);
                    return;
                }

                UIEditorManager.Instance.mainEditor.CreateNode(newClass);
            }
            else
            {
                if (DiagramPool.Instance.ClassDiagram.FindClassByName(inpClassName) != null)
                {
                    errorMessage.gameObject.SetActive(true);
                    return;
                }

                UIEditorManager.Instance.mainEditor.UpdateNodeName(className.text, inpClassName);
                _formerName = null;
            }

            Deactivate();
        }
    }
}
