using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    [Header("Animation Settings")]
    public Animator animator;
    private int idleStateHash;

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

    [Header("Sound Settings")]
    public AudioSource audioSource;
    public AudioClip fireAudioClip;
    public AudioClip reloadAudioClip;
    public AudioClip dryFireAudioClip;

    void Start()
    {
    }

    void Update()
    {
    }

    /// <summary>
    /// 새로운 Input System을 사용할 때, Player Input 컴포넌트가 "Fire" 액션에 대해 호출하는 메서드.
    /// Send Messages 또는 Unity Events 모드로 연결되어 있어야 합니다.
    /// </summary>
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed && currentAmmo > 0)
        {
            Shoot();
        }
        else if(context.performed && currentAmmo <= 0)
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
        if (context.performed && !isReloading && currentAmmo < maxAmmo)
        {
            StartCoroutine(ReloadCoroutine());
        }
    }


    /// <summary>
    /// Raycast를 이용하여 총을 발사하는 로직
    /// </summary>
    void Shoot()
    {
        audioSource.clip = fireAudioClip;
        audioSource.Play();
        animator.SetTrigger("Fire");

        currentAmmo--;

        // fpsCamera의 위치와 전방을 기준으로 레이캐스트 수행
        RaycastHit hit;
        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);

            // (옵션) 데미지 적용: 충돌한 오브젝트에 ZombieController 컴포넌트가 있다면 데미지 처리
            ZombieController target = hit.transform.GetComponent<ZombieController>();
            if (target != null)
            {
                target.health -= damage;
                Debug.Log($"{target.name}의 남은 체력 : {target.health}");
            }
        }
    }

    /// <summary>
    /// 재장전 코루틴: 재장전 애니메이션을 재생한 후 일정 시간 대기하고, 총알 수를 리셋합니다.
    /// </summary>
    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        audioSource.clip = reloadAudioClip;
        audioSource.Play();
        animator.SetBool("IsReloading", true);
        // 애니메이터가 "Reload" 상태이며, 그 재생이 끝날 때까지 대기
        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName("Reload") && stateInfo.normalizedTime >= 1.0f;
        });
        animator.SetBool("IsReloading", false);
        currentAmmo = maxAmmo;
        isReloading = false;
    }
}
