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
            zombie.health -= weaponController.damage;
            Debug.Log("남은 좀비 체력 : " + zombie.health);
        }
        Destroy(gameObject);
    }

    [PunRPC]
    void SyncBulletVelocity(Vector3 velocity)
    {
        rb.linearVelocity = velocity;
    }
}