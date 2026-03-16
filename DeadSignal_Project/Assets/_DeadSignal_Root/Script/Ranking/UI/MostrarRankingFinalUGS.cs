using UnityEngine;
using Unity.Services.Leaderboards.Models;
using Unity.Services.Authentication;
using System.Collections.Generic;
using TMPro;



public class MostrarRankingFinalUGS : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private OnlineRankingManager rankingManager;
    [SerializeField] private Transform container;
    [SerializeField] private GameObject rowPrefab;

    private List<GameObject> activeRows = new List<GameObject>();

    private void OnEnable()
    {
        if (rankingManager != null)
            rankingManager.OnRankingUpdated += Render;
    }

    private void OnDisable()
    {
        if (rankingManager != null)
            rankingManager.OnRankingUpdated -= Render;
    }

    private void Render(List<LeaderboardEntry> top50, LeaderboardEntry playerEntry)
    {
        Clear();

        string myPlayerId = AuthenticationService.Instance.PlayerId;

        foreach (var entry in top50)
        {
            string displayName = GetDisplayName(entry);

            GameObject row = Instantiate(rowPrefab, container);

            TextMeshProUGUI[] texts = row.GetComponentsInChildren<TextMeshProUGUI>();

            if (texts.Length >= 3)
            {
                texts[0].text = entry.Rank.ToString();
                texts[1].text = displayName;
                texts[2].text = entry.Score.ToString();
            }

            // Resaltar si es el jugador actual con COLOR AMARILLO
            if (entry.PlayerId == myPlayerId)
            {
                foreach (var t in texts)
                {
                    t.color = Color.yellow;
                }
            }

            activeRows.Add(row);
        }

        // Si el jugador no está en el Top 50, lo mostramos abajo COLOR CYAN
        if (playerEntry != null && !IsPlayerInTop(top50, myPlayerId))
        {
            GameObject row = Instantiate(rowPrefab, container);

            TextMeshProUGUI[] texts = row.GetComponentsInChildren<TextMeshProUGUI>();

            string displayName = GetDisplayName(playerEntry);

            if (texts.Length >= 3)
            {
                texts[0].text = playerEntry.Rank.ToString();
                texts[1].text = displayName;
                texts[2].text = playerEntry.Score.ToString();
            }

            foreach (var t in texts)
            {
                t.color = Color.cyan;
            }

            activeRows.Add(row);
        }
    }

    private void Clear()
    {
        foreach (var row in activeRows)
        {
            Destroy(row);
        }

        activeRows.Clear();
    }

    private bool IsPlayerInTop(List<LeaderboardEntry> list, string playerId)
    {
        foreach (var entry in list)
        {
            if (entry.PlayerId == playerId)
                return true;
        }
        return false;
    }

    private string GetDisplayName(LeaderboardEntry entry)
    {
        if (!string.IsNullOrEmpty(entry.Metadata))
        {
            LeaderboardMetadata meta =
                JsonUtility.FromJson<LeaderboardMetadata>(entry.Metadata);

            if (meta != null && !string.IsNullOrEmpty(meta.displayName))
                return meta.displayName;
        }

        return "Unknown";
    }
}