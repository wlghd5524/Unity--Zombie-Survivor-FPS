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

    TextMeshProUGUI text1;      //�ִ� źâ UI�ؽ�Ʈ
    TextMeshProUGUI text2;      //���� źâ UI�ؽ�Ʈ

    WeaponController weapon;
    PlayerView pv;
    private void Start()
    {
        pv = gameObject.GetComponent<PlayerView>();
        text1 = max_Ammo.GetComponent<TextMeshProUGUI>();
        text2 = current_Ammo.GetComponent<TextMeshProUGUI>();
        weapon = pv.player.GetComponent<WeaponController>();
        //������ �� ����
        fill_Amount.GetComponent<Image>().fillAmount =((float)weapon.maxAmmo /(float)weapon.currentAmmo);
        //�ؽ�Ʈ ����
        text1.text = (1000+ weapon.maxAmmo).ToString().Substring(1);
        text2.text = (1000+ weapon.maxAmmo).ToString().Substring(1);
    }
    //�Ѿ� ���� ���� �Լ�
    public void Change_UI()
    {
        if (weapon.menuView_Check.activeSelf)
            return;

        // �ؽ�Ʈ ����
        text2.text = (1000 + weapon.currentAmmo).ToString().Substring(1);
        //������ �� ����
        fill_Amount.GetComponent<Image>().fillAmount = 1.0f / ((float)weapon.maxAmmo / (float)weapon.currentAmmo);
    }

    public void Basic_UI()
    {
        if (weapon.menuView_Check.activeSelf)
            return;

        fill_Amount.GetComponent<Image>().fillAmount = 1.0f;
        //�ؽ�Ʈ ����
        text1.text = (1000 + weapon.maxAmmo).ToString().Substring(1);
        text2.text = (1000 + weapon.maxAmmo).ToString().Substring(1);
    }
}
