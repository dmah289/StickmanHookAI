using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public static class LobbyEvents
{
    public delegate void LobbyUpdatedDelegate(Lobby lobby);
    public static LobbyUpdatedDelegate OnLobbyUpdated;

    public static void TriggerLobbyUpdated(Lobby lobby)
    {
        OnLobbyUpdated?.Invoke(lobby);
    }
}
