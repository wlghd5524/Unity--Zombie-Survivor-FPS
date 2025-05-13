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
        ZombieController zombie = collision.gameObject.GetComponent<ZombieController>();
        if (zombie != null)
        {
            Debug.Log(collision.gameObject.name + " 충돌");
            if(collision.gameObject.name == "Z_Head")
            {
                zombie.Damage(50f, true);
            }
            else
            {
                zombie.Damage(25f);
            }
            Debug.Log("남은 좀비 체력 : " + zombie.health);
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