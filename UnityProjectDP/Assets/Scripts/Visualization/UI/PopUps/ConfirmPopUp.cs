using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AnimArch.Visualization.UI
{
    public class ConfirmPopUp : AbstractPopUp
    {
        public Button confirmButton;
        public Button cancelButton;
        public Button exitButton;

        public void ActivateCreation(UnityAction call)
        {
            base.ActivateCreation();
            confirmButton.onClick.AddListener(call);
        }

        public override void Deactivate()
        {
            base.Deactivate();
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(Deactivate);
            
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(Deactivate);
            
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(Deactivate);
        }
    }
}
