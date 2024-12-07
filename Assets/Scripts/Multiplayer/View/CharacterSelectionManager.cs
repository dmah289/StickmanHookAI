using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class CharacterSelectionManager : GenericSingleton<CharacterSelectionManager>
{
    [Header("Anim-------------------------------------------")]
    [SerializeField] Sprite touchedArrow;
    [SerializeField] Sprite untouchedArrow;

    [Header("Logic------------------------------------------")]
    [SerializeField] int index;
    [SerializeField] Sprite[] characters;

    [SerializeField] Image currChar;
    [SerializeField] Image nextChar;
    [SerializeField] Image prevChar;

    [Header("Anim List --------------------------------------")]
    [SerializeField] Sprite[] balls;
    [SerializeField] Sprite[] slowdown;
    [SerializeField] Sprite[] speedUp;
    [SerializeField] Sprite[] stop;
    [SerializeField] Sprite[] win;


    private void OnEnable()
    {
        InputManager.OnLeftPressed += HandleLeftArrow;
        InputManager.OnRightPressed += HandleRightArrow;
        InputManager.OnGameplayEntered += HandleEnterGameplay;
    }

    private void Start()
    {
        index = 1;
        prevChar.sprite = characters[index - 1];
        nextChar.sprite = characters[index + 1];
        currChar.sprite = characters[index];

        ArrowButtonManager.OnArrowPressed += HandleArrowDown;
        ArrowButtonManager.OnArrowReleased += HandleArrowUp;

    }

    public Sprite GetSpriteByIndex(int index)
    {
        return characters[index];
    }

    private void HandleArrowUp(Image img)
    {
        img.sprite = untouchedArrow;
    }

    public void HandleArrowDown(Image img)
    {
        img.sprite = touchedArrow;

        if (img.gameObject.name.Equals(KeySave.leftArrow))
        {
            HandleLeftArrow();
        }
        else if(img.gameObject.name.Equals (KeySave.rightArrow))
        {
            HandleRightArrow();
        }
    }

    public void HandleLeftArrow()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(async () => {
            index = Math.Max(--index, 0);
            UpdateCharacterSprite();
        });
        
    }

    public void HandleRightArrow()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(async () => {
            index = Mathf.Min(++index, characters.Length - 1);
            UpdateCharacterSprite();
        });
    }

    private void HandleEnterGameplay()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            DataTransition.instance.SetCharacterSprites(
            balls[index], slowdown[index], speedUp[index], stop[index], win[index]);
        });
    }

    public async void UpdateCharacterSprite()
    {
        bool succeeded = await GameLobbyController.instance.SetLocalPlayerSprite(index);

        if (succeeded)
        {
            currChar.sprite = characters[index];

            if (index == 0)
            {
                prevChar.sprite = null;
                nextChar.sprite = characters[index + 1];
            }
            else if (index == characters.Length - 1)
            {
                prevChar.sprite = characters[index - 1];
                nextChar.sprite = null;
            }
            else
            {
                prevChar.sprite = characters[index - 1];
                nextChar.sprite = characters[index + 1];
            }
        }
        else await Task.Yield();
    }

    private void OnDisable()
    {
        InputManager.OnLeftPressed -= HandleLeftArrow;
        InputManager.OnRightPressed -= HandleRightArrow;
        InputManager.OnGameplayEntered -= HandleEnterGameplay;
    }

    
}
