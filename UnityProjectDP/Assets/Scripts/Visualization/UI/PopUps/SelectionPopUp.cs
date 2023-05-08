using TMPro;

namespace Visualization.UI.PopUps
{
    public class SelectionPopUp : AbstractPopUp
    {
        public TMP_Dropdown dropdown;

        public override void Confirmation()
        {
            UIEditorManager.Instance.StartSelection(dropdown.options[dropdown.value].text);
            dropdown.SetValueWithoutNotify(0);
            Deactivate();
            UIEditorManager.SetDiagramButtonsActive(false);
        }
    }
}
