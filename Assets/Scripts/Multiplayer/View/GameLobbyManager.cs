using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameLobbyManager : GenericSingleton<GameLobbyManager>
{
    [SerializeField] private bool isHost;

    [SerializeField] RectTransform _lobby;
    [SerializeField] Text statusAction;
    [SerializeField] Text lobbyCode;

    [SerializeField] Button enterGameBtn;
    [SerializeField] Button leaveGameBtn;
    
    // Danh sách player prefab trong Lobby
    [SerializeField] private List<PlayerDataLobbyManager> lobbyPlayers;

    private void OnEnable()
    {
        InputManager.OnGameplayEntered += OnStartPressed;
        GameLobbyEvent.OnGameLobbyUpdated += OnLobbyUpdated;
        leaveGameBtn.onClick.AddListener(OnLeaveLobby);

        isHost = LobbyController.instance.CheckHost();
        if (isHost)
        {
            statusAction.text = "Start";
            enterGameBtn?.onClick.AddListener(OnStartPressed);
        }
        else
        {
            statusAction.text = "Ready";
            enterGameBtn?.onClick.AddListener(OnReadyPressed);
        }
        
        lobbyCode.text = GameLobbyController.instance.GetLobbyCode();
    }

    private void OnDisable()
    {
        GameLobbyEvent.OnGameLobbyUpdated -= OnLobbyUpdated;
        InputManager.OnGameplayEntered -= OnStartPressed;
        leaveGameBtn.onClick.RemoveListener(OnLeaveLobby);

        if (isHost)
        {
            enterGameBtn?.onClick.RemoveListener(OnStartPressed);
        }
        else
        {
            enterGameBtn?.onClick.RemoveListener(OnReadyPressed);
        }
    }

    private void OnLobbyUpdated()
    {
        List<PlayerDataLobby> playersData = GameLobbyController.instance.GetPlayers();

        ResetPlaceholder();

        for (int i = 0; i < playersData.Count; i++)
        {
            // Lấy dữ liệu mới được cập nhật
            PlayerDataLobby data = playersData[i];

            lobbyPlayers[i].gameObject.SetActive(true);

            // Đặt dữ liệu cho prefab
            lobbyPlayers[i].SetData(data);
        }
    }

    private void ResetPlaceholder()
    {
        for (int i = 0; i < lobbyPlayers.Count; i++)
        {
            lobbyPlayers[i].gameObject.SetActive(false);
        }
    }

    public void OnStartPressed()
    {
        DataTransition.instance.gameState = GameState.LOADING;
        _lobby.DOScale(Vector3.zero, 1f).SetEase(Ease.InBack).OnComplete(() =>
        {
            CanvasManager.instance.EnterGameplay();
        });
    }

    public async void OnReadyPressed()
    {
        bool succeeded = await GameLobbyController.instance.SetLocalPlayerReady();
    }

    private async void OnLeaveLobby()
    {
        bool succedded = await GameLobbyController.instance.RemovePlayer();
        if(succedded)
        {
            // To-do: Chuyển về giao diện MatchMaking
        }
    }
}