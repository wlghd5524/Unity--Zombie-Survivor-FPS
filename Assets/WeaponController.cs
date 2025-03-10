using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    [Header("Animation Settings")]
    public Animator animator;
    private int idleStateHash;

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
    /// ���ο� Input System�� ����� ��, Player Input ������Ʈ�� "Fire" �׼ǿ� ���� ȣ���ϴ� �޼���.
    /// Send Messages �Ǵ� Unity Events ���� ����Ǿ� �־�� �մϴ�.
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
    /// Player Input ������Ʈ�� "Reload" �׼ǿ� ���� ȣ���ϴ� �޼���.
    /// RŰ�� ���ε��Ͽ� ������ ����� �����մϴ�.
    /// </summary>
    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.performed && !isReloading && currentAmmo < maxAmmo)
        {
            StartCoroutine(ReloadCoroutine());
        }
    }


    /// <summary>
    /// Raycast�� �̿��Ͽ� ���� �߻��ϴ� ����
    /// </summary>
    void Shoot()
    {
        audioSource.clip = fireAudioClip;
        audioSource.Play();
        animator.SetTrigger("Fire");

        currentAmmo--;

        // fpsCamera�� ��ġ�� ������ �������� ����ĳ��Ʈ ����
        RaycastHit hit;
        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, range))
        {
            Debug.Log("Hit: " + hit.transform.name);

            // (�ɼ�) ������ ����: �浹�� ������Ʈ�� ZombieController ������Ʈ�� �ִٸ� ������ ó��
            ZombieController target = hit.transform.GetComponent<ZombieController>();
            if (target != null)
            {
                target.health -= damage;
                Debug.Log($"{target.name}�� ���� ü�� : {target.health}");
            }
        }
    }

    /// <summary>
    /// ������ �ڷ�ƾ: ������ �ִϸ��̼��� ����� �� ���� �ð� ����ϰ�, �Ѿ� ���� �����մϴ�.
    /// </summary>
    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        audioSource.clip = reloadAudioClip;
        audioSource.Play();
        animator.SetBool("IsReloading", true);
        // �ִϸ����Ͱ� "Reload" �����̸�, �� ����� ���� ������ ���
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
