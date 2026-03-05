using UnityEngine;

public class EnemyDeathCounter : MonoBehaviour
{
    public int TotalKills { get; private set; }

    public void AddKill()
    {
        TotalKills++;
        Debug.Log("Enemigo muerto. Total de muertes: " + TotalKills);
    }
}
