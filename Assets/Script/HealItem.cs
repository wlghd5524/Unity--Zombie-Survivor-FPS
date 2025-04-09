using UnityEngine;
using System.Collections;
using Photon.Pun;
public class HealItem : MonoBehaviour
{
    public float rotationSpeed = 90f;
    private void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) // 충돌한 오브젝트가 플레이어인지 확인
        {
            other.gameObject.GetComponent<PlayerController>().Heal();
            PhotonNetwork.Destroy(gameObject);
            Debug.Log("응급키트 오브젝트와 플레이어 충돌");
        }
    }
}