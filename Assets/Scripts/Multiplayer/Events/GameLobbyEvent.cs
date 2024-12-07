public static class GameLobbyEvent
{
    // Events.LobbyEvents
    public delegate void OnLobbyUpdatedDelegate();

    public static OnLobbyUpdatedDelegate OnGameLobbyUpdated;

    public static void TriggerOnGameLobbyUpdated()
    {
        OnGameLobbyUpdated?.Invoke();
    }
}
