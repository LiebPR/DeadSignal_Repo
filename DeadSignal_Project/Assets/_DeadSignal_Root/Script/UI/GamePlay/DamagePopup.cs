using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour, IReusable
{
    [Tooltip("Velocidad a la que sube hacia arriba")]
    [SerializeField] float upSpeed = 1.5f;
    [Tooltip("Tiempo de vida del Popup.")]
    [SerializeField] float duration = 0.8f;
    [Tooltip("Tamaño random para el Popup.")]
    [SerializeField] Vector2 randomOffset = new Vector2(0.3f, 0.3f);

    TextMeshPro textMesh;
    float timer;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public void SetDamage(float amount)
    {
        textMesh.text = amount.ToString("0");
        textMesh.alpha = 1f;          // Reinicia transparencia
        transform.localScale = Vector3.one; // Reinicia escala
    }

    public void OnObjectReuse()
    {
        // Pequeño offset aleatorio para que no se vea siempre en la misma posición
        transform.position += new Vector3(
            Random.Range(-randomOffset.x, randomOffset.x),
            Random.Range(-randomOffset.y, randomOffset.y),
            0f
        );
        timer = 0f;
    }

    private void Update()
    {
        // Movimiento hacia arriba
        transform.position += Vector3.up * upSpeed * Time.deltaTime;

        // Animación de fade y escala
        float progress = timer / duration;
        transform.localScale = Vector3.one * (1f + progress); // Crece ligeramente
        textMesh.alpha = 1f - progress;                       // Desaparece

        timer += Time.deltaTime;
        if (timer >= duration)
            gameObject.SetActive(false); // Se recicla automáticamente
    }
}
