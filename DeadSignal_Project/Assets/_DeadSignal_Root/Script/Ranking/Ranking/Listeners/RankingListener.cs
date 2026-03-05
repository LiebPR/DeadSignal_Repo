using UnityEngine;
using UnityEngine.SceneManagement;

public class RankingListener : MonoBehaviour
{
    [SerializeField] private OnlineRankingManager rankingManager;

    private void OnEnable()
    {
        MatchEvents.OnMatchEnded += HandleMatchEnded;
    }

    private void OnDisable()
    {
        MatchEvents.OnMatchEnded -= HandleMatchEnded;
    }

    private async void HandleMatchEnded(int finalScore)
    {
        string playerName = PlayerPrefs.GetString("PlayerName", "Player");

        await rankingManager.SubmitAndRefresh(finalScore, playerName);

        SceneManager.LoadScene("SCN_Ranking");
    }
}