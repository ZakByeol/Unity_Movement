using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.iOS;

public class Player_Movement : MonoBehaviour
{
    private Rigidbody player_Rigidbody;
    public Animator player_Animate;
    public GameObject CamHolder;

    public float move_speed;
    private float origin_speed;
    public float rotate_seneitive = 3f;
    public float Mouse_sensitivity,max_force,Jump_force,Sprint_rate;
    public bool isjumping, key_jump, key_Dodge,isdodge;

    private Vector2 move, look;
    private float lookRotation;
    
    
    
    private void Start()
    {
        // 중력 시스템, Rigidbody 컴포넌트 할당
        player_Rigidbody = this.GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서 Invisible;
    }

    private void Awake()
    {
        // 애니메이션 시스템, Animator 컴포넌트 할당
        player_Animate = GetComponentInChildren<Animator>();
        origin_speed = move_speed;
    }

    
    // PlayerInputSystem 을 통한 입력값 할당 이벤트
    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
        
        // 일명 input threshold 문제를 해결하기 위해 데드존을 조정 및 작성
        // 벽에 끼거가 타는 현상은 static 활성과 물체의 Rigidbody(in Kine~,Gravity) 조정
        if (Mathf.Abs(move.x) < 0.5f && Mathf.Abs(move.y) < 0.5f)
        {
            player_Rigidbody.velocity = Vector3.zero;
            player_Animate.SetBool("isWalk", false);
        }
        else
        {
            player_Animate.SetBool("isWalk", true);
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        // LeftShift 누를때는 이동속도를 달리기 비율을 곱하고, 땔때는 초기 속도를 할당
        if (context.started)
        {
            move_speed = move_speed * ( 1 + Sprint_rate / 100);
            player_Animate.SetBool("isSprint", true);
        }
        else if (context.canceled)
        {
            move_speed = origin_speed;
            player_Animate.SetBool("isSprint", false);
        }
    }
        
    private void FixedUpdate()
    {
        Move_Event();
    }

    private void Update()
    {
        key_jump = Input.GetButtonDown("Jump");
        key_Dodge = Input.GetButtonDown("Dodge");
        Jump_Event();
    }

    void Move_Event()
    {
        // 목표 속도 계산
        // 방향성 계산 및 해당 값의 스칼라 곱셈
        Vector3 currentVelocity = player_Rigidbody.velocity;
        Vector3 targetVelocity = new Vector3(move.x, 0, move.y);
        Vector3 rotatecharacter = targetVelocity;
        targetVelocity *= move_speed;
        
        // 이전 물리효과를 반영하지 않는다는 단점을 개선하기 위한 속도 변화량 할당
        // 해당 값을 통한 동적할당 그리고 대각선 이동의 등속을 위한 정규화
        Vector3 VelocityChange = (targetVelocity - currentVelocity);
        VelocityChange = new Vector3(VelocityChange.x, 0, VelocityChange.z).normalized;
        Vector3.ClampMagnitude(VelocityChange, max_force); // 메소드를 사용하기 전, 가하는 힘의 제한
        player_Rigidbody.AddForce(VelocityChange, ForceMode.VelocityChange);
        // AddForce 특성상 질량(mass)을 무시한다.
        if (Mathf.Sign(transform.forward.x) != Mathf.Sign(targetVelocity.x) || Mathf.Sign(transform.forward.z) != Mathf.Sign(targetVelocity.z))
        {
            transform.Rotate(0, 1,0);
        }
        transform.forward = Vector3.Lerp(transform.forward, targetVelocity, rotate_seneitive * Time.deltaTime);
        
        if (key_Dodge && !isjumping && !isdodge) // Dodge키를 눌렀을때, 공중에 있지 않을때, 회피중이 아닐때
        {
            player_Animate.SetTrigger("doDodge");
            move_speed *= 2;
            isdodge = true;
            Task.Delay(100);
            move_speed *= 0.5f;
            isdodge = false;
        }
    }
    void Jump_Event()
    {
        Vector3 JumpForces = Vector3.zero; // 초기 점프 힘 값을 0으로 할당
        if (key_jump && !isjumping && !isdodge) // jump 키를 받고, 공중에 있지 않을때 실행
        {
            JumpForces = Vector3.up * Jump_force;
            player_Rigidbody.AddForce(JumpForces, ForceMode.VelocityChange);
            player_Animate.SetTrigger("doJump");
        }
    }
    

    // 점프의 여부를 체크하는 함수로, Player_GrondCheck 파일에 쓰이기 위해 public으로 작성
    public void SetJumpBool(bool state)
    {
        isjumping = state;
        
        // 주의사항
        // 점프할때, isFalling 과 isJumped가 true가 됨으로, 애니메이션을 입힐때 점프할때는 isjumped 경우만, 떨어질때는 isFalling true/isJumped False;
    }
}


