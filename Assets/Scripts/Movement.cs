using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // 외부 스크립트에서 접근할 수 없지만 [SerializeField]를 통해
    // private 변수를 인스펙터 창에서 접근할 수 있게 해줌.
    // 오브젝트, 캐릭터마다 다른 수치이므로 인스펙터에서 조절이 가능해야 함.

    [SerializeField]
    float jumpPower = 8f;

    [SerializeField]
    float moveSpeed = 5f;

    [SerializeField]
    float minJumpHeight = 1f;

    [SerializeField]
    float jumpCutDelay = 0.5f;

    [SerializeField]
    float maxJumpTime = 0.1f;

    // 필요한 요소들을 불러올 변수.
    Collider2D getCollider2D;
    Rigidbody2D getRigidbody2D;
    PlayerController getPlayerController;

    // transform.position.y 값을 담아줄 변수.
    float endJumpHeight = 0f;

    // 점프 거리와 관련된 변수.
    float startJumpHeight = 0f;
    float jumpDistance = 0f;

    // [플레이어] 버튼 인식과 관련된 변수.
    int recognizedKeyInput;
    int blockedDirection;


    private void Start()
    {
        // Collider2D와 Rigidbody2D를 불러옴.
        getCollider2D = GetComponent<Collider2D>();
        getRigidbody2D = GetComponent<Rigidbody2D>();

        // [플레이어] 버튼을 인식함.
        getPlayerController = GetComponent<PlayerController>();
    }

    // 필요한 메소드들을 매 프레임마다 불러와줌.
    private void Update()
    {
        OnMove();
        OnJump();

        // recognizedKeyInput 을 갱신해줌.
        recognizedKeyInput = this.getPlayerController.recognizedKey;
    }


    void OnJump()
    {
        Vector2 jumpVelocity = this.getRigidbody2D.velocity;
        float clampedJumpVertical = Mathf.Clamp(jumpVelocity.y, -0.01f, 0f);
        bool isRangedJumpVertical = clampedJumpVertical == jumpVelocity.y;

        // 점프 거리가 점프최대거리보다 많다면 리턴함. (점프 최대거리 제한)
        if (isRangedJumpVertical && this.recognizedKeyInput == 1)
        {
            // PlayerController.cs의 점프 사운드를 재생하는 메소드를 실행함.
            getPlayerController.SendMessage("PlayAudio");

            startJumpHeight = transform.position.y;
            getRigidbody2D.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }

        if (this.recognizedKeyInput == 2)
        {
            // 버튼을 뗀 시점에 y 포지션 값을 저장함.
            endJumpHeight = transform.position.y;
            // 점프 시작 시점부터 끝난 시점가지의 거리를 구함.
            jumpDistance = endJumpHeight - startJumpHeight;

            // 떨어지는 시점에 점프할수 없게 만들어줌.
            if (jumpVelocity.y < 0)
            {
                return;
            }


            // 점프 거리가 최소 점프 높이보다 낮다면 제한된 점프를 시작함.
            if (jumpDistance < minJumpHeight)
            {
                print("최소 점프컷에 걸림.");
                InvokeStopJump(jumpCutDelay);
            }
            // 어느정도(예: 2.5초) 길게 점프키를 눌렀다면 무조건 끝까지 점프하도록 함.
            else if (jumpDistance > maxJumpTime)
            {
                print("최대 점프컷에 걸림.");
                return;
            }
            // 그렇지 않다면 일반 점프를 시작함.
            else
            {
                print("일반 점프.");
                StopJump();
            }
        }
    }

    // 제한된 점프를 구현하는 코루틴 메소드.
    void InvokeStopJump(float sec)
    {
        StartCoroutine(StopJump(sec));
    }

    // 점프를 멈추는 코루틴 메소드.
    IEnumerator StopJump(float sec)
    {
        yield return new WaitForSeconds(sec);
        StopJump();
    }

    // 점프를 멈추는 메소드.
    void StopJump()
    {
        getRigidbody2D.velocity = new Vector2(Input.GetAxis("MoveX") * moveSpeed, 1);

        // 혹시 몰라 남겨둔 코드.
        // getRigidbody2D.AddForce(Vector2.down * jumpPower);
        // getRigidbody2D.AddForce(Vector2.down * jumpPower * 0.3f, ForceMode2D.Impulse);
    }


    // 좌우 방향을 반환해주는 프로퍼티 (프로퍼티도 메소드의 종류 중 하나이다).
    Vector2 MoveDirection
    {
        get
        {
            // Vector2.right에 인풋값에 따라 양수 또는 음수를 곱함.
            return (Vector2.right * Input.GetAxis("MoveX"));
        }
    }
    
    void OnMove()
    {   
        RaycastHit2D rayHitRight = Physics2D.Raycast(getRigidbody2D.position, Vector2.right, 0.5f, LayerMask.GetMask("Ground"));
        RaycastHit2D rayHitLeft = Physics2D.Raycast(getRigidbody2D.position, Vector2.left, 0.5f, LayerMask.GetMask("Ground"));

        if (rayHitRight.collider != null && MoveDirection.x > 0)
        {
            return;
        }
        
        else if (rayHitLeft.collider != null && MoveDirection.x < 0)
        {
            return;
        }
        
        // 반환받은 MoveDirection Vector2 메소드의 값으로 좌우로 움직이게 해주는 코드.
        getRigidbody2D.velocity = new Vector2(MoveDirection.x * moveSpeed, getRigidbody2D.velocity.y);
    }

    // 안쓰는 코드.
    // private void OnCollisionEnter2D(Collision2D target) 
    // {
    //     if (target.gameObject.layer != LayerMask.NameToLayer("Ground"))
    //     {   
    //         return;
    //     }
        
    //     bool isGroundVertical, isGroundSide;
    //     // foreach 반목문으로 contacts의 (    )를 contact 변수에 반복할 때마다 담아준다.
    //     foreach (ContactPoint2D contact in target.contacts)
    //     {
    //         isGroundVertical = contact.normal.y > 0;
    //         isGroundSide = contact.normal.x != 0 || isGroundVertical;

    //         print($"땅 : {isGroundVertical}" );
    //         print($"사이드 : {isGroundSide}, ({contact.normal.x}, {contact.normal.y})");
    //     }
    // }
}