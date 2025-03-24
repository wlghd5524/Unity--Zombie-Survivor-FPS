using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Change_Ammo_UI : MonoBehaviour
{
    [SerializeField]
    GameObject max_Ammo;          //�ִ� źâ UI

    [SerializeField]
    GameObject current_Ammo;      //���� źâ UI

    [SerializeField]
    GameObject fill_Amount;      //źâ �̹��� ���

    TextMeshProUGUI text1;      //���� ź�� ���� UI�ؽ�Ʈ
    TextMeshProUGUI text2;      //���� źâ UI�ؽ�Ʈ
    WeaponController weapon;
    PlayerView pv;

    public void init()
    {
        pv = gameObject.GetComponent<PlayerView>();
        text1 = max_Ammo.GetComponent<TextMeshProUGUI>();
        text2 = current_Ammo.GetComponent<TextMeshProUGUI>();
        weapon = pv.player.GetComponent<WeaponController>();
        //������ �� ����
        fill_Amount.GetComponent<Image>().fillAmount = ((float)weapon.Max_Ammo / (float)weapon.currentAmmo);
        //�ؽ�Ʈ ����
        text1.text = (1000 + weapon.remaining_Ammo).ToString().Substring(1);
        text2.text = (1000 + weapon.currentAmmo).ToString().Substring(1);
    }
    //�Ѿ� ���� ���� �Լ�
    public void Change_UI(int currentAmmo, int Max_Ammo)
    {
        // �ؽ�Ʈ ����
        text2.text = (1000 + currentAmmo).ToString().Substring(1);
        //������ �� ����
        fill_Amount.GetComponent<Image>().fillAmount = 1.0f / ((float)Max_Ammo / (float)currentAmmo);
    }

    public void Basic_UI()
    { 
        fill_Amount.GetComponent<Image>().fillAmount = 1.0f;

        //�ؽ�Ʈ ����
        text1.text = (1000 + weapon.remaining_Ammo).ToString().Substring(1);
        text2.text = (1000 + weapon.currentAmmo).ToString().Substring(1);
    }
}
