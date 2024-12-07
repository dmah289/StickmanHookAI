using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDataLobby : MonoBehaviour
{
    [SerializeField] private string _id;
    [SerializeField] private string _playerTag;
    [SerializeField] private bool _isReady;
    [SerializeField] private int _charIdx;

    public string ID => _id;

    public string PlayerTag => _playerTag;

    public bool IsReady
    {
        get => _isReady;
        set => _isReady = value;
    }

    public int CharIdx
    {
        get => _charIdx;
        set => _charIdx = value;
    }

    public void Initialize(string id, bool isReady, string playerTag, int charIdx)
    {
        _id = id;
        _playerTag = playerTag;
        _charIdx = charIdx;
        _isReady = isReady;
    }

    public void Initialize(Dictionary<string, PlayerDataObject> playerData)
    {
        if (playerData != null)
            UpdateState(playerData);
    }

    public void UpdateState(Dictionary<string, PlayerDataObject> playerData)
    {
        if (playerData.ContainsKey("ID"))
            _id = playerData["ID"].Value;
        if(playerData.ContainsKey("PlayerTag"))
            _playerTag = playerData["PlayerTag"].Value;
        if (playerData.ContainsKey("IsReady"))
            _isReady = playerData["IsReady"].Value == "True";
        if (playerData.ContainsKey("CharIdx"))
            _charIdx = int.Parse(playerData["CharIdx"].Value);
    }

    public Dictionary<string, string> SerializePlayerData()
    {
        return new Dictionary<string, string>()
        {
            {"_id", _id},
            {"PlayerTag", _playerTag},
            {"IsReady", _isReady.ToString()},
            {"CharIdx", _charIdx.ToString()},
        };
    }
}
