using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Change_Ammo_UI : MonoBehaviour
{
    [SerializeField]
    GameObject max_Ammo;          //최대 탄창 UI

    [SerializeField]
    GameObject current_Ammo;      //현재 탄창 UI

    [SerializeField]
    GameObject fill_Amount;      //탄창 이미지 토글

    TextMeshProUGUI text1;      //최대 탄창 UI텍스트
    TextMeshProUGUI text2;      //현재 탄창 UI텍스트

    WeaponController weapon;
    PlayerView pv;
    private void Start()
    {
        pv = gameObject.GetComponent<PlayerView>();
        text1 = max_Ammo.GetComponent<TextMeshProUGUI>();
        text2 = current_Ammo.GetComponent<TextMeshProUGUI>();
        weapon = pv.player.GetComponent<WeaponController>();
        //게이지 바 변경
        fill_Amount.GetComponent<Image>().fillAmount =((float)weapon.maxAmmo /(float)weapon.currentAmmo);
        //텍스트 변경
        text1.text = (1000+ weapon.maxAmmo).ToString().Substring(1);
        text2.text = (1000+ weapon.maxAmmo).ToString().Substring(1);
    }
    //총알 개수 변경 함수
    public void Change_UI()
    {
        if (weapon.menuView_Check.activeSelf)
            return;

        // 텍스트 변경
        text2.text = (1000 + weapon.currentAmmo).ToString().Substring(1);
        //게이지 바 변경
        fill_Amount.GetComponent<Image>().fillAmount = 1.0f / ((float)weapon.maxAmmo / (float)weapon.currentAmmo);
    }

    public void Basic_UI()
    {
        if (weapon.menuView_Check.activeSelf)
            return;

        fill_Amount.GetComponent<Image>().fillAmount = 1.0f;
        //텍스트 변경
        text1.text = (1000 + weapon.maxAmmo).ToString().Substring(1);
        text2.text = (1000 + weapon.maxAmmo).ToString().Substring(1);
    }
}
