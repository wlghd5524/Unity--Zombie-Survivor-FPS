using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu]
public class ItemSO : ScriptableObject
{
    public int mediKit_heal;
    public int add_Ammo;
    public int Pistol_Damage;
    public float Speed;
    public float floatSpeed; // ���Ʒ� �̵� �ӵ�
    public float floatAmount; //���Ʒ� �̵� ����
    public GameObject[] dropItem;
}
