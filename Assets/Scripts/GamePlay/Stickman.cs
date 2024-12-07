using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stickman : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] public Sprite ballSprite;
    [SerializeField] public Sprite stopSprite;
    [SerializeField] public Sprite speedUpSprite;
    [SerializeField] public Sprite slowDownSprite;
    [SerializeField] public Sprite winSprite;

    [Header("Components")]
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] HingeJoint2D hingeJoint2D;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] public TrailRenderer trailRenderer;

    [Header("Charater Stat")]
    [SerializeField] float gravityRope;
    [SerializeField] float gravityAir;
    [SerializeField] float factorX;
    [SerializeField] float factorY;
    [SerializeField] float winSpeed;

    private int lastBestPosSwung;           // Điểm vừa đu
    private int lastBestPosSelected;        // Điểm đu gần nhất được chọn
    private int bestPos;                    // Điểm đu tốt nhất hiện tại
    private float bestDistance;
    private Vector3 actualJointPos;
    [SerializeField] public bool won;

    public bool Sticked { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        bestPos = 0;
        lastBestPosSwung = 0;
        lastBestPosSelected = 0;
        Sticked = false;
        won = false;
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.zero);
        lineRenderer.enabled = true;

        spriteRenderer.sprite = ballSprite;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0f;
        gameObject.transform.position = LevelManager.instance.currentLevel.playerPos;
        gameObject.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);

        InputManager.OnSpacePressed += Swing;
        InputManager.OnSpaceReleased += Release;
    }

    private void OnDisable()
    {
        trailRenderer.enabled = false;

        InputManager.OnSpacePressed -= Swing;
        InputManager.OnSpaceReleased -= Release;
    }

    private void Update()
    {
        if (!won)
        {
            UpdateBestPos();
            CheckInput();
            CheckLineRender();
            UpdateSelectedPos();
        }
    }

    private void UpdateBestPos()
    {
        bestPos = 0;
        bestDistance = float.MaxValue;

        // Duyệt qua tất cả các điểm và tìm vị trí phù hợp nhất
        for (int i = 0; i < LevelManager.instance.joints.Count; i++)
        {
            float actualDistance = Vector2.Distance(gameObject.transform.position, LevelManager.instance.joints[i].transform.position);
            if (actualDistance < bestDistance)
            {
                bestPos = i;
                bestDistance = actualDistance;
            }
        }
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            SwingKeyboard();
        }
        if (Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0))
        {
            ReleaseKeyBoard();
        }
    }

    private void Release()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => { ReleaseKeyBoard(); });
    }

    private void ReleaseKeyBoard()
    {
        lineRenderer.enabled = false;
        hingeJoint2D.enabled = false;
        rb.gravityScale = gravityAir;       // Đặt trọng lực rơi tự do

        rb.velocity = new Vector2(rb.velocity.x * factorX, rb.velocity.y + factorY);
        LevelManager.instance.joints[lastBestPosSwung].SetUnsticked();

        // Nếu vị trí tốt nhất không đổi thì cho chọn
        if (bestPos == lastBestPosSwung)
            LevelManager.instance.joints[lastBestPosSwung].Selectable();

        spriteRenderer.sprite = ballSprite;
        rb.AddTorque(-rb.velocity.magnitude);
        Sticked = false;
    }


    private void Swing()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => { SwingKeyboard(); });
    }

    private void SwingKeyboard()
    {
        lineRenderer.enabled = true;
        hingeJoint2D.enabled = true;
        rb.gravityScale = gravityRope;      // Đặt trọng lực khi đu dây

        hingeJoint2D.connectedBody = LevelManager.instance.rb2ds[bestPos];
        actualJointPos = LevelManager.instance.joints[bestPos].gameObject.transform.position;
        LevelManager.instance.joints[bestPos].SetSticked();

        lastBestPosSwung = bestPos;
        rb.angularVelocity = 0f;
        Sticked = true;
    }

    private void CheckLineRender()
    {
        if (Sticked)
        {
            lineRenderer.SetPosition(0, gameObject.transform.position);
            lineRenderer.SetPosition(1, actualJointPos);
            ChangeState();
        }
    }

    private void UpdateSelectedPos()
    {
        // Chỉ cập nhật trạng thái chọn khi có điểm mới tốt hơn
        if (lastBestPosSelected != bestPos)
        {
            LevelManager.instance.joints[lastBestPosSelected].Unselectable();
            LevelManager.instance.joints[bestPos].Selectable();
            lastBestPosSelected = bestPos;
        }
    }

    private void ChangeState()
    {
        spriteRenderer.flipX = rb.velocity.x > 0.7f ? false : true;

        if (rb.velocity.x > -0.7f && rb.velocity.x < 0.7f && gameObject.transform.position.y < actualJointPos.y)
            spriteRenderer.sprite = stopSprite;
        else
            spriteRenderer.sprite = (rb.velocity.y > 0) ? slowDownSprite : speedUpSprite;

        gameObject.transform.eulerAngles = gameObject.transform.eulerAngles
            .With(z : Vector2.SignedAngle(Vector2.up, actualJointPos - gameObject.transform.position));
    }

    public void Win()
    {
        won = true;
        Sticked = false;
        spriteRenderer.flipX = false;
        spriteRenderer.sprite = winSprite;
        lineRenderer.enabled = false;

        rb.gravityScale = 0f;
        rb.velocity = rb.velocity.normalized * winSpeed;
        rb.angularVelocity = 0f;

        gameObject.transform.eulerAngles = gameObject.transform.eulerAngles.With(z: Vector2.SignedAngle(Vector2.up, rb.velocity));
        float currentY = gameObject.transform.position.y;
        if (currentY >= 6f)
            gameObject.transform.position = gameObject.transform.position.With(y: 4f);
        else if (currentY <= -6f)
            gameObject.transform.position = gameObject.transform.position.With(y: -4f);

    }
}
