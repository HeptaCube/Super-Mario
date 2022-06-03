using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 스프라이트 관련 변수들.
    public AnimatedSprite getAnimatedSprite;
    public int spriteDirection { get; private set; }

    // Collider과 Rigidbody를 담기 위한 변수.
    Collider2D getCollider2D;
    Rigidbody2D getRigidbody2D;

    // 눌린 키를 담을 int형 변수 생성.
    public int recognizedKey { get; private set; }

    // 오디오 소스를 담을 변수.
    AudioSource audioSource;


    // 필요한 메소드들을 매 프레임마다 불러와줌.
    private void Update()
    {
        SpriteControl();
        recognizedKey = CheckRecognizedKey();
        CheckRecognizedKey();
        CheckJumpState();
    }


    private void Start()
    {
        getRigidbody2D = GetComponent<Rigidbody2D>();
        getAnimatedSprite = GetComponent<AnimatedSprite>();
        audioSource = GetComponent<AudioSource>();
        spriteDirection = 1;
    }


    // 점프키를 안 눌렀고 멈춘 상태가 맞는지 확인함.
    // 두 조건이 참이면 점프 상태를 끔.
    private void CheckJumpState()
    {
        if (recognizedKey != 1 && this.getRigidbody2D.velocity.y == 0)
        {
            this.getAnimatedSprite.isJump = false;
        }
    }


    // 어떤 키가 눌렸는지 확인한다.
    public int CheckRecognizedKey()
    {
        // 점프키 누른 상태 : 1
        if (Input.GetButton("Jump"))
        {
            // 점프를 허용함.
            this.getAnimatedSprite.isJump = true;
            return 1;
        }
        // 점프키 뗀 순간 : 2
        else if (Input.GetButtonUp("Jump"))
        {
            return 2;

        }
        // 안누름 : 0
        else
        {
            return 0;
        }
    }


    // 오디오를 재생하는 메소드.
    void PlayAudio()
    {
        audioSource.time = 0.8f;
        audioSource.Play();
    }


    // 스프라이트를 컨트롤하는 메소드.
    void SpriteControl()
    {
        // 멈춘 상태가 아니라면 idle 상태를 품.
        if (Input.GetAxis("MoveX") != 0)
        {
            this.getAnimatedSprite.isIdle = false;
        }
        else
        // 그렇지 않다면 idle 상태로 만들어줌.
        {
            this.getAnimatedSprite.isIdle = true;
            return;
        }

        // 스프라이트의 좌우 방향을 지정해줌.
        if (Input.GetAxis("MoveX") > 0)
        {
            spriteDirection = 1;
        }
        else if (Input.GetAxis("MoveX") < 0)
        {
            spriteDirection = -1;
        }
    }
}