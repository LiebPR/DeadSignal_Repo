using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DropItem
{
    public string poolTag;
    [Range(0f, 1f)]
    public float weight;
}
public class EnemyDrop : MonoBehaviour
{
    #region References
    EnemyFSM FSM;
    #endregion

    #region Drop Settings
    [SerializeField] List<DropItem> dropTable = new List<DropItem>();

    [Tooltip("Probabilidad de NO dropear nada")]
    [SerializeField, Range(0f, 1f)] float noDropChance = 0.6f;

    #endregion

    private void Awake()
    {
        FSM = GetComponent<EnemyFSM>();
    }

    private void OnEnable()
    {
        if (FSM != null)
            FSM.OnStateChanged += HandleStateChange;
    }

    private void OnDisable()
    {
        if (FSM != null)
            FSM.OnStateChanged -= HandleStateChange;
    }

    #region State Listener
    void HandleStateChange(EnemyState newState)
    {
        if (newState == EnemyState.Death)
            DropLoot();
    }
    #endregion

    #region Drop Logic
    void DropLoot()
    {
        string poolTag = GetWeightedDrop();

        if (string.IsNullOrEmpty(poolTag))
            return; // no drop

        PoolManager.Instance.SpawnFromPool(poolTag, transform.position, Quaternion.identity);
    }

    string GetWeightedDrop()
    {
        float totalWeight = 0f;

        foreach (DropItem drop in dropTable) totalWeight += drop.weight;

        float randomValue = UnityEngine.Random.Range(0, totalWeight);

        //Chequeo de no drop
        if(randomValue < noDropChance) return null;
        randomValue -= noDropChance;

        //Selección ponderada de drop
        foreach (DropItem drop in dropTable)
        {
            if (randomValue < drop.weight) return drop.poolTag;

            randomValue -= drop.weight;
        }
        return null;
    }
    #endregion
}
