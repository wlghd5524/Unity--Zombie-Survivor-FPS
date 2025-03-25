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
        if (collision.gameObject.CompareTag("Player")) // �浹�� ������Ʈ�� �÷��̾����� Ȯ��
        {
            player = collision.gameObject;
            _p = player.GetComponent<PlayerController>();

            Debug.Log("�Ĺ� ������Ʈ�� �÷��̾� �浹");

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