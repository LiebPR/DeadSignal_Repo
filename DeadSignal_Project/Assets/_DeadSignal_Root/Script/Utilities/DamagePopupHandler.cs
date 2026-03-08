using UnityEngine;

public class DamagePopupHandler : MonoBehaviour
{
    [SerializeField] string poolTag = "DamagePopup";

    HealthSystem healthSystem;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        if (healthSystem != null)
            healthSystem.OnHit += ShowDamage;
    }

    private void OnDestroy()
    {
        if (healthSystem != null)
            healthSystem.OnHit -= ShowDamage;
    }

    void ShowDamage(float amount)
    {
        Vector3 spawnPos = transform.position + Vector3.up * 1f; // Ajusta altura sobre el sprite
        GameObject popup = PoolManager.Instance.SpawnFromPool(poolTag, spawnPos, Quaternion.identity);
        popup.GetComponent<DamagePopup>().SetDamage(amount);
    }
}
