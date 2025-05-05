using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanelFader : MonoBehaviour
{
    [SerializeField] private Image panelImage;         // Привязка панели с Image
    [SerializeField] private float fadeDuration = 0.5f;  // Время перехода
    [SerializeField] private float fadeInTargetAlpha = 0.65f; // Насколько затемнять
    [SerializeField] private float fadeOutTargetAlpha = 0f;  // Насколько убирать затемнение

    public void FadeIn()
    {
        if (panelImage == null)
        {
            Debug.LogError("Panel Image is not assigned!");
            return;
        }

        StartCoroutine(FadeToAlpha(fadeInTargetAlpha));
    }

    public void FadeOut()
    {
        if (panelImage == null)
        {
            Debug.LogError("Panel Image is not assigned!");
            return;
        }

        StartCoroutine(FadeToAlpha(fadeOutTargetAlpha));
    }

    private IEnumerator FadeToAlpha(float targetAlpha)
    {
        Color currentColor = panelImage.color;
        float startAlpha = currentColor.a;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            panelImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
            yield return null;
        }

        panelImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);
    }
}
