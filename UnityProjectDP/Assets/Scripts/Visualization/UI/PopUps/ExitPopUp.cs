using UnityEngine;
using UnityEngine.UI;

namespace Visualization.UI.PopUps
{
    public class ExitPopUp : AbstractPopUp
    {
        public Button confirmButton;
        private GameObject _otherActivePopUp;
        private bool _isAnimationPaused;
        private bool _isManagerPlaying;
        private bool _isManagerCreating;

        private void Awake()
        {
            confirmButton.onClick.AddListener(Application.Quit);
        }

        public override void OnEnable()
        {
            base.OnEnable();
            var canvas = GameObject.Find("Canvas").transform;
            foreach (Transform child in canvas.Find("PopUps").transform)
            {
                if (child.gameObject.activeSelf && child != confirmButton.transform.parent)
                {
                    _otherActivePopUp = child.gameObject;
                }
            }

            if (_otherActivePopUp != null)
                _otherActivePopUp.SetActive(false);

            _isManagerPlaying = MenuManager.Instance.isPlaying;
            _isManagerCreating = MenuManager.Instance.isCreating;

            MenuManager.Instance.isPlaying = false;
            MenuManager.Instance.isCreating = false;

            if (!Animation.Animation.Instance.AnimationIsRunning) return;

            Animation.Animation.Instance.Pause();
            _isAnimationPaused = true;
        }

        public override void OnDisable()
        {
            base.OnDisable();

            MenuManager.Instance.isPlaying = _isManagerPlaying;
            MenuManager.Instance.isCreating = _isManagerCreating;

            if (_otherActivePopUp != null)
                _otherActivePopUp.SetActive(true);

            if (_isAnimationPaused)
                Animation.Animation.Instance.Pause();

            _otherActivePopUp = null;
            _isAnimationPaused = false;
            _isManagerPlaying = false;
            _isManagerCreating = false;
        }
    }
}
