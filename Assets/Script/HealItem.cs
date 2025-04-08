using UnityEngine;
using System.Collections;
public class HealItem : MonoBehaviour
{
    PlayerController _p;
    GameObject player;
    GameObject medkit;
    float player_hp;

    private void Awake()
    {
        //수정예정
        medkit = transform.Find("Medkit_01_Prefab_01").gameObject;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!medkit.activeSelf)
            return;

        if (collision.gameObject.CompareTag("Player")) // 충돌한 오브젝트가 플레이어인지 확인
        {
            player = collision.gameObject;
            _p = player.GetComponent<PlayerController>();

            Debug.Log("응급키트 오브젝트와 플레이어 충돌");
            CollisionFunction(); // 충돌 시 실행할 함수
        }
    }

    void CollisionFunction()
    {
        player.GetComponent<PlayerController>().Heal();

        StartCoroutine(DeactivateAndReactivate());
    }

    /// <summary>
    /// 30초간 비활성화
    /// </summary>
    /// <returns></returns>
    IEnumerator DeactivateAndReactivate()
    {
        // 객체 비활성화
        medkit.SetActive(false);

        // 30초 대기
        yield return new WaitForSeconds(5);

        // 객체 다시 활성화
        medkit.SetActive(true);
    }
}