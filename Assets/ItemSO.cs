using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu]
public class ItemSO : ScriptableObject
{
    public int mediKit_heal;
    public int add_Ammo;
    public int Pistol_Damage;
    public float Speed;
    public float floatSpeed; // 위아래 이동 속도
    public float floatAmount; //위아래 이동 범위
    public GameObject[] dropItem;
}
