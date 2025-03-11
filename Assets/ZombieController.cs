using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    private enum ZombieStates           //좀비 상태
    {
        Attack,         //공격
        Die,            //죽음
        Walking         //걷기
    }

    ZombieStates state;      //좀비 상태 결정

    private PlayerController player = null;
    private PlayerView playerView = null;
    private Animator animator;
    private NavMeshAgent agent;
    private float attack_Damage = 25.0f;         //좀비 공격 데미지
    public float distance = 0.0f;                //좀비와의 거리
    public float health = 100.0f;                //좀비 체력
    

    private void Start()
    {
        //플레이어 찾기
        player = GameObject.Find("Player Character").GetComponent<PlayerController>();
        playerView = GameObject.Find("PlayerView").GetComponent<PlayerView>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // 대상이 없으면 실행하지 않음
        if (player == null) return;
        if (agent == null) return; 
        if (playerView == null) return;

        // 플레이어와의 거리 계산
        distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance > 2.0f)
        {
            state = ZombieStates.Walking;       //걷기
            // 자연스럽게 플레이어 바라보기
            Vector3 direction = (player.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, agent.angularSpeed * Time.deltaTime);
            agent.SetDestination(player.transform.position);
        }

        else
        {
            state = ZombieStates.Attack;        //공격
        }

        if (health <=0)
        {
            state = ZombieStates.Die;
            agent.isStopped = true;
        }
    
        Animation();
    }

    public void Attack()           // 공격 함수
    {
        if (playerView == null)
            return;

        playerView.Damage(attack_Damage);
    }

    public void Die()
    {
        
        Destroy(gameObject);
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
}
