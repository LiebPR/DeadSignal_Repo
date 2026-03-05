using TMPro;
using UnityEngine;

public class EnemyKillCounter : MonoBehaviour
{
    [SerializeField] TMP_Text killText;
    [SerializeField] EnemyDeathCounter deathCounter;

    int lastCount = -1; // Para detectar cambios

    private void Update()
    {
        if (deathCounter == null || killText == null) return;

        int currentCount = deathCounter.TotalKills;
        if (currentCount != lastCount)
        {
            lastCount = currentCount;
            killText.text = currentCount.ToString(); // Solo el número
        }
    }
}
