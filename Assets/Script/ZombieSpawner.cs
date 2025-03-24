using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class ZombieSpawner : MonoBehaviourPunCallbacks
{
    //public Transform spawnPointsParent; // 스폰 위치들을 포함하는 부모 오브젝트
    private List<Transform> spawnPoints = new List<Transform>(); // 스폰 위치 목록
    public float initialSpawnTime = 5f; // 최초 생성 간격
    public float minSpawnTime = 1f; // 최소 생성 간격
    public float spawnAcceleration = 0.95f; // 생성 주기 감소율
    private float currentSpawnTime; // 현재 생성 간격

    void Start()
    {
        currentSpawnTime = initialSpawnTime;

        // ✅ spawnPointsParent의 모든 자식 오브젝트를 찾아서 리스트에 추가
        foreach (Transform child in transform)
        {
            spawnPoints.Add(child);
        }

        StartCoroutine(SpawnZombies());
    }

    IEnumerator SpawnZombies()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentSpawnTime);
            SpawnZombie();

            // 시간이 지날수록 스폰 속도 증가
            currentSpawnTime *= spawnAcceleration;
            currentSpawnTime = Mathf.Max(currentSpawnTime, minSpawnTime);
        }
    }

    void SpawnZombie()
    {
        if (spawnPoints.Count == 0) return;

        // 랜덤한 스폰 위치 선택
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        // 좀비 생성
        PhotonNetwork.Instantiate("Zombie1", spawnPoint.position, spawnPoint.rotation);
    }
}
