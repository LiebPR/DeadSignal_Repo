using System.Collections;
using TMPro;
using UnityEngine;

public class WaveUIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI waveText;
    [SerializeField] CanvasGroup canvasGroup;

    [Header("Timing")]
    [SerializeField] float waitInitial = 1f;
    [SerializeField] float dropDuration = 0.6f;
    [SerializeField] float waitAfterDrop = 0.3f;
    [SerializeField] float riseDuration = 0.6f;
    [SerializeField] float popDuration = 0.6f;

    [Header("Movement")]
    [SerializeField] float dropDistance = 60f;

    public void ShowWave(int previousWave, int currentWave)
    {
        StopAllCoroutines();
        StartCoroutine(Animate(previousWave, currentWave));
    }

    IEnumerator Animate(int previousWave, int currentWave)
    {
        waveText.text = $"WAVE {previousWave}";
        canvasGroup.alpha = 1;

        yield return null;
        waveText.ForceMeshUpdate();

        yield return new WaitForSeconds(waitInitial);

        yield return AnimateNumberDrop();

        yield return new WaitForSeconds(waitAfterDrop);

        waveText.text = $"WAVE {currentWave}";
        yield return null;
        waveText.ForceMeshUpdate();

        yield return AnimateNumberRise();

        yield return AnimatePopFade();
    }

    IEnumerator AnimateNumberDrop()
    {
        TMP_TextInfo textInfo = waveText.textInfo;
        int charIndex = textInfo.characterCount - 1;

        if (!textInfo.characterInfo[charIndex].isVisible)
            yield break;

        int materialIndex = textInfo.characterInfo[charIndex].materialReferenceIndex;
        int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;

        Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

        Vector3[] original = new Vector3[4];

        for (int i = 0; i < 4; i++)
            original[i] = vertices[vertexIndex + i];

        float t = 0;

        while (t < dropDuration)
        {
            t += Time.deltaTime;
            float p = EaseOutCubic(t / dropDuration);

            Vector3 offset = Vector3.down * dropDistance * p;

            for (int i = 0; i < 4; i++)
                vertices[vertexIndex + i] = original[i] + offset;

            canvasGroup.alpha = 1 - p;

            waveText.UpdateVertexData();
            yield return null;
        }

        canvasGroup.alpha = 0;
    }

    IEnumerator AnimateNumberRise()
    {
        TMP_TextInfo textInfo = waveText.textInfo;
        int charIndex = textInfo.characterCount - 1;

        if (!textInfo.characterInfo[charIndex].isVisible)
            yield break;

        int materialIndex = textInfo.characterInfo[charIndex].materialReferenceIndex;
        int vertexIndex = textInfo.characterInfo[charIndex].vertexIndex;

        Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

        Vector3[] original = new Vector3[4];

        for (int i = 0; i < 4; i++)
            original[i] = vertices[vertexIndex + i];

        float t = 0;

        while (t < riseDuration)
        {
            t += Time.deltaTime;
            float p = EaseOutCubic(t / riseDuration);

            Vector3 offset = Vector3.up * dropDistance * (1 - p);

            for (int i = 0; i < 4; i++)
                vertices[vertexIndex + i] = original[i] + offset;

            canvasGroup.alpha = p;

            waveText.UpdateVertexData();
            yield return null;
        }

        canvasGroup.alpha = 1;
    }

    IEnumerator AnimatePopFade()
    {
        float t = 0;

        Vector3 big = Vector3.one * 1.25f;

        while (t < popDuration)
        {
            t += Time.deltaTime;

            float p = EaseOutBack(t / popDuration);

            waveText.rectTransform.localScale = Vector3.Lerp(big, Vector3.one, p);
            canvasGroup.alpha = 1 - p;

            yield return null;
        }

        canvasGroup.alpha = 0;
        waveText.rectTransform.localScale = Vector3.one;
    }

    float EaseOutCubic(float x)
    {
        return 1 - Mathf.Pow(1 - x, 3);
    }

    float EaseOutBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;
        return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
    }
}