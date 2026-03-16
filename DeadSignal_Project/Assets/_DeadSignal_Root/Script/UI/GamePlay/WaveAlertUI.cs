using UnityEngine;
using TMPro;

public class WaveAlertUI : MonoBehaviour
{
    #region References

    [SerializeField] RectTransform rect;
    [SerializeField] TextMeshProUGUI text;

    #endregion

    #region Settings

    [SerializeField] float appearDuration = 0.5f;
    [SerializeField] float visibleDuration = 1.5f;
    [SerializeField] float disappearDuration = 0.6f;

    [SerializeField] float scalePeak = 1.2f;
    [SerializeField] float rotationRange = 10f;

    #endregion

    #region State

    float timer;
    float randomRotation;
    bool playing;

    Color baseColor;

    #endregion

    void Awake()
    {
        if (!rect)
            rect = GetComponent<RectTransform>();

        if (!text)
            text = GetComponent<TextMeshProUGUI>();

        baseColor = text.color;

        rect.localScale = Vector3.one;

        text.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);
    }

    public void Play(string message)
    {
        text.text = message;

        timer = 0f;
        playing = true;

        randomRotation = Random.Range(-rotationRange, rotationRange);

        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.Euler(0, 0, randomRotation);

        text.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);
    }

    void Update()
    {
        if (!playing) return;

        timer += Time.deltaTime;

        if (timer < appearDuration)
        {
            float t = timer / appearDuration;

            rect.localScale = Vector3.one * Mathf.Lerp(1f, scalePeak, t);

            rect.localRotation =
                Quaternion.Euler(0, 0, Mathf.Lerp(randomRotation, 0f, t));

            float alpha = Mathf.Lerp(0f, 1f, t);

            text.color =
                new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
        }
        else if (timer < appearDuration + visibleDuration)
        {
            rect.localScale =
                Vector3.one * Mathf.Lerp(scalePeak, 1f,
                (timer - appearDuration) / visibleDuration);

            rect.localRotation = Quaternion.identity;
        }
        else if (timer < appearDuration + visibleDuration + disappearDuration)
        {
            float t =
                (timer - appearDuration - visibleDuration) / disappearDuration;

            rect.localScale =
                Vector3.one * Mathf.Lerp(1f, scalePeak, t);

            rect.localRotation =
                Quaternion.Euler(0, 0, Mathf.Lerp(0f, randomRotation, t));

            float alpha = Mathf.Lerp(1f, 0f, t);

            text.color =
                new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
        }
        else
        {
            playing = false;
        }
    }
}