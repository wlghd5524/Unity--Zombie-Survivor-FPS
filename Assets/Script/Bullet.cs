using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviourPunCallbacks
{
    public float speed = 50f;
    public float maxDistance = 100f;
    public float lifetime = 3f;
    public Vector3 shootDirection;

    public WeaponController weaponController;
    private Rigidbody rb;
    private bool hasDamageApplied = false;
    public bool isHeadshot = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (photonView.IsMine)
        {
            StartCoroutine(DestroyTimer());
        }
    }
    public void Init()
    {
        rb.freezeRotation = true;
        rb.linearVelocity = shootDirection * speed;
        photonView.RPC("SyncBulletVelocity", RpcTarget.OthersBuffered, rb.linearVelocity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasDamageApplied)
            return;
        
        ZombieController zombie = collision.gameObject.GetComponent<ZombieController>();
        if (zombie != null)
        {
            // 충돌 지점 가져오기 (첫 번째 접촉점 사용)
            Vector3 hitPoint = collision.contacts[0].point;
            
            // 좀비의 로컬 좌표계로 변환
            Vector3 localHitPoint = zombie.transform.InverseTransformPoint(hitPoint);
            
            // 좀비 높이 구하기
            Collider zombieCollider = zombie.GetComponent<Collider>();
            float zombieHeight = zombieCollider.bounds.size.y;
            
            // 좀비 모델의 상단 25%를 머리로 간주
            float headThreshold = zombieHeight * 0.75f;
            
            // 머리 타격 판정
            isHeadshot = localHitPoint.y > headThreshold;
            
            zombie.Damage(weaponController.damage, isHeadshot);
            
            Debug.Log("좀비 " + (isHeadshot ? "헤드샷!" : "바디샷") + " / 남은 체력: " + zombie.health);
            
            hasDamageApplied = true;
        }
        
        if(photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    void SyncBulletVelocity(Vector3 velocity)
    {
        rb.linearVelocity = velocity;
    }

    IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(5f);
        PhotonNetwork.Destroy(gameObject);
    }
}