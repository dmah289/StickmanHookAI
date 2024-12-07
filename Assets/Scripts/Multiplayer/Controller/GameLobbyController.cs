using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class GameLobbyController : GenericSingleton<GameLobbyController>
{
    // Nơi Prefab truy cập để cập nhật UI
    List<PlayerDataLobby> _playersLobbyList = new List<PlayerDataLobby>();
    
    // Người chơi tại máy local
    public PlayerDataLobby _localPlayerDataLobby;
    
    private void OnEnable()
    {
        LobbyEvents.OnLobbyUpdated += OnLobbyUpdated;
    }

    private void OnDisable()
    {
        LobbyEvents.OnLobbyUpdated -= OnLobbyUpdated;
    }

    public string GetLobbyCode()
    {
        return LobbyController.instance.GetLobbyCode();
    }
    
    public async Task<bool> CreateLobby()
    {
        // Khởi tạo đối tượng Player trên sảnh -> Tuần tự hóa -> Gửi lên Services
        PlayerDataLobby playerDataLobby = new PlayerDataLobby();
        playerDataLobby.Initialize(AuthenticationService.Instance.PlayerId, true, "HostPlayer", 1);

        _localPlayerDataLobby = playerDataLobby;
        
        bool succeeded = await LobbyController.instance.CreateLobby("Room1", 4, false, playerDataLobby.SerializePlayerData());
        return succeeded;
    }

    public async Task<bool> JoinLobby(string code)
    {
        PlayerDataLobby playerDataLobby = new PlayerDataLobby();
        playerDataLobby.Initialize(AuthenticationService.Instance.PlayerId, false, "JoinedPlayer", 1);
        
        _localPlayerDataLobby = playerDataLobby;

        bool succeeded = await LobbyController.instance.JoinLobby(code, playerDataLobby.SerializePlayerData());
        return succeeded;
    }
    
    private void OnLobbyUpdated(Lobby lobby)
    {
        // Lấy danh sách thông tin player trong lobby mỗi khi lobby được update
        List<Dictionary<string, PlayerDataObject>> playersData = LobbyController.instance.GetPlayersData();
        
        // Xóa dữ liệu cũ
        _playersLobbyList.Clear();

        // Truyền dữ liệu mới vào List
        foreach (Dictionary<string, PlayerDataObject> data in playersData)
        {
            // Tạo 1 đối tượng để lưu trữ mới
            PlayerDataLobby playerDataLobby = new PlayerDataLobby();
            playerDataLobby.Initialize(data);
            
            // Kiểm tra xem phần tử này có phải người chơi local không
            if (playerDataLobby.ID == AuthenticationService.Instance.PlayerId)
            {
                _localPlayerDataLobby = playerDataLobby;
            }
            
            // Add đối tượng vừa tạo vào List
            _playersLobbyList.Add(playerDataLobby);
        }
        
        GameLobbyEvent.TriggerOnGameLobbyUpdated();
    }

    public List<PlayerDataLobby> GetPlayers()
    {
        return _playersLobbyList;
    }

    internal async Task<bool> SetLocalPlayerReady()
    {
        _localPlayerDataLobby.IsReady = true;
        return await LobbyController.instance.UpdatePlayerData(_localPlayerDataLobby.ID, _localPlayerDataLobby.SerializePlayerData());
    }

    public async Task<bool> SetLocalPlayerSprite(int index)
    {
        _localPlayerDataLobby.CharIdx = index;
        return await LobbyController.instance.UpdatePlayerData(_localPlayerDataLobby.ID, _localPlayerDataLobby.SerializePlayerData());
    }

    internal async Task<bool> RemovePlayer()
    {
        return await LobbyController.instance.RemovePlayerFromLobby(_localPlayerDataLobby.ID);
    }
}
