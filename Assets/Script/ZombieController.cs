using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviourPunCallbacks, IPunObservable
{
    private enum ZombieStates
    {
        Attack,
        Die,
        Walking
    }

    ZombieStates state;
    private readonly string ATTACK_STATE_NAME = "Z_Attack";
    private bool hasDealtDamage = false; // 데미지 판정 여부를 체크하는 변수 추가

    private GameObject targetPlayer = null; // 가장 가까운 플레이어
    private Animator animator;
    private NavMeshAgent agent;
    private float attack_Damage = 25.0f;
    public float distance = 0.0f;
    public float health = 100.0f;
    public float fieldOfViewAngle = 60f;
    private Vector3 lastTargetPosition;
    private PlayerController playerController;

    //네트워크 동기화를 위한 변수들
    private Vector3 networkPosition;
    private Quaternion networkRotation;

    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        lastTargetPosition = transform.position;

        if (PhotonNetwork.IsMasterClient)
        {
            FindClosestPlayer(); // 최초 실행 시 가장 가까운 플레이어 찾기
        }
    }

    void Update()
    {
        if (state == ZombieStates.Die) return;

        if (targetPlayer == null)
        {
            return;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            FindClosestPlayer(); // 추적 대상이 없으면 다시 찾기

            distance = Vector3.Distance(transform.position, targetPlayer.transform.position);

            if (distance > 2.0f)
            {
                state = ZombieStates.Walking;
                photonView.RPC("SetAnimation", RpcTarget.All, "IsWalking", true);
                agent.isStopped = false;
                hasDealtDamage = false; // Walking 상태로 전환 시 데미지 판정 초기화

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
                photonView.RPC("SetAnimationTrigger", RpcTarget.All, "Attack");
                agent.isStopped = true; // 공격 중 이동 정지
            }

            if (!IsLookingAtPlayer())
            {
                RotateTowardsPlayer();
            }

            // 공격 상태일 때 타이밍 체크
            if (state == ZombieStates.Attack)
            {
                CheckAttackHit();
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 10);
        }
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

    private void CheckAttackHit()
    {
        if (animator == null) return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // 공격 애니메이션이 끝났는지 체크
        if (stateInfo.IsName(ATTACK_STATE_NAME) && stateInfo.normalizedTime >= 1.0f)
        {
            state = ZombieStates.Walking;
            agent.isStopped = false;
            hasDealtDamage = false; // 다음 공격을 위해 초기화
            return;
        }

        // 아직 데미지를 주지 않았고, 애니메이션이 40~60% 구간일 때만 공격
        if (!hasDealtDamage && 
            stateInfo.IsName(ATTACK_STATE_NAME) && 
            stateInfo.normalizedTime >= 0.4f && 
            stateInfo.normalizedTime <= 0.6f)
        {
            Attack();
            hasDealtDamage = true; // 데미지를 준 후 플래그 설정
        }
    }

    public void Attack()
    {
        if (targetPlayer == null) return;
        
        // 실제 공격 시점에서 거리를 다시 체크
        float currentDistance = Vector3.Distance(transform.position, targetPlayer.transform.position);
        if (currentDistance > 2.0f) return; // 공격 범위를 벗어났다면 공격하지 않음
        
        playerController = targetPlayer.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.Damage(attack_Damage);
        }
    }

    private void Die()
    {
        if (state == ZombieStates.Die) return;
        photonView.RPC("SetAnimationTrigger", RpcTarget.All, "Dead");
        state = ZombieStates.Die;
        Rigidbody rd = GetComponent<Rigidbody>();
        if (rd != null) rd.isKinematic = true;
        if (agent != null) agent.isStopped = true;
        
        // Medkit 생성 위치 설정 (좀비 위치에서 y축으로 1 위)
        Vector3 spawnPosition = transform.position + Vector3.up;
        PhotonNetwork.Instantiate("Medkit", spawnPosition, transform.rotation);
        
        StartCoroutine(DestroyAfterAnimation());
    }

    IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        PhotonNetwork.Destroy(gameObject);
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
        if (!PhotonNetwork.IsMasterClient) return;
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    // 네트워크 동기화를 위한 메서드
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 마스터 클라이언트에서 데이터 전송
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(health);
        }
        else
        {
            // 클라이언트에서 데이터 수신
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            health = (float)stream.ReceiveNext();
        }
    }

    [PunRPC]
    void SetAnimation(string parameter, bool value)
    {
        if(animator == null)
        {
            animator = GetComponent<Animator>();
        }
        animator.SetBool(parameter, value);
    }

    [PunRPC]
    void SetAnimationTrigger(string parameter)
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        animator.SetTrigger(parameter);
    }
}
