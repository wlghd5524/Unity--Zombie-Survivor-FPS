using UnityEngine;

public class ZombieController : MonoBehaviour
{
    PlayerController player = null;
    float distance;
    Animator animator;
    public float rotationSpeed = 5f; // 회전 속도
    float speed = 0.5f;
    bool check = false;

    private void Start()
    {
        //플레이어 찾기
        player = GameObject.Find("Player Character").GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 대상이 없으면 실행하지 않음
        if (player == null) return;

        // 플레이어와의 거리 계산
        distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance > 2.0f)
        {
            check = true;
            // 플레이어 방향으로 이동
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

            // 자연스럽게 플레이어 바라보기
            Vector3 direction = (player.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

        }

        else
            check = false;

        Animation();
    }

    void Animation()
    {
        if(animator == null) return;

        if (check == false)
            animator.Play("Z_Attack");
        else
            animator.Play("Z_Walk_InPlace");
    }
}
