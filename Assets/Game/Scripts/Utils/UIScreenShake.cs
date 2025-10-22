using UnityEngine;

public class UIScreenShake : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float amplitudeGain = 10f;   // Intensit� du shake
    [SerializeField] private float frequencyGain = 25f;   // Fr�quence du shake

    private RectTransform rectTransform;
    private Vector3 originalPosition;
    private float shakeTimer = 0f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    private void Update()
    {
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.unscaledDeltaTime; // pour que �a marche m�me en pause UI
            float progress = 1f - (shakeTimer / shakeDuration);

            // Bruit pseudo-al�atoire oscillant (perlin pour un effet fluide)
            float offsetX = (Mathf.PerlinNoise(Time.time * frequencyGain, 0f) - 0.5f) * 2f * amplitudeGain;
            float offsetY = (Mathf.PerlinNoise(0f, Time.time * frequencyGain) - 0.5f) * 2f * amplitudeGain;

            // On r�duit progressivement l�amplitude selon la progression
            float fade = Mathf.Lerp(1f, 0f, progress);
            rectTransform.anchoredPosition = originalPosition + new Vector3(offsetX, offsetY, 0f) * fade;

            if (shakeTimer <= 0f)
                rectTransform.anchoredPosition = originalPosition;
        }
    }

    public void Shake()
    {
        shakeTimer = shakeDuration;
    }
}
