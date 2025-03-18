using UnityEngine;
using System.Collections;
public class HealItem : MonoBehaviour
{
    GameObject player;
    GameObject medkit;
    float player_hp;

    private void Awake()
    {
        medkit = transform.Find("Medkit_01_Prefab_01").gameObject;
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) // �浹�� ������Ʈ�� �÷��̾����� Ȯ��
        {
            player = collision.gameObject;
            Debug.Log("����ŰƮ ������Ʈ�� �÷��̾� �浹");
            CollisionFunction(); // �浹 �� ������ �Լ�
        }
    }

    void CollisionFunction()
    {
        player.GetComponent<PlayerController>().Heal();

        StartCoroutine(DeactivateAndReactivate());
    }

    /// <summary>
    /// 30�ʰ� ��Ȱ��ȭ
    /// </summary>
    /// <returns></returns>
    IEnumerator DeactivateAndReactivate()
    {
        // ��ü ��Ȱ��ȭ
        medkit.SetActive(false);

        // 30�� ���
        yield return new WaitForSeconds(30);

        // ��ü �ٽ� Ȱ��ȭ
        medkit.SetActive(true);
    }
}