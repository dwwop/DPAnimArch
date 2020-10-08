using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            GetComponentInChildren<Image>().color = defaultColor;
        }
	}

}
