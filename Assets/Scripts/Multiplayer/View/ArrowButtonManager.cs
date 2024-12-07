using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArrowButtonManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static Action<Image> OnArrowPressed;
    public static Action<Image> OnArrowReleased;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnArrowPressed?.Invoke(GetComponent<Image>());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnArrowReleased?.Invoke(GetComponent<Image>());
    }

}
