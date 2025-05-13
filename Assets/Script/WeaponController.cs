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
    [Tooltip("1인칭 카메라 (레이캐스트 기준)")]
    public Camera fpsCamera;
    [Tooltip("총 발사 사거리")]
    public float range = 100f;
    [Tooltip("한 발당 데미지")]
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

    private PlayerView playerView;

    void Start()
    {
        fpsCamera = FindFirstObjectByType<Camera>();
        GetComponent<PlayerInput>().camera = fpsCamera;
        playerView = GameObject.Find("PlayerView").GetComponent<PlayerView>();
    }



    /// <summary>
    /// 새로운 Input System을 사용할 때, Player Input 컴포넌트가 "Fire" 액션에 대해 호출하는 메서드.
    /// Send Messages 또는 Unity Events 모드로 연결되어 있어야 합니다.
    /// </summary>
    public void OnFire(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine || !remoteAnimator.GetBool("IsAlive"))
            return;

        if (isReloading)
            return;

        if (playerView.menu.activeSelf)
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
    /// Player Input 컴포넌트가 "Reload" 액션에 대해 호출하는 메서드.
    /// R키에 바인딩하여 재장전 기능을 구현합니다.
    /// </summary>
    public void OnReload(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine || !remoteAnimator.GetBool("IsAlive"))
            return;

        if (playerView.menu.activeSelf)
            return;

        if (context.performed && !isReloading && currentAmmo < maxAmmo)
        {
            StartCoroutine(ReloadCoroutine());

        }
    }


    /// <summary>
    /// 총을 발사하는 로직
    /// </summary>
    void Shoot()
    {
        audioSource.clip = fireAudioClip;
        audioSource.Play();
        localAnimator.SetTrigger("Fire");
        remoteAnimator.SetTrigger("Fire");

        currentAmmo--;
        playerView.UpdateAmmoUI(currentAmmo, maxAmmo);

        // 화면 중앙에서 레이캐스트를 수행하여 정확한 충돌지점을 구합니다.
        Vector3 rayOrigin = fpsCamera.transform.position;
        Vector3 rayDirection = fpsCamera.transform.forward;

        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, 1000f))
        {
            targetPoint = hit.point;  // 충돌 지점이 있다면, 그곳을 타겟으로 지정
        }
        else
        {
            // 아무것도 충돌하지 않았다면 먼 지점을 타겟으로 지정
            targetPoint = rayOrigin + rayDirection * 100f;
        }

        // 실제 총알은 카메라에서 발사
        Vector3 shootOrigin = bulletSpawnPoint.position;
        Vector3 shootDirection = (targetPoint - bulletSpawnPoint.position).normalized;

        GameObject projectile = PhotonNetwork.Instantiate("Bullet", shootOrigin, Quaternion.LookRotation(shootDirection));
        Bullet bullet = projectile.GetComponent<Bullet>();
        bullet.weaponController = this;
        bullet.shootDirection = shootDirection;
        bullet.Init();
    }

    /// <summary>
    /// 재장전 코루틴: 재장전 애니메이션을 재생한 후 일정 시간 대기하고, 총알 수를 리셋합니다.
    /// </summary>
    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        audioSource.clip = reloadAudioClip;
        audioSource.Play();
        localAnimator.SetBool("IsReloading", true);
        remoteAnimator.SetBool("IsReloading", true);
        // 애니메이터가 "Reload" 상태이며, 그 재생이 끝날 때까지 대기
        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo stateInfo = localAnimator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName("Reload") && stateInfo.normalizedTime >= 1.0f;
        });
        
        localAnimator.SetBool("IsReloading", false);
        remoteAnimator.SetBool("IsReloading", false);
        currentAmmo = maxAmmo;
        playerView.ResetAmmoUI(maxAmmo);
        isReloading = false;
    }
}
