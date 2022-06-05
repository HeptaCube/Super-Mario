using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 스프라이트 관련 변수들.
    public AnimatedSprite getAnimatedSprite;
    public int spriteDirection;

    // Collider과 Rigidbody를 담기 위한 변수.
    Collider2D getCollider2D;
    Rigidbody2D getRigidbody2D;

    // 오디오 소스를 담을 변수.
    AudioSource audioSource;


    private void Start()
    {
        getRigidbody2D = GetComponent<Rigidbody2D>();
        getAnimatedSprite = GetComponent<AnimatedSprite>();
        audioSource = GetComponent<AudioSource>();
        spriteDirection = 1;
    }


    // 눌린 키를 담을 int형 변수 생성.
    //public int recognizedKey { get; private set; }
    public int recognizedKey
    {
        get
        {
            // 점프키 누른 상태 : 1
            if (Input.GetButton("Jump"))
            {
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
    }


    // 오디오를 재생하는 메소드.
    void PlayAudio()
    {
        audioSource.time = 0.8f;
        audioSource.Play();
    }
}