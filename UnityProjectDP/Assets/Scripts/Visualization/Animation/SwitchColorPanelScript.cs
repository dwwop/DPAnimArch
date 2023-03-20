using UnityEngine;
using UnityEngine.UI;

namespace Visualization.Animation
{
    public class SwitchColorPanelScript : MonoBehaviour
    {
        [SerializeField]
        private Image[] images;

        public void SetAllImagesUnOutlined()
        {
            foreach (Image img in images)
            {
                img.GetComponent<Outline>().enabled = false;
            }
        }

        public void OnImageClicked(Image clickedImage)
        {
            int imageIndex = System.Array.IndexOf(images, clickedImage);

            if (imageIndex == -1)
                return;

            SetAllImagesUnOutlined();

            clickedImage.GetComponent<Outline>().enabled = true;
        }
    }
}
