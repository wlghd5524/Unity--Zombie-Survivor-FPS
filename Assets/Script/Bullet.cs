using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviourPunCallbacks
{
    public Vector3 shootDirection;
    public BulletSO bulletSO;

    public WeaponController weaponController;
    private Rigidbody rb;
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
        rb.linearVelocity = shootDirection * bulletSO.speed;
        photonView.RPC("SyncBulletVelocity", RpcTarget.OthersBuffered, rb.linearVelocity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        ZombieController zombie = collision.gameObject.GetComponent<ZombieController>();
        if (zombie != null)
        {
            PlayerController player = weaponController.gameObject.GetComponent<PlayerController>();
            zombie.Damage(bulletSO.bullet_damage,player);
            Debug.Log("남은 좀비 체력 : " + zombie.health);
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