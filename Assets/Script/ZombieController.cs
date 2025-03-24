using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviourPunCallbacks
{
    private enum ZombieStates
    {
        Attack,
        Die,
        Walking
    }

    ZombieStates state;

    private GameObject targetPlayer = null; // 가장 가까운 플레이어
    private Animator animator;
    private NavMeshAgent agent;
    private float attack_Damage = 25.0f;
    public float distance = 0.0f;
    public float health = 100.0f;
    public float fieldOfViewAngle = 60f;
    private bool isDead = false;
    private Vector3 lastTargetPosition;
    private PlayerController playerController;

    private void Awake()
    {
        photonView.OwnershipTransfer = OwnershipOption.Takeover; // ✅ 자동 소유권 이전 설정
    }
    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        lastTargetPosition = transform.position;
        FindClosestPlayer(); // 최초 실행 시 가장 가까운 플레이어 찾기
    }

    void Update()
    {
        if (isDead) return;

        if (targetPlayer == null)
        {
           
            return;
        }

        FindClosestPlayer(); // 추적 대상이 없으면 다시 찾기

        distance = Vector3.Distance(transform.position, targetPlayer.transform.position);

        if (distance > 2.0f)
        {
            state = ZombieStates.Walking;

            if (Vector3.Distance(lastTargetPosition, targetPlayer.transform.position) > 0.5f)
            {
                lastTargetPosition = targetPlayer.transform.position;
                RotateTowardsPlayer();
                agent.SetDestination(lastTargetPosition);
            }
        }
        else
        {
            state = ZombieStates.Attack;
        }

        if (!IsLookingAtPlayer())
        {
            RotateTowardsPlayer();
        }

        Animation();
    }

    void FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float closestDistance = Mathf.Infinity;
        GameObject closestPlayer = null;

        foreach (GameObject player in players)
        {
            float playerDistance = Vector3.Distance(transform.position, player.transform.position);
            if (playerDistance < closestDistance)
            {
                closestDistance = playerDistance;
                closestPlayer = player;
            }
        }

        targetPlayer = closestPlayer;
    }

    public void Attack()
    {
        if (targetPlayer == null) return;
        playerController = targetPlayer.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.Damage(attack_Damage);
        }
    }

    private void Die()
    {
        if (isDead) return;

        state = ZombieStates.Die;
        isDead = true;

        Rigidbody rd = GetComponent<Rigidbody>();
        if (rd != null) rd.isKinematic = true;
        if (agent != null) agent.isStopped = true;

        Animation();
        StartCoroutine(DestroyAfterAnimation());
    }

    IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        RequestDestroy();
    }

    bool IsLookingAtPlayer()
    {
        if (targetPlayer == null) return false;

        Vector3 toPlayer = (targetPlayer.transform.position - transform.position).normalized;
        float dotProduct = Vector3.Dot(transform.forward, toPlayer);
        float viewThreshold = Mathf.Cos(fieldOfViewAngle * 0.5f * Mathf.Deg2Rad);
        return dotProduct >= viewThreshold;
    }

    private void RotateTowardsPlayer()
    {
        if (targetPlayer == null) return;

        Vector3 direction = (targetPlayer.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, agent.angularSpeed * Time.deltaTime);
    }

    public void Damage(float damage)
    {
        health -= damage;
    }

    private void Animation()
    {
        if (animator == null) return;

        switch (state)
        {
            case ZombieStates.Attack:
                animator.Play("Z_Attack");
                break;

            case ZombieStates.Walking:
                animator.Play("Z_Walk_InPlace");
                break;

            case ZombieStates.Die:
                animator.Play("Z_FallingForward");
                break;
        }
    }

    [PunRPC]
    void DestroyZombie()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void RequestDestroy()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            // 소유권을 변경하고 삭제
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);

            // 또는 RPC 방식 사용
            photonView.RPC("DestroyZombie", RpcTarget.MasterClient);
        }
    }
    [PunRPC]
    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("남은 좀비 체력: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

}
