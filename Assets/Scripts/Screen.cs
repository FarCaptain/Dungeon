using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Screen : MonoBehaviour
{
    Image image;
    private void OnEnable()
    {
        image = GetComponent<Image>();
        Color c = image.color;
        image.color = new Color(c.r, c.g, c.b, 0f);

        InvokeRepeating("fadeScreen", 1, 0.2f);
    }

    void fadeScreen()
    {
        Color c = image.color;
        float alpha = Mathf.Clamp01(c.a + 0.1f);
        image.color = new Color(c.r, c.g, c.b, alpha);

        if (alpha == 1f)
            CancelInvoke();
    }
}
