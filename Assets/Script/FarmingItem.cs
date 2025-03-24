using UnityEngine;
using System.Collections;
using Photon.Pun;
public class FarmingItem: MonoBehaviourPunCallbacks
{
    PlayerController _p;
    GameObject player;
    float player_hp;
    string object_name;
    int Plistol_Ammo = 14;
    int heal = 50;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) // �浹�� ������Ʈ�� �÷��̾����� Ȯ��
        {
            player = collision.gameObject;
            _p = player.GetComponent<PlayerController>();

            Debug.Log("�Ĺ� ������Ʈ�� �÷��̾� �浹");

            if(gameObject.name == "Pistol")
            {
                PistolFunc();
            }
            else if(gameObject.name == "Medkit")
            {
                MedkitFunc();
            }
        }
    }

    void PistolFunc()
    {
        player.GetComponent<PlayerController>().Reload(Plistol_Ammo);
        Destroy(gameObject);
    }

    void MedkitFunc()
    {

        player.GetComponent<PlayerController>().Heal(heal);
        Destroy(gameObject);
    }
}