using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDataLobbyManager : MonoBehaviour
{
    [Header("Sprites-------------------------------")]
    [SerializeField] private Sprite ready;
    [SerializeField] private Sprite pending;

    [Header("Components-------------------------------")]
    [SerializeField] private Text _playerNameText;
    [SerializeField] private Image _characterSprite;
    [SerializeField] private Image _status;

    [Header("Async Player Data-------------------------------")]
    [SerializeField] private PlayerDataLobby _playerDataLobby;

    public void SetData(PlayerDataLobby playerData)
    {
        _playerDataLobby = playerData;
        _playerNameText.text = playerData.PlayerTag;
        _characterSprite.sprite = CharacterSelectionManager.instance.GetSpriteByIndex(playerData.CharIdx);

        _status.sprite = playerData.IsReady ? ready : pending;
    }

    public void UpdateSprite(Sprite sprite)
    {
        _characterSprite.sprite = sprite;
    }
}
