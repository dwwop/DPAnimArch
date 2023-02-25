using System;
using TMPro;
using UnityEngine;

namespace Visualization.UI.PopUps
{
    public abstract class AbstractClassPopUp : AbstractPopUp
    {
        public TMP_InputField inp;
        protected TMP_Text className;
        public TMP_Text errorMessage;

        protected void Awake()
        {
            inp.onValueChanged.AddListener(delegate(string arg)
            {
                if (string.IsNullOrEmpty(arg))
                    return;
                if (arg.Length == 1 && (char.IsLetter(arg[0]) || arg[0] == '_'))
                    inp.text = arg;
                else if (arg.Length > 1 && char.IsLetterOrDigit(arg[^1]) || arg[^1] == '_')
                    inp.text = arg;
                else
                    inp.text = arg[..^1];
            });
        }

        public virtual void ActivateCreation(TMP_Text classTxt)
        {
            ActivateCreation();
            className = classTxt;
        }


        public override void Deactivate()
        {
            base.Deactivate();
            inp.text = "";
            errorMessage.gameObject.SetActive(false);
        }
    }
}
