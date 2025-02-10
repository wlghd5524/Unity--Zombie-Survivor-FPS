using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("플레이어 이동 속도")]
    public float moveSpeed = 5f;

    [Header("Look Settings")]
    [Tooltip("마우스 감도")]
    public float lookSensitivity = 1f;
    [Tooltip("카메라의 수직 회전 제한 (각도)")]
    public float verticalRotationLimit = 80f;

    [Header("Camera Reference")]
    [Tooltip("플레이어 자식에 배치한 FPS 카메라의 Transform")]
    public Transform cameraTransform;

    // 내부에서 저장할 입력 값
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float verticalRotation = 0f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Rigidbody가 외부 힘에 의한 회전을 자동으로 적용하지 않도록 회전 축을 동결합니다.
        rb.freezeRotation = true;

        // 게임 시작 시 커서 숨김 및 잠금
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Player Input 컴포넌트가 Send Messages 모드로 입력 이벤트를 호출할 때 실행되는 메서드들

    /// <summary>
    /// "Move" 액션에 연결된 입력 이벤트
    /// Input Action Asset에서 액션 이름이 "Move"여야 자동으로 호출됩니다.
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
    /// Input Action Asset에서 액션 이름이 "Look"이어야 자동으로 호출됩니다.
    /// </summary>
    public void OnLook(InputAction.CallbackContext context)
    {
        if (context.performed)
            lookInput = context.ReadValue<Vector2>();
        else if (context.canceled)
            lookInput = Vector2.zero;
    }

    // 물리 기반 이동 처리는 FixedUpdate에서 실행합니다.
    private void FixedUpdate()
    {
        HandleMovement();
    }

    // 회전(시점) 처리는 Update에서 실행합니다.
    private void Update()
    {
        HandleLook();
    }

    /// <summary>
    /// 입력된 이동 값에 따라 Rigidbody.MovePosition을 이용하여 이동 처리
    /// </summary>
    private void HandleMovement()
    {
        // 플레이어의 오른쪽과 앞쪽 벡터를 기준으로 이동 방향 계산
        Vector3 moveDirection = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;
        // 이동 벡터 계산 (Time.fixedDeltaTime을 곱해 프레임 간 이동 거리 조절)
        Vector3 movement = moveDirection * moveSpeed * Time.fixedDeltaTime;
        // Rigidbody.MovePosition을 통해 충돌 처리와 함께 이동
        rb.MovePosition(rb.position + movement);
    }

    /// <summary>
    /// 입력된 마우스 값에 따라 플레이어와 카메라 회전 처리
    /// </summary>
    private void HandleLook()
    {
        // 좌우 회전: 플레이어 GameObject 자체를 Y축 기준으로 회전
        transform.Rotate(Vector3.up, lookInput.x * lookSensitivity);

        // 상하 회전: 누적 회전값을 기반으로 카메라의 피치(pitch)를 조절
        verticalRotation -= lookInput.y * lookSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalRotationLimit, verticalRotationLimit);

        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }
    }
}
