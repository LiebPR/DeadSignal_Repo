using System.Collections;
using UnityEngine;

public class PoolObjectTimer : MonoBehaviour, IReusable
{
    [Tooltip("Tiempo total antes de reiniciar el objeto")]
    [SerializeField] float lifeTime = 20f;
    [Tooltip("Tiempo de parpadeo antes de reiniciar")]
    [SerializeField] float blinkDuration = 3f;
    [Tooltip("Número de parpadeos en la duración de parpadeo.")]
    [SerializeField] int blinkCount = 5;
    [Tooltip("Alpha mínimo durante el parpadeo")]
    [SerializeField] float minAlpha = 0.2f;

    SpriteRenderer spriteRenderer;
    Coroutine lifeCoroutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        StartCoroutine(LifeCycle());
    }

    IEnumerator LifeCycle()
    {
        //Esperamos el tiempo total menos el tiempo de parpadeo
        yield return new WaitForSeconds(lifeTime - blinkDuration);

        //Parpadeo
        yield return StartCoroutine(SmoothBlink(blinkDuration, blinkCount));

        //Reiniciar objeto al pool (desactivar)
        gameObject.SetActive(false);
    }

    IEnumerator SmoothBlink(float duration, int count)
    {
        float elapsed = 0f;
        float totalCycles = count * 2; //ida y vuelt, por ciclo
        while (elapsed < duration)
        {
            float t = Mathf.PingPong(elapsed / duration * totalCycles, 1f);
            float alpha = Mathf.Lerp(minAlpha, 1f, t);
            SetAlpha(alpha);

            elapsed += Time.deltaTime;
            yield return null;
        }

        //Asegura que no se vea al termina Blink
        SetAlpha(0f);
    }

    private void SetAlpha(float alpha)
    {
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = alpha;
            spriteRenderer.color = c;
        }
    }

    #region IReusable Method
    public void OnObjectReuse()
    {
        //Reiniciar Alpha
        SetAlpha(1f);

        //Detener coroutine anterior si existía
        if (lifeCoroutine != null)
            StopCoroutine(lifeCoroutine);
    }
    #endregion
}
