using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    private PlayerController player = null;
    private CharacterHpBar HpBar = null;
    private Animator animator;
    private NavMeshAgent agent;
    public float rotationSpeed = 5f; // 회전 속도
    public float speed = 0.5f;
    public float distance = 0.0f;
    private bool check = false;             //플레이어와의 거리
    private int attack_Damage = 25;         //좀비 공격 데미지

    private void Start()
    {
        //플레이어 찾기
        player = GameObject.Find("Player Character").GetComponent<PlayerController>();
        HpBar = player.GetComponent<CharacterHpBar>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // 대상이 없으면 실행하지 않음
        if (player == null) return;
        if (agent == null) return; 
        if (HpBar == null) return;

        // 플레이어와의 거리 계산
        distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance > 2.0f)
        {
            check = true;
            // 자연스럽게 플레이어 바라보기
            Vector3 direction = (player.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
            agent.SetDestination(player.transform.position);
        }

        else
        {
            check = false;
        }
            
        Animation();
    }

    public void Attack()           // 공격 함수
    {
        if (HpBar == null)
            return;

        HpBar.Damage(attack_Damage);
    }

    void Animation()
    {
        if (animator == null) return;

        if (check == false)
            animator.Play("Z_Attack");
       
        else
            animator.Play("Z_Walk_InPlace");
    }
}
