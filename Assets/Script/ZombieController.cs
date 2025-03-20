using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    private enum ZombieStates           //���� ����
    {
        Attack,         //����
        Die,            //����
        Walking         //�ȱ�
    }

    ZombieStates state;      //���� ���� ����

    private PlayerController player = null;
    private PlayerView playerView = null;
    private Animator animator;
    private NavMeshAgent agent;
    private float attack_Damage = 25.0f;         //���� ���� ������
    public float distance = 0.0f;                //������� �Ÿ�
    public float health = 100.0f;                //���� ü��
    public float fieldOfViewAngle = 60f; // ������ �þ߰� (��: 60��)

    private void Start()
    {
        //�÷��̾� ã��
        player = GameObject.Find("GameManager").GetComponent<PlayerController>();
        playerView = GameObject.Find("PlayerView").GetComponent<PlayerView>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // ����� ������ �������� ����
        if (player == null) return;
        if (agent == null && agent.isStopped) return; 
        if (playerView == null) return;

        // �÷��̾���� �Ÿ� ���
        distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance > 2.0f)
        {
            state = ZombieStates.Walking;       //�ȱ�
            Lotation_Zombie();
            agent.SetDestination(player.transform.position);
        }

        else
        {
            state = ZombieStates.Attack;        //����
        }
        
        //���� �÷��̾ �ٶ󺸰� ���� ������
        if(!IsLookingAtPlayer())
        {
            Lotation_Zombie();
        }

        if (health <=0)
        {
            state = ZombieStates.Die;
            Rigidbody rd = GetComponent<Rigidbody>();
            rd.isKinematic = true;
            agent.isStopped = true;
        }
    
        Animation();
    }

    public void Attack()           // ���� �Լ�
    {
        if (playerView == null)
            return;

        playerView.Damage(attack_Damage);
    }

    public void Die()
    {  
        Destroy(gameObject);
    }

    bool IsLookingAtPlayer()
    {
        Vector3 toPlayer = (player.gameObject.transform.position - transform.position).normalized; // ���� �� �÷��̾� ����
        float dotProduct = Vector3.Dot(transform.forward, toPlayer); // ��Ʈ ���δ�Ʈ ���

        float viewThreshold = Mathf.Cos(fieldOfViewAngle * 0.5f * Mathf.Deg2Rad); // ������ �������� ��ȯ �� �ڻ��� ��

        return dotProduct >= viewThreshold; // Ư�� �� �̻��̸� �÷��̾ �ٶ󺸴� ��
    }

    private void Lotation_Zombie()
    {
        // �ڿ������� �÷��̾� �ٶ󺸱�
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, agent.angularSpeed * Time.deltaTime);
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
