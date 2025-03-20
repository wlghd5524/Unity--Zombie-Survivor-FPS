using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class WeaponController : MonoBehaviourPunCallbacks
{
    [Header("Animation Settings")]
    public Animator localAnimator;
    public Animator remoteAnimator;

    [Header("Shooting Settings")]
    [Tooltip("1��Ī ī�޶� (����ĳ��Ʈ ����)")]
    public Camera fpsCamera;
    [Tooltip("�� �߻� ��Ÿ�")]
    public float range = 100f;
    [Tooltip("�� �ߴ� ������")]
    public float damage = 50f;

    public int currentAmmo = 7;
    public int maxAmmo = 7;
    private bool isReloading = false;

    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;

    [Header("Sound Settings")]
    public AudioSource audioSource;
    public AudioClip fireAudioClip;
    public AudioClip reloadAudioClip;
    public AudioClip dryFireAudioClip;

    public GameObject menuView_Check;

    void Start()
    {
        fpsCamera = FindFirstObjectByType<Camera>();
        GetComponent<PlayerInput>().camera = fpsCamera;
    }

    void Update()
    {
    }

    /// <summary>
    /// ���ο� Input System�� ����� ��, Player Input ������Ʈ�� "Fire" �׼ǿ� ���� ȣ���ϴ� �޼���.
    /// Send Messages �Ǵ� Unity Events ���� ����Ǿ� �־�� �մϴ�.
    /// </summary>
    public void OnFire(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine)
            return;

        if (isReloading)
            return;

        if (context.performed && currentAmmo > 0)
        {
            Shoot();
        }
        else if (context.performed && currentAmmo <= 0)
        {
            audioSource.clip = dryFireAudioClip;
            audioSource.Play();
        }
    }


    /// <summary>
    /// Player Input ������Ʈ�� "Reload" �׼ǿ� ���� ȣ���ϴ� �޼���.
    /// RŰ�� ���ε��Ͽ� ������ ����� �����մϴ�.
    /// </summary>
    public void OnReload(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine)
            return;

        if (context.performed && !isReloading && currentAmmo < maxAmmo)
        {
            StartCoroutine(ReloadCoroutine());
        }
    }


    /// <summary>
    /// ���� �߻��ϴ� ����
    /// </summary>
    void Shoot()
    {
        audioSource.clip = fireAudioClip;
        audioSource.Play();
        localAnimator.SetTrigger("Fire");
        remoteAnimator.SetTrigger("Fire");

        currentAmmo--;

        // ȭ�� �߾ӿ��� ����ĳ��Ʈ�� �����Ͽ� ��Ȯ�� �浹������ ���մϴ�.
        Vector3 rayOrigin = fpsCamera.transform.position;
        Vector3 rayDirection = fpsCamera.transform.forward;

        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, 1000f))
        {
            targetPoint = hit.point;  // �浹 ������ �ִٸ�, �װ��� Ÿ������ ����
        }
        else
        {
            // �ƹ��͵� �浹���� �ʾҴٸ� �� ������ Ÿ������ ����
            targetPoint = rayOrigin + rayDirection * 100f;
        }

        // ���� �Ѿ��� ī�޶󿡼� �߻�
        Vector3 shootOrigin = bulletSpawnPoint.position;
        Vector3 shootDirection = (targetPoint - bulletSpawnPoint.position).normalized;

        GameObject projectile = PhotonNetwork.Instantiate("Bullet", shootOrigin, Quaternion.LookRotation(shootDirection));
        Bullet bullet = projectile.GetComponent<Bullet>();
        bullet.weaponController = this;
        bullet.shootDirection = shootDirection;
        bullet.Init();
    }

    /// <summary>
    /// ������ �ڷ�ƾ: ������ �ִϸ��̼��� ����� �� ���� �ð� ����ϰ�, �Ѿ� ���� �����մϴ�.
    /// </summary>
    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        audioSource.clip = reloadAudioClip;
        audioSource.Play();
        localAnimator.SetBool("IsReloading", true);
        remoteAnimator.SetBool("IsReloading", true);
        // �ִϸ����Ͱ� "Reload" �����̸�, �� ����� ���� ������ ���
        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo stateInfo = localAnimator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName("Reload") && stateInfo.normalizedTime >= 1.0f;
        });
        localAnimator.SetBool("IsReloading", false);
        remoteAnimator.SetBool("IsReloading", false);
        currentAmmo = maxAmmo;
        isReloading = false;
    }
}
