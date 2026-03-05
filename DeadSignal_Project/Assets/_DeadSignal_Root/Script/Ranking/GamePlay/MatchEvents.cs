using System;

public static class MatchEvents
{
    public static event Action<int> OnMatchEnded;

    public static void RaiseMatchEnded(int finalScore)
    {
        OnMatchEnded?.Invoke(finalScore);
    }
}