using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 50f;
    public float maxDistance = 100f;
    public float lifetime = 3f;

    public WeaponController weaponController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, 5f);
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
}