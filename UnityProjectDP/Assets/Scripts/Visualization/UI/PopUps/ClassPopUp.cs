using System;
using TMPro;
using Visualization.ClassDiagram;
using Visualization.ClassDiagram.ClassComponents;

namespace Visualization.UI.PopUps
{
    public class ClassPopUp : AbstractClassPopUp
    {
        public TMP_Text confirm;
        private Class _formerClass;

        public override void ActivateCreation()
        {
            base.ActivateCreation();
            confirm.text = "Add";
        }

        public override void ActivateCreation(TMP_Text classTxt)
        {
            base.ActivateCreation(classTxt);
            inp.text = classTxt.text;
            _formerClass = DiagramPool.Instance.ClassDiagram.FindClassByName(inp.text).ParsedClass;
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
            if (_formerClass == null)
            {
                var newClass = new Class(inpClassName, DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString());

                if (DiagramPool.Instance.ClassDiagram.FindClassByName(newClass.Name) != null)
                {
                    errorMessage.gameObject.SetActive(true);
                    return;
                }

                UIEditorManager.Instance.mainEditor.CreateNode(newClass);
            }
            else
            {
                var classInDiagram = DiagramPool.Instance.ClassDiagram.FindClassByName(inpClassName).ParsedClass;
                if (classInDiagram != null && !_formerClass.Equals(classInDiagram))
                {
                    errorMessage.gameObject.SetActive(true);
                    return;
                }

                UIEditorManager.Instance.mainEditor.UpdateNodeName(className.text, inpClassName);
            }

            Deactivate();
        }

        public override void Deactivate()
        {
            base.Deactivate();
            _formerClass = null;
        }
    }
}
