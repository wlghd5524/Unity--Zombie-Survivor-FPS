using UnityEngine;
using System.Collections;
using Photon.Pun;
public class FarmingItem: MonoBehaviourPunCallbacks
{
    PlayerController _p;
    GameObject player;

    public ItemSO itemSO;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) // 충돌한 오브젝트가 플레이어인지 확인
        {
            player = collision.gameObject;
            _p = player.GetComponent<PlayerController>();

            Debug.Log("파밍 오브젝트와 플레이어 충돌");

            if(gameObject.name.Contains("Pistol_Item"))
            {
                PistolFunc();
            }
            else if(gameObject.name.Contains("Medkit"))
            {
                MedkitFunc();
            }
        }
    }

    void PistolFunc()
    {
        player.GetComponent<PlayerController>().Reload(itemSO.add_Ammo);
        Destroy(gameObject);
    }

    void MedkitFunc()
    {

        player.GetComponent<PlayerController>().Heal(itemSO.mediKit_heal);
        Destroy(gameObject);
    }
}