using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MatchMakingManager : MonoBehaviour
{
    [SerializeField] private Button _hostBtn;
    [SerializeField] private Button _joinBtn;

    [SerializeField] Transform _matchMaking;
    [SerializeField] Transform _lobby;
    
    [SerializeField] InputField _codeInputField;

    private void Awake()
    {
        transform.localScale = Vector3.zero;
    }

    private void OnEnable()
    {
        _hostBtn.onClick.AddListener(OnHostClicked);
        _joinBtn.onClick.AddListener(OnJoinClicked);
    }

    private void OnDisable()
    {
        _hostBtn.onClick.RemoveListener(OnHostClicked);
        _joinBtn.onClick.RemoveListener(OnJoinClicked);
    }

    private async void OnHostClicked()
    {
        bool succeeded = await GameLobbyController.instance.CreateLobby();

        await EnterGameLobby(succeeded);
    }
    
    private async void OnJoinClicked()
    {
        string code = _codeInputField.text; 

        if (string.IsNullOrEmpty(code))
        {
            CanvasManager.instance.AnimateNotification("Lobby code is required");
        }
        else
        {
            print(code + " " + code.Length);
            bool succeeded = await GameLobbyController.instance.JoinLobby(code);
            await EnterGameLobby(succeeded);
        }
    }

    private async Task EnterGameLobby(bool succeeded)
    {
        if (succeeded)
        {
            _matchMaking.DOScale(Vector3.zero, 1f).SetEase(Ease.InBack).OnComplete(() =>
            {
                DataTransition.instance.gameState = GameState.LOBBY;
                _lobby.gameObject.SetActive(true);
                _lobby.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack);
                _matchMaking.gameObject.SetActive(false);
            });
        }
        else await Task.Yield();
    }
}
