using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [Header("Movement Settings")]
    [Tooltip("�÷��̾� �̵� �ӵ�")]
    public float moveSpeed = 5f;

    [Header("Jump Settings")]
    [Tooltip("���� �� (��� ��)")]
    public float jumpForce = 50f;
    [Tooltip("�ٴ� ������ ���� ����ĳ��Ʈ �Ÿ�")]
    public float groundCheckDistance = 0.2f;

    [Header("Look Settings")]
    [Tooltip("���콺 ����")]
    public float lookSensitivity = 1f;
    [Tooltip("ī�޶��� ���� ȸ�� ���� (����)")]
    public float verticalRotationLimit = 80f;

    [Header("Camera Reference")]
    [Tooltip("�÷��̾� �ڽĿ� ��ġ�� FPS ī�޶��� Transform")]
    public Transform cameraTransform;

    // ���ο��� ������ �Է� ��
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float verticalRotation = 0f;

    private Rigidbody rb;
    private Animator animator;

    // ü�� 
    public float current_hp = 100.0f;
    public float max_hp = 100.0f;
    public float min_hp = 0.0f;

    GameObject playerView;
    PlayerView pv;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Rigidbody�� �ܺ� ���� ���� ȸ���� �ڵ����� �������� �ʵ��� ȸ�� ���� �����մϴ�.
        rb.freezeRotation = true;

        animator = GetComponent<Animator>();

        if (playerView == null)
            playerView = GameObject.Find("PlayerView");

        pv = playerView.GetComponent<PlayerView>();

        if (photonView.IsMine)
        {
            // ���� ���� �� Ŀ�� ���� �� ���
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            if (cameraTransform != null)
            {
                cameraTransform.gameObject.SetActive(false);

                // ���� �÷��̾��� ���, LocalPlayer ���̾ RemotePlayer ���̾�� ����
                ChangeLayerRecursively(transform, LayerMask.NameToLayer("LocalPlayer"), LayerMask.NameToLayer("RemotePlayer"));
            }
        }
    }

    // Player Input ������Ʈ�� Send Messages Ȥ�� Unity Events�� ȣ���� �� ����Ǵ� �޼����

    /// <summary>
    /// "Move" �׼ǿ� ����� �Է� �̺�Ʈ
    /// </summary>
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine)
            return;
        if (context.performed)
            moveInput = context.ReadValue<Vector2>();
        else if (context.canceled)
            moveInput = Vector2.zero;
    }

    /// <summary>
    /// "Look" �׼ǿ� ����� �Է� �̺�Ʈ
    /// </summary>
    public void OnLook(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine)
            return;
        if (context.performed)
            lookInput = context.ReadValue<Vector2>();
        else if (context.canceled)
            lookInput = Vector2.zero;
    }

    /// <summary>
    /// "Jump" �׼ǿ� ����� �Է� �̺�Ʈ
    /// </summary>
    public void OnJump(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine)
            return;
        // ���� �Է��� performed �����̰�, �ٴڿ� ��� ���� ���� ���� ó��
        if (context.performed && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jump");

        }
    }
    public void OnCanceld(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine)
            return;

        if(context.canceled)
            pv.Active_Menu();

    }
    // ���� ��� �̵� �� �߷� ó���� FixedUpdate���� �����մϴ�.
    private void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;
        HandleMovement();
        UpdateAnimation();
    }

    // ȸ��(����) ó���� Update���� �����մϴ�.
    private void Update()
    {
        if (!photonView.IsMine)
            return;
        HandleLook();
    }

    /// <summary>
    /// �Էµ� �̵� ���� ���� �ӵ��� �ݿ��Ͽ� �̵� ó��
    /// </summary>
    private void HandleMovement()
    {
        // �÷��̾��� �����ʰ� ���� ���͸� �������� ���� �̵� ���� ���
        Vector3 moveDirection = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;
        Vector3 horizontalMovement = moveDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + horizontalMovement);

        animator.SetFloat("MoveX", moveDirection.x, 0.05f, Time.deltaTime);
        animator.SetFloat("MoveZ", moveDirection.z, 0.05f, Time.deltaTime);
        animator.SetFloat("MoveSpeed", moveSpeed);
    }

    /// <summary>
    /// �Էµ� ���콺 ���� ���� �÷��̾�� ī�޶� ȸ�� ó��
    /// </summary>
    private void HandleLook()
    {
        // �¿� ȸ��: �÷��̾� GameObject ��ü�� Y�� �������� ȸ��
        transform.Rotate(Vector3.up, lookInput.x * lookSensitivity);

        // ���� ȸ��: ���� ȸ������ ������� ī�޶��� ��ġ(pitch) ����
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
    /// �ٴ� ������ ���� �÷��̾� �Ʒ��� ����ĳ��Ʈ�� ����
    /// </summary>
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance);
    }

    void ChangeLayerRecursively(Transform obj, int targetLayer, int newLayer)
    {
        if (obj.gameObject.layer == targetLayer)
        {
            obj.gameObject.layer = newLayer;
        }
        foreach (Transform child in obj)
        {
            ChangeLayerRecursively(child, targetLayer, newLayer);
        }
    }
    /// <summary>
    /// ȸ�����
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
    /// ��������
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
    /// ����Լ�
    /// </summary>
    private void Dead()
    {
        pv.Dead();
    }
}
