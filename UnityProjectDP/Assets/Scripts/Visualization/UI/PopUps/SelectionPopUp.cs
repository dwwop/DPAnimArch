using TMPro;

namespace AnimArch.Visualization.UI
{
    public class SelectionPopUp : AbstractPopUp
    {
        public TMP_Dropdown dropdown;

        public override void Confirmation()
        {
            UIEditorManager.Instance.StartSelection(dropdown.options[dropdown.value].text);
            Deactivate();
        }
    }
}
