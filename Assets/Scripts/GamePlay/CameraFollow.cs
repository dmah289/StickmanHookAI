using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] Vector3 initPos;

    [SerializeField] float speed;
    [SerializeField] float zoomInRate;
    [SerializeField] public float offset;       // Tính tỷ lệ offset tương ứng cho Dynamic Shadow
    [SerializeField] public float maxOffset;
    [SerializeField] float minX, maxX;

    private void Update()
    {
        if (GameManager.instance.playerRb.velocity.x > 0f)
        {
            offset += Time.deltaTime * speed;
            if (offset > maxOffset) offset = maxOffset;
        }
        else if (GameManager.instance.playerRb.velocity.x < 0f)
        {
            offset -= Time.deltaTime * speed;
            if (offset < -maxOffset) offset = -maxOffset;
        }

        float nextX = GameManager.instance.player.transform.position.x + offset;
        if (nextX < minX) nextX = minX;
        if (nextX > maxX) nextX = maxX;

        gameObject.transform.position = gameObject.transform.position.With(x: nextX);
    }

    public void Win()
    {
        maxOffset = 0;
        cam.gameObject.transform.position = GameManager.instance.player.transform.position.With(z: -10);
        cam.orthographicSize /= zoomInRate;
    }

    public void Reset()
    {
        maxOffset = 0.5f;
        gameObject.transform.position = initPos;
        cam.orthographicSize *= zoomInRate;
    }

}
