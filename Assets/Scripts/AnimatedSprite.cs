using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimatedSprite : MonoBehaviour
{
    // 요소들을 담을 변수.
    public SpriteRenderer getSpriteRenderer { get; private set; }
    public PlayerController getPlayerController { get; private set; }

    // Collider과 Rigidbody를 담기 위한 변수.
    Collider2D getCollider2D;
    Rigidbody2D getRigidbody2D;

    public Sprite[] sprites;
    public float animationTime = 0.25f;
    public int animationFrame { get; private set; }
    public bool loop = true;

    // idle, jump 관련 변수들.
    public Sprite idleSprite;
    public Sprite jumpSprite;
    public bool isIdle;
    public bool isJump;


    private void Awake()
    {
        getRigidbody2D = GetComponent<Rigidbody2D>();
        getCollider2D = GetComponent<Collider2D>();

        this.getSpriteRenderer = GetComponent<SpriteRenderer>();
        this.getPlayerController = GetComponent<PlayerController>();
        isIdle = true;
        isJump = false;
    }


    private void Start()
    {
        // 애니메이션이 돌아가는 메소드의 반복을 시작한다.
        InvokeRepeating(nameof(Advance), this.animationTime / 5, this.animationTime / 5);
    }


    // 필요한 메소드들을 매 프레임마다 불러와줌.
    private void Update()
    {
        spriteDirection();
        SetSpriteStatus();
        SpriteControl();
        CheckJumpState();

        if (gameObject.CompareTag("Player") && Input.GetButton("Jump"))
        {
            TriggerJump();
        }

    }


    // 스프라이트의 상태를 결정한다.
    private void SetSpriteStatus()
    {
        // isJump가 참이라면 스프라이트를 점프 상태로 바꾼다.
        if (isJump)
        {
            this.getSpriteRenderer.sprite = jumpSprite;
            this.animationFrame = 0;
            return;
        }
        // 그렇지 않다면 idle도 검사한다.
        else if (isIdle)
        {
            this.getSpriteRenderer.sprite = idleSprite;
            this.animationFrame = 0;
            return;
        }
    }

    private void Advance()
    {
        this.animationFrame++;

        // "애니메이션 프레임이 최대길이보다 같거나 많은" 상태이고 "loop가 true" 라면 
        if (this.animationFrame >= this.sprites.Length && this.loop && !isIdle && !isJump)
        {
            // 애니메이션의 프레임을 0으로 되돌림.
            this.animationFrame = 0;
        }

        // "애니메이션 프레임이 0 이상인 상태"이고 "애니메이션 프레임이 스프라이트 길이 미만"인 경우
        // 스프라이트가 동작한다.
        if (this.animationFrame >= 0 && this.animationFrame < this.sprites.Length && !isIdle && !isJump)
        {
            this.getSpriteRenderer.sprite = this.sprites[this.animationFrame];
        }
    }


    // 스프라이트의 방향을 결정해줌.
    private void spriteDirection()
    {
        gameObject.transform.localScale = new Vector2(this.getPlayerController.spriteDirection, 1);
    }

    // 스프라이트를 컨트롤하는 메소드.
    void SpriteControl()
    {
        // 게임태그가 Player인 경우 실행됨.
        if (gameObject.CompareTag("Player"))
        {
            // 멈춘 상태가 아니라면 idle 상태를 품.
            if (Input.GetAxis("MoveX") != 0)
            {
                isIdle = false;
            }
            else
            // 그렇지 않다면 idle 상태로 만들어줌.
            {
                isIdle = true;
                return;
            }

            // 스프라이트의 좌우 방향을 지정해줌.
            if (Input.GetAxis("MoveX") > 0)
            {
                this.getPlayerController.spriteDirection = 1;
            }
            else if (Input.GetAxis("MoveX") < 0)
            {
                this.getPlayerController.spriteDirection = -1;
            }
        }

        // 게임태그가 Monster인 경우 실행됨.
        if (gameObject.CompareTag("Monster"))
        {
            // 코드.
        }
    }

    private void CheckJumpState()
    {
        // 게임태그가 Player인 경우 실행됨.
        if (gameObject.CompareTag("Player"))
        {
            // 점프키를 안 눌렀고 멈춘 상태가 맞는지 확인함.
            // 두 조건이 참이면 점프 상태를 끔.
            if (this.getRigidbody2D.velocity.y == 0)
            {
                isJump = false;
            }
        }
    }

    void TriggerJump()
    {
        // 점프를 허용함.
        isJump = true;
    }

    public void Restart()
    {
        // 이 값을 0으로 하면 Advance() 메서드에서 this.animationFrame++ 가 적용되므로
        // 0이 아니라 1부터 시작하게 된다. 그렇기 때문에 -1로 초기화한다.
        this.animationFrame = -1;
        Advance();
    }
}
