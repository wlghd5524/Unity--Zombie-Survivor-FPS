using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using UnityEngine;
using UnityEngine.UIElements;

public class Bullet : MonoBehaviourPunCallbacks
{
    public float speed = 50f;
    public float maxDistance = 100f;
    public float lifetime = 3f;
    public Vector3 shootDirection;

    public WeaponController weaponController;
    private Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, 5f);
    }
    public void Init(WeaponController weaponController = null)
    {
        rb.freezeRotation = true;
        rb.linearVelocity = shootDirection * speed;
        photonView.RPC("SyncBulletVelocity", RpcTarget.OthersBuffered, rb.linearVelocity);
        this.weaponController = weaponController;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ZombieController zombie = collision.gameObject.GetComponent<ZombieController>();
        if (zombie != null)
        {
            if (zombie.photonView.IsMine) // ✅ 소유권이 있으면 직접 호출
            {
                zombie.TakeDamage(weaponController.damage);
            }
            else // ✅ 소유권이 없으면 RPC 호출
            {
                zombie.photonView.RPC("TakeDamage", RpcTarget.All, weaponController.damage);
            }
            Debug.Log("남은 좀비 체력 : " + zombie.health);
        }
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    void SyncBulletVelocity(Vector3 velocity)
    {
        rb.linearVelocity = velocity;
    }
}