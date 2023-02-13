using TMPro;

namespace AnimArch.Visualization.UI
{
    public abstract class AbstractClassPopUp : AbstractPopUp
    {
        public TMP_InputField inp;
        protected TMP_Text className;


        public virtual void ActivateCreation(TMP_Text classTxt)
        {
            ActivateCreation();
            className = classTxt;
        }


        public override void Deactivate()
        {
            base.Deactivate();
            inp.text = "";
        }
    }
}
