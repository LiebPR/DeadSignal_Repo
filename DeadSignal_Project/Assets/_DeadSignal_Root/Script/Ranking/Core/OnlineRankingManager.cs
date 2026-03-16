using UnityEngine;
using Unity.Services.Leaderboards.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


public class OnlineRankingManager : MonoBehaviour
{
    [SerializeField] private UGSGlobalRanking ugsRanking;

    public event Action<List<LeaderboardEntry>, LeaderboardEntry> OnRankingUpdated;

    public async Task SubmitAndRefresh(int score, string playerName, float matchTime)
    {
        // Enviar score
        await ugsRanking.SubmitScore(score, playerName,matchTime);

        // Obtener Top 50
        var top50 = await ugsRanking.GetTop50();

        // Obtener entrada del jugador
        var playerEntry = await ugsRanking.GetPlayerEntry();

        OnRankingUpdated?.Invoke(top50, playerEntry);
    }
}