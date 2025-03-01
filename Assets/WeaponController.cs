using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Animator animator;
    private int idleStateHash;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        idleStateHash = Animator.StringToHash("Idle");
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if(stateInfo.IsName("Idle") && stateInfo.normalizedTime > 0.45f)
        {
            animator.Play(idleStateHash, 0, 0f);
        }
    }
}
