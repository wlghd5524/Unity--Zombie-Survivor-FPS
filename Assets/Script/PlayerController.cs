using SimpleFPS;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("플레이어 이동 속도")]
    public float moveSpeed = 5f;

    [Header("Jump Settings")]
    [Tooltip("점프 힘 (양수 값)")]
    public float jumpForce = 50f;
    [Tooltip("바닥 판정을 위한 레이캐스트 거리")]
    public float groundCheckDistance = 0.2f;

    [Header("Look Settings")]
    [Tooltip("마우스 감도")]
    public float lookSensitivity = 1f;
    [Tooltip("카메라의 수직 회전 제한 (각도)")]
    public float verticalRotationLimit = 80f;

    [Header("Camera Reference")]
    [Tooltip("플레이어 자식에 배치한 FPS 카메라의 Transform")]
    public Transform cameraTransform;

    [SerializeField]
    GameObject playerView;
    PlayerView pv;

    // 내부에서 저장할 입력 값
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float verticalRotation = 0f;

    private Rigidbody rb;
    private Animator animator;
    
    // 체력 
    public float current_hp = 100.0f;
    public float max_hp = 100.0f;
    public float min_hp = 0.0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Rigidbody가 외부 힘에 의한 회전을 자동으로 적용하지 않도록 회전 축을 동결합니다.
        rb.freezeRotation = true;

        animator = GetComponent<Animator>();

        // 게임 시작 시 커서 숨김 및 잠금
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerView == null)
            GameObject.Find("PlayerView");

        pv = playerView.GetComponent<PlayerView>();

    }

    // Player Input 컴포넌트가 Send Messages 혹은 Unity Events로 호출할 때 실행되는 메서드들

    /// <summary>
    /// "Move" 액션에 연결된 입력 이벤트
    /// </summary>
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
            moveInput = context.ReadValue<Vector2>();
        else if (context.canceled)
            moveInput = Vector2.zero;
    }

    /// <summary>
    /// "Look" 액션에 연결된 입력 이벤트
    /// </summary>
    public void OnLook(InputAction.CallbackContext context)
    {
        if (context.performed)
            lookInput = context.ReadValue<Vector2>();
        else if (context.canceled)
            lookInput = Vector2.zero;
    }

    /// <summary>
    /// "Jump" 액션에 연결된 입력 이벤트
    /// </summary>
    public void OnJump(InputAction.CallbackContext context)
    {
        // 점프 입력이 performed 상태이고, 바닥에 닿아 있을 때만 점프 처리
        if (context.performed && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jump");
            
        }
    }

    // 물리 기반 이동 및 중력 처리는 FixedUpdate에서 실행합니다.
    private void FixedUpdate()
    {
        HandleMovement();
        UpdateAnimation();
    }

    // 회전(시점) 처리는 Update에서 실행합니다.
    private void Update()
    {
        HandleLook();
    }

    /// <summary>
    /// 입력된 이동 값과 수직 속도를 반영하여 이동 처리
    /// </summary>
    private void HandleMovement()
    {
        // 플레이어의 오른쪽과 앞쪽 벡터를 기준으로 수평 이동 방향 계산
        Vector3 moveDirection = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;
        Vector3 horizontalMovement = moveDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + horizontalMovement);

        animator.SetFloat("MoveX", moveDirection.x, 0.05f, Time.deltaTime);
        animator.SetFloat("MoveZ", moveDirection.z, 0.05f, Time.deltaTime);
        animator.SetFloat("MoveSpeed", moveSpeed);
    }

    /// <summary>
    /// 입력된 마우스 값에 따라 플레이어와 카메라 회전 처리
    /// </summary>
    private void HandleLook()
    {
        // 좌우 회전: 플레이어 GameObject 자체를 Y축 기준으로 회전
        transform.Rotate(Vector3.up, lookInput.x * lookSensitivity);

        // 상하 회전: 누적 회전값을 기반으로 카메라의 피치(pitch) 조절
        verticalRotation -= lookInput.y * lookSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalRotationLimit, verticalRotationLimit);

        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }

        animator.SetFloat("Look", -verticalRotation / 90f);
    }

    private void UpdateAnimation()
    {
        // Set animation parameters.
        animator.SetFloat("LocomotionTime", Time.time * 2f);
        animator.SetBool("IsGrounded", IsGrounded());
    }

    /// <summary>
    /// 바닥 판정을 위해 플레이어 아래로 레이캐스트를 실행
    /// </summary>
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance);
    }

    /// <summary>
    /// 체력회복 함수
    /// </summary>
    public void Heal()
    {
        float before_hp = current_hp;

        if (current_hp < 50)
            current_hp += 50;

        else
            current_hp = 100;

        pv.Heal(current_hp, before_hp, max_hp);
    }
    /// <summary>
    /// 대미지 함수
    /// </summary>
    /// <param name="damage"></param>
    public void Damage(float damage)
    {
        if (current_hp > max_hp)
            return;

        current_hp -= damage;

        if (current_hp <= min_hp)
            Dead();

        pv.Damage(current_hp);
    }

    /// <summary>
    /// 사망함수
    /// </summary>
    private void Dead()
    {
        pv.Dead();
        //게임 종료 코드 작성
    }
}
