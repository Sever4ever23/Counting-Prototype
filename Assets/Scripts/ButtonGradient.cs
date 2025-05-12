using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PulsatingRedTextWithDelay : MonoBehaviour
{
    public float fadeToRedTime = 0.8f;    // Время до полного красного
    public float redStayTime = 2.5f;      // Время задержки на красном
    public float fadeToBlackTime = 0.8f;  // Время до полного черного

    private TMP_Text buttonText;
    private string baseText = "+1 жизнь за рекламу";
    private float timer = 0f;
    private enum FadeState { ToRed, StayRed, ToBlack }
    private FadeState currentState = FadeState.ToRed;
    private Color blackColor = Color.black;
    private Color redColor = new Color(1f, 0f, 0f); // Чистый красный

    void Start()
    {
        buttonText = GetComponentInChildren<TMP_Text>();
        if (buttonText != null)
        {
            baseText = buttonText.text;
        }
    }

    void Update()
    {
        if (buttonText == null) return;

        timer += Time.deltaTime;

        switch (currentState)
        {
            case FadeState.ToRed:
                // Плавный переход от черного к красному
                float progressToRed = Mathf.Clamp01(timer / fadeToRedTime);
                UpdateTextColor(Color.Lerp(blackColor, redColor, progressToRed));

                if (progressToRed >= 1f)
                {
                    currentState = FadeState.StayRed;
                    timer = 0f;
                }
                break;

            case FadeState.StayRed:
                // Остаемся на красном
                UpdateTextColor(redColor);

                if (timer >= redStayTime)
                {
                    currentState = FadeState.ToBlack;
                    timer = 0f;
                }
                break;

            case FadeState.ToBlack:
                // Плавный переход от красного к черному
                float progressToBlack = Mathf.Clamp01(timer / fadeToBlackTime);
                UpdateTextColor(Color.Lerp(redColor, blackColor, progressToBlack));

                if (progressToBlack >= 1f)
                {
                    currentState = FadeState.ToRed;
                    timer = 0f;
                }
                break;
        }
    }

    void UpdateTextColor(Color color)
    {
        string coloredWord = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>+1 жизнь</color>";
        buttonText.text = baseText.Replace("+1 жизнь", coloredWord);
    }

    // Методы для ручного управления
    public void StartPulsation()
    {
        enabled = true;
        currentState = FadeState.ToRed;
        timer = 0f;
    }

    public void StopPulsation() => enabled = false;
}