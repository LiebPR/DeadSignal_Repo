using System.Collections;
using TMPro;
using UnityEngine;

public class WaveUIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI waveText;
    [SerializeField] CanvasGroup canvasGroup;

    #region Timing

    [SerializeField] float waitBeforeChange = 0.4f;
    [SerializeField] float waitAfterChange = 0.5f;

    #endregion

    #region Current Wave Drop

    [SerializeField] float currentDigitDropDistance = 80f;
    [SerializeField] float currentDigitDropDuration = 0.5f;
    [SerializeField] float currentDigitDropDelay = 0.15f;

    #endregion

    #region New Wave Drop

    [SerializeField] float newDigitDropDistance = 120f;
    [SerializeField] float newDigitDropDuration = 0.45f;

    #endregion

    #region Impact

    [SerializeField] float impactOffset = 10f;
    [SerializeField] float impactDuration = 0.18f;

    #endregion

    #region Scale Animation

    [SerializeField] float appearScaleMax = 1.25f;
    [SerializeField] float appearScaleDuration = 0.25f;

    [SerializeField] float exitScaleMax = 1.2f;
    [SerializeField] float exitScaleDuration = 0.35f;

    #endregion

    public void ShowWave(int previousWave, int currentWave)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateWave(previousWave, currentWave));
    }

    IEnumerator AnimateWave(int previousWave, int currentWave)
    {
        waveText.text = $"WAVE {previousWave}";
        waveText.ForceMeshUpdate();

        canvasGroup.alpha = 1;

        yield return AnimateAppearScale();

        yield return AnimateCurrentDigitDrop();

        yield return new WaitForSeconds(waitBeforeChange);

        waveText.text = $"WAVE {currentWave}";
        waveText.ForceMeshUpdate();

        yield return AnimateNewDigitDropWithImpact();

        yield return new WaitForSeconds(waitAfterChange);

        yield return AnimateExitScale();
    }

    #region Current Digit Drop

    IEnumerator AnimateCurrentDigitDrop()
    {
        yield return new WaitForSeconds(currentDigitDropDelay);

        TMP_TextInfo textInfo = waveText.textInfo;

        int charIndex = textInfo.characterCount - 1;
        if (!textInfo.characterInfo[charIndex].isVisible)
            yield break;

        int matIdx = textInfo.characterInfo[charIndex].materialReferenceIndex;
        int vertIdx = textInfo.characterInfo[charIndex].vertexIndex;

        Vector3[] vertices = textInfo.meshInfo[matIdx].vertices;
        Color32[] colors = textInfo.meshInfo[matIdx].colors32;

        Vector3[] original = new Vector3[4];

        for (int i = 0; i < 4; i++)
            original[i] = vertices[vertIdx + i];

        float t = 0;

        while (t < currentDigitDropDuration)
        {
            t += Time.deltaTime;
            float p = t / currentDigitDropDuration;

            for (int i = 0; i < 4; i++)
            {
                vertices[vertIdx + i] = original[i] + Vector3.down * currentDigitDropDistance * p;

                Color32 c = colors[vertIdx + i];
                c.a = (byte)(255 * (1 - p));
                colors[vertIdx + i] = c;
            }

            waveText.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            yield return null;
        }
    }

    #endregion

    #region New Digit Drop + Impact

    IEnumerator AnimateNewDigitDropWithImpact()
    {
        TMP_TextInfo textInfo = waveText.textInfo;

        int charIndex = textInfo.characterCount - 1;

        int matIdx = textInfo.characterInfo[charIndex].materialReferenceIndex;
        int vertIdx = textInfo.characterInfo[charIndex].vertexIndex;

        Vector3[] vertices = textInfo.meshInfo[matIdx].vertices;
        Color32[] colors = textInfo.meshInfo[matIdx].colors32;

        Vector3[] original = new Vector3[4];

        for (int i = 0; i < 4; i++)
        {
            original[i] = vertices[vertIdx + i];
            vertices[vertIdx + i] = original[i] + Vector3.up * newDigitDropDistance;

            Color32 c = colors[vertIdx + i];
            c.a = 0;
            colors[vertIdx + i] = c;
        }

        waveText.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

        float t = 0;

        while (t < newDigitDropDuration)
        {
            t += Time.deltaTime;
            float p = t / newDigitDropDuration;

            Vector3 offset = Vector3.up * newDigitDropDistance * (1 - p);

            for (int i = 0; i < 4; i++)
            {
                vertices[vertIdx + i] = original[i] + offset;

                Color32 c = colors[vertIdx + i];
                c.a = (byte)(255 * p);
                colors[vertIdx + i] = c;
            }

            waveText.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            yield return null;
        }

        int charCount = textInfo.characterCount;
        Vector3[] originalAll = new Vector3[charCount * 4];

        for (int c = 0; c < charCount; c++)
        {
            int m = textInfo.characterInfo[c].materialReferenceIndex;
            int v = textInfo.characterInfo[c].vertexIndex;

            Vector3[] verts = textInfo.meshInfo[m].vertices;

            for (int i = 0; i < 4; i++)
                originalAll[c * 4 + i] = verts[v + i];
        }

        float impactTime = 0;

        while (impactTime < impactDuration)
        {
            impactTime += Time.deltaTime;
            float p = Mathf.Sin((impactTime / impactDuration) * Mathf.PI);

            for (int c = 0; c < charCount; c++)
            {
                int m = textInfo.characterInfo[c].materialReferenceIndex;
                int v = textInfo.characterInfo[c].vertexIndex;

                Vector3[] verts = textInfo.meshInfo[m].vertices;

                for (int i = 0; i < 4; i++)
                    verts[v + i] = originalAll[c * 4 + i] + Vector3.down * impactOffset * p;
            }

            waveText.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            yield return null;
        }
    }

    #endregion

    #region Appear Scale

    IEnumerator AnimateAppearScale()
    {
        RectTransform rt = waveText.rectTransform;

        float t = 0;

        while (t < appearScaleDuration)
        {
            t += Time.deltaTime;
            float p = t / appearScaleDuration;

            float scale = Mathf.Lerp(1, appearScaleMax, p);
            rt.localScale = Vector3.one * scale;

            yield return null;
        }

        t = 0;

        while (t < appearScaleDuration)
        {
            t += Time.deltaTime;
            float p = t / appearScaleDuration;

            float scale = Mathf.Lerp(appearScaleMax, 1, p);
            rt.localScale = Vector3.one * scale;

            yield return null;
        }

        rt.localScale = Vector3.one;
    }

    #endregion

    #region Exit Scale

    IEnumerator AnimateExitScale()
    {
        RectTransform rt = waveText.rectTransform;

        float t = 0;

        while (t < exitScaleDuration)
        {
            t += Time.deltaTime;
            float p = t / exitScaleDuration;

            float scale;

            if (p < 0.5f)
                scale = Mathf.Lerp(1, exitScaleMax, p * 2);
            else
                scale = Mathf.Lerp(exitScaleMax, 1, (p - 0.5f) * 2);

            rt.localScale = Vector3.one * scale;

            canvasGroup.alpha = 1 - p;

            yield return null;
        }

        canvasGroup.alpha = 0;
        rt.localScale = Vector3.one;
    }

    #endregion
}