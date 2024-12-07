using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using UnityEngine;

public class LobbyController : GenericSingleton<LobbyController>
{
    public Lobby _lobby;
    private Coroutine _heartbeatCoroutine;
    private Coroutine _refreshLobbyCoroutine;
    
    public string GetLobbyCode()
    {
        return _lobby?.LobbyCode;
    }

    public async Task<bool> CreateLobby(string name, int maxPlayer, bool isPrivate, Dictionary<string, string> data)
    {
        Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);
        Player player = new Player(AuthenticationService.Instance.PlayerId, null, playerData);

        CreateLobbyOptions options = new CreateLobbyOptions()
        {
            IsPrivate = isPrivate,
            Player = player
        };

        try
        {
            _lobby = await LobbyService.Instance.CreateLobbyAsync(name, maxPlayer, options);
            // print($"Lobby created with: {_lobby.Name}, {_lobby.Id}");
        }
        catch (Exception)
        {
            return false;
        }
        
        _heartbeatCoroutine = StartCoroutine(HeartBeatLobbyCoroutine(_lobby.Id, 6f));
        _refreshLobbyCoroutine = StartCoroutine(RefreshLobbyCoroutine(_lobby.Id, 1f));
            
        return true;
    }
    
    public async Task<bool> JoinLobby(string code, Dictionary<string, string> playerData)
    {
        JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions()
        {
            Player = new Player(AuthenticationService.Instance.PlayerId, null, SerializePlayerData(playerData))
        };

        try
        {
            _lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code, options);
        }
        catch (Exception e)
        {
            return false;
        }

        // Không gửi heartbeat nhưng vẫn cần update lobby
        _refreshLobbyCoroutine = StartCoroutine(RefreshLobbyCoroutine(_lobby.Id, 1f));
        
        return true;
    }

    private IEnumerator HeartBeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        while (true)
        {
            // print("Sending heartbeat");
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return new WaitForSecondsRealtime(waitTimeSeconds);
        }
    }
    
    private IEnumerator RefreshLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        while (true)
        {
            Task<Lobby> task = LobbyService.Instance.GetLobbyAsync(lobbyId);
            yield return new WaitUntil(() => task.IsCompleted);
            Lobby newLobby = task.Result;
            if (newLobby.LastUpdated > _lobby.LastUpdated)
            {
                _lobby = newLobby;
                LobbyEvents.TriggerLobbyUpdated(_lobby);
            }
            
            yield return new WaitForSecondsRealtime(waitTimeSeconds);
        }
    }

    private Dictionary<string, PlayerDataObject> SerializePlayerData(Dictionary<string, string> data)
    {
        Dictionary<string, PlayerDataObject> playerData = new Dictionary<string, PlayerDataObject>();

        foreach(var (key, val) in data)
        {
            // Cho phép các thành viên trong sảnh nhìn thấy
            playerData.Add(key, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, val));
        }

        return playerData;
    }

    public bool CheckHost()
    {
        return _lobby.HostId.Equals(AuthenticationService.Instance.PlayerId);
    }

    private async void OnApplicationQuit()
    {
        // Chỉ host mới xóa được matchMaking
        if(_lobby != null && _lobby.HostId == AuthenticationService.Instance.PlayerId)
        {
            await LobbyService.Instance.DeleteLobbyAsync(_lobby.Id);
        }
        else
        {
            // To-do: Xử lý khi player thoát phòng thì tắt UI
            bool succeeded = await RemovePlayerFromLobby(AuthenticationService.Instance.PlayerId);
        }
        // StopCoroutine(_heartbeatCoroutine);
        // StopCoroutine(_refreshLobbyCoroutine);
    }

    public List<Dictionary<string, PlayerDataObject>> GetPlayersData()
    {
        List<Dictionary<string, PlayerDataObject>> result = new List<Dictionary<string, PlayerDataObject>>();

        // Duyệt qua dữ liệu người chơi trong lobby hiện tại để cập nhật vào list
        foreach (Player player in _lobby.Players)
        {
            result.Add(player.Data);
        }
        
        return result;
    }

    internal async Task<bool> UpdatePlayerData(string playerId, Dictionary<string, string> data)
    {
        Dictionary<string, PlayerDataObject> playerData = SerializePlayerData(data);

        UpdatePlayerOptions options = new UpdatePlayerOptions()
        {
            // Truyền dữ liệu được Serialize để cập nhật
            Data = playerData
        };
        try
        {
            // Cập nhật lobby sau khi có thay đổi trong sảnh
            _lobby = await LobbyService.Instance.UpdatePlayerAsync(_lobby.Id, playerId, options);
        }
        catch (Exception ex) { return false; }

        // Thông báo có thay đổi trong sảnh
        LobbyEvents.TriggerLobbyUpdated(_lobby);

        return true;
    }

    internal async Task<bool> RemovePlayerFromLobby(string playerId)
    {
        try
        {
            // Xóa người chơi khỏi lobby thông qua dịch vụ Lobby
            await LobbyService.Instance.RemovePlayerAsync(_lobby.Id, playerId);
        }
        catch (Exception ex)
        {
            return false;
        }

        // Thông báo rằng lobby đã thay đổi sau khi người chơi thoát
        LobbyEvents.TriggerLobbyUpdated(_lobby);

        return true;
    }
}
