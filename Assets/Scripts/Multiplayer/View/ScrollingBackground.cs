using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingBackground : MonoBehaviour
{
    [SerializeField] RawImage bg;
    [SerializeField] Vector2 offset;

    private void Update()
    {
        AnimateBg();
    }

    private void AnimateBg()
    {
        bg.uvRect = new Rect(bg.uvRect.position + offset * Time.deltaTime, bg.uvRect.size);
    }
}
