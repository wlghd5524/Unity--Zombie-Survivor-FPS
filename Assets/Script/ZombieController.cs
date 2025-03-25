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
    public ItemSO itemSO;
    public ZombieSO zombie;
    private GameObject targetPlayer = null; // 가장 가까운 플레이어
    private Animator animator;
    private NavMeshAgent agent;
    public float distance = 0.0f;
    private Vector3 lastTargetPosition;
    private PlayerController playerController;

    public int health = 100;
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

            if (distance > 1.5f)
            {
                state = ZombieStates.Walking;
                //animator.SetBool("IsWalking", true);
                photonView.RPC("SetAnimation", RpcTarget.All, "IsWalking", true);

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
                //animator.SetTrigger("Attack");
                photonView.RPC("SetAnimationTrigger", RpcTarget.All, "Attack");
            }

            if (!IsLookingAtPlayer())
            {
                RotateTowardsPlayer();
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

    public void Attack()
    {
        if (targetPlayer == null) return;
        playerController = targetPlayer.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.Damage(zombie.attack_Damage);
        }
    }

    private void Die()
    {
        if (state == ZombieStates.Die) return;
        //animator.SetBool("Dead", true);
        photonView.RPC("SetAnimationTrigger", RpcTarget.All, "Dead");
        state = ZombieStates.Die;
        Rigidbody rd = GetComponent<Rigidbody>();
        if (rd != null) rd.isKinematic = true;
        if (agent != null) agent.isStopped = true;

        StartCoroutine(DestroyAfterAnimation());
    }

    private string Random_Item()
    {
        int randomValue = Random.Range(0, 101);
        Vector3 spawnPosition = transform.position + new Vector3(0, 1.0f, 0);

        if (randomValue < 90)
        {
            int randomIndex = Random.Range(0, itemSO.dropItem.Length);
            GameObject.Instantiate(itemSO.dropItem[randomIndex], spawnPosition, Quaternion.identity);
        }
        return null;
    }
    IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        Random_Item();
        PhotonNetwork.Destroy(gameObject);
    }

    bool IsLookingAtPlayer()
    {
        if (targetPlayer == null) return false;

        Vector3 toPlayer = (targetPlayer.transform.position - transform.position).normalized;
        float dotProduct = Vector3.Dot(transform.forward, toPlayer);
        float viewThreshold = Mathf.Cos(zombie.fieldOfViewAngle * 0.5f * Mathf.Deg2Rad);
        return dotProduct >= viewThreshold;
    }

    private void RotateTowardsPlayer()
    {
        if (targetPlayer == null) return;

        Vector3 direction = (targetPlayer.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, agent.angularSpeed * Time.deltaTime);
    }

    public void Damage(int damage, PlayerController player = null)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        health -= damage;
        //player.pv.hitUi.Play();

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
            health = (int)stream.ReceiveNext();
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
