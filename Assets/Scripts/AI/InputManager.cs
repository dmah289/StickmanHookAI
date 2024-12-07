using System;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    // Sự kiện cho các script khác đăng ký
    public static event Action OnSpacePressed;
    public static event Action OnSpaceReleased;
    public static event Action OnLeftPressed;
    public static event Action OnRightPressed;
    public static event Action OnGameplayEntered;

    public static void TriggerSpacePressed()
    {
        if (!GameManager.instance.stickman.won)
        {
            OnSpacePressed?.Invoke();
        }
    }

    public static void TriggerSpaceReleased()
    {
        if (!GameManager.instance.stickman.won)
        {
            OnSpaceReleased?.Invoke();
        }
    }

    public static void TriggerLeftPressed()
    {
        OnLeftPressed?.Invoke();
    }

    public static void TriggerRightPressed()
    {
        OnRightPressed?.Invoke();
    }

    public static void TriggerGameplayEntered()
    {
        OnGameplayEntered?.Invoke();
    }
}