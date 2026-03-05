using UnityEngine;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public class UGSGlobalRanking : MonoBehaviour
{
    private const string LeaderboardId = "global-ranking";

    public async Task SubmitScore(int score, string playerName)
    {
        await LeaderboardsService.Instance.AddPlayerScoreAsync(
            LeaderboardId,
            score, 
            new AddPlayerScoreOptions
            {
                Metadata = JsonUtility.ToJson(
                    new LeaderboardMetadata { displayName = playerName }
                )
            }
        );

        Debug.Log("Score enviado con nombre: " + playerName);
    }

    public async Task<List<LeaderboardEntry>> GetTop50()
    {
        var response = await LeaderboardsService.Instance.GetScoresAsync(
            LeaderboardId,
            new GetScoresOptions
            {
                Limit = 50
            }
        );

        return response.Results;
    }

    public async Task<LeaderboardEntry> GetPlayerEntry()
    {
        var response = await LeaderboardsService.Instance.GetPlayerScoreAsync(LeaderboardId);
        return response;
    }
}