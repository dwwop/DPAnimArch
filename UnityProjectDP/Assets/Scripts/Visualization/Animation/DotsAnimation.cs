using System.Linq;
using TMPro;
using UnityEngine;

namespace Visualization.Animation
{
    public class DotsAnimation : MonoBehaviour
    {
        public float speed = 0.33f;
        private float timeWaited = 0f;
        public int maxDots = 6;
        private int dots = 1;
        TMP_Text affectedText;
        public string currentText = "Select source class\n for call function\ndirectly in diagram\n.";
        // Update is called once per frame
        private void Awake()
        {
            affectedText = GetComponent<TMP_Text>();
            affectedText.text = currentText;
        }
        void Update()
        {
            timeWaited += Time.deltaTime;
            if (timeWaited > speed)
            {
                timeWaited = 0;
                if (dots < maxDots)
                {
                    affectedText.text = currentText + string.Concat(Enumerable.Repeat(".", dots));
                    dots++;
                }
                else
                {
                    affectedText.text = currentText;
                    dots = 1;
                }
            }
        }
    }
}