using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class PlayerView : MonoBehaviour
{
    [SerializeField]
    GameObject menu;                //메뉴
   
    [SerializeField]
    GameManager gameManager;

    public GameObject player = null;     //플레이어 오브젝트

    private Transform child;           //자식 오브젝트
    private GameObject Health;         //Health 오브젝트
    private GameObject DeathEffect;    //사망효과 오브젝트
    public GameObject HitDirections;   //hit 효과
    public GameObject HitTakenEffect;   //hit 효과(강화버전)
    public GameObject ImmortalityIndicator;     //회복효과
    public GameObject HealthBar;
    public GameObject healtext;             //회복량 텍스트 UI
    public GameObject Hp;              //체력 수치 나타내는 오브젝트 CurrentNumber
    public TextMeshProUGUI HpNumber;
    private GameObject HpBar;               //체력바 Progress

    private void Start()
    {
        child = transform;
        player = gameManager.go;

        Health = child.Find("Health").gameObject;
        HitDirections = child.Find("HitDirections").gameObject;
        DeathEffect = child.Find("DeathEffect").gameObject;
        HealthBar = Health.transform.Find("HealthBar").gameObject;
        Hp = Health.transform.Find("HealthGroup").Find("CurrentNumber").gameObject;
        HpNumber = Hp.GetComponent<TextMeshProUGUI>();
        HpNumber.text = player.GetComponent<PlayerController>().max_hp.ToString();
        HitTakenEffect = child.Find("HitTakenEffect").gameObject;
        ImmortalityIndicator = child.Find("ImmortalityIndicator").gameObject;
        healtext = Health.transform.Find("Heal").gameObject;

        //활성화
        Health.SetActive(true);
        child.Find("Nickname").gameObject.SetActive(true);
        child.Find("Weapons").gameObject.SetActive(true);
        child.Find("Crosshair").gameObject.SetActive(true);
        HealthBar.transform.Find("Background").Find("Deco").gameObject.SetActive(true);

        //비활성화
        DeathEffect.SetActive(false);
        HitDirections.SetActive(false);
        child.Find("NoAmmoGroup").gameObject.SetActive(false);
        Health.transform.Find("Heal").gameObject.SetActive(false);
        HitTakenEffect.SetActive(false);
        ImmortalityIndicator.SetActive(false);
        healtext.SetActive(false);

        HpBar = Health.transform.Find("HealthBar").transform.Find("Progress").gameObject;
        HpBar.GetComponent<Image>().fillAmount = 1.0f;

        menu.SetActive(false);              //메뉴 비활성화
    }

    //대미지 함수
    public void Damage(float current_hp)
    {
        Hp_Bar_Change(current_hp);

        //오브젝트 활성화 비활성화 코드
        if (current_hp <= 25)
            StartCoroutine(ActivateTemporarily(HitTakenEffect));
        else
            StartCoroutine(ActivateTemporarily(HitDirections));      

    }

    IEnumerator ActivateTemporarily(GameObject ui_object, bool destory_ui = false)
    {
        ui_object.SetActive(true);  //오브젝트 활성화
        yield return new WaitForSeconds(2.0f); // 1.5초 대기
        ui_object.SetActive(false); // 오브젝트 비활성화
    }

    //사망함수
    public void Dead()
    {
        DeathEffect.SetActive(true);
    }

    //회복 효과
    public void Heal(float current_hp, float before_hp, float max_hp)
    {
        Hp_Bar_Change(current_hp);

        if (before_hp > 50)
            healtext.GetComponent<TextMeshProUGUI>().text = "+" + (max_hp - before_hp).ToString() + " HP";
        else
            healtext.GetComponent<TextMeshProUGUI>().text = "+50 HP";

        StartCoroutine(ActivateTemporarily(ImmortalityIndicator));
        StartCoroutine(ActivateTemporarily(healtext));
    }

    //Hp_bar 변경
    private void Hp_Bar_Change(float current_hp)
    {
        if (HpNumber == null)
            HpNumber = Hp.GetComponent<TextMeshProUGUI>();

        HpNumber.text = current_hp.ToString();        //현재 체력수치 UI변경

        if (HpBar == null)
            HpBar = transform.Find("Progress").gameObject;

        HpBar.GetComponent<Image>().fillAmount = current_hp / 100;     //체력바 조절
    }
    public void Active_Menu()
    {
        if (menu == null)
            return;

        if (menu.activeSelf)
        {
            menu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            player.GetComponent<PlayerController>().enabled = true;
            player.GetComponent<WeaponController>().enabled = true;
            gameObject.GetComponent<Change_Ammo_UI>().enabled = true;
        }

        else
        {
            menu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            player.GetComponent<PlayerController>().enabled = false;
            player.GetComponent<WeaponController>().enabled = false;
            gameObject.GetComponent<Change_Ammo_UI>().enabled = false;
        }
    }
}
