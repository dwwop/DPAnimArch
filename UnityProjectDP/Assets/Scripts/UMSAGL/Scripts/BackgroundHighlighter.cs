using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Animation = Visualization.Animation.Animation;

namespace UMSAGL.Scripts
{
	public class BackgroundHighlighter : MonoBehaviour {

		private Color defaultColor;
		private int highlight = 0;
		private void Awake()
		{
			defaultColor = GetComponentInChildren<Image>().color;
			highlight = 0;
		}

		public void HighlightOutline()
		{
			GetComponentInChildren<Outline>().enabled = true;
		}

		public void HighlightBackground()
		{
			if (highlight == 0)
			{
				RectTransform rc = GetComponent<RectTransform>();
				rc.DOScaleX(1.2f, 0.5f);
				rc.DOScaleY(1.2f, 0.5f);
				GetComponentInChildren<Image>().color = Animation.Instance.classColor;
			}
			highlight++;
		}

		public void UnhighlightOutline()
		{
			GetComponentInChildren<Outline>().enabled = false;
		}

		public void UnhighlightBackground()
		{
			if (highlight>0)
				highlight--;
			if (highlight == 0)
			{
				RectTransform rc = GetComponent<RectTransform>();
				rc.DOScaleX(1f, 0.5f);
				rc.DOScaleY(1f, 0.5f);
				GetComponentInChildren<Image>().color = defaultColor;
			}
		}

	}
}
