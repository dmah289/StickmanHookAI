using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    LOADING,
    MATCHMAKING,
    LOBBY,
    GAMEPLAY
}

public class DataTransition : GenericSingleton<DataTransition>
{
    [SerializeField] Sprite ball;
    [SerializeField] Sprite slowDown;
    [SerializeField] Sprite speedUp;
    [SerializeField] Sprite stop;
    [SerializeField] Sprite win;

    [SerializeField] public GameState gameState;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public void SetCharacterSprites(params Sprite[] sprites)
    {
        ball = sprites[0];
        slowDown = sprites[1];
        speedUp = sprites[2];
        stop = sprites[3];
        win = sprites[4];
    }

    public void GetCharacterSprites()
    {
        GameManager.instance.stickman.ballSprite = ball;
        GameManager.instance.stickman.slowDownSprite = slowDown;
        GameManager.instance.stickman.speedUpSprite = speedUp;
        GameManager.instance.stickman.stopSprite = stop;
        GameManager.instance.stickman.winSprite = win;
    }
}
