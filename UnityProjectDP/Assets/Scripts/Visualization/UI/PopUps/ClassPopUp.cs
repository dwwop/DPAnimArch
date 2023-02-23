using System;
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

                if (DiagramPool.Instance.ClassDiagram.FindClassByName(newClass.Name) != null)
                {
                    errorMessage.gameObject.SetActive(true);
                    return;
                }

                MainEditor.CreateNode(newClass, MainEditor.Source.Editor);

                _id++;
            }
            else
            {
                if (DiagramPool.Instance.ClassDiagram.FindClassByName(inp.text) != null)
                {
                    errorMessage.gameObject.SetActive(true);
                    return;
                }

                MainEditor.UpdateNodeName(className.text, inp.text, false);
                _formerName = null;
            }

            Deactivate();
        }
    }
}
