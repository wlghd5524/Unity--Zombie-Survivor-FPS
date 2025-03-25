using UnityEngine;

public class RotateItem : MonoBehaviour
{
    private Vector3 startPosition;
    ItemSO itemSO;

    private void Awake()
    {
        transform.Rotate(0f, Random.Range(0f, 360f), 0f);
        startPosition = transform.position;
        itemSO = GetComponent<FarmingItem>().itemSO;
    }

    private void Update()
    {
        transform.Rotate(0f, itemSO.Speed * Time.deltaTime, 0f);
        float newY = startPosition.y + Mathf.Sin(Time.time * itemSO.floatSpeed) * itemSO.floatAmount;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
