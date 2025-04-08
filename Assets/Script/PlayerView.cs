using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class PlayerView : MonoBehaviour
{
    [Header("Menu")]
    public GameObject menu;

    [Header("Health UI")]
    private GameObject Health;
    public GameObject HealthBar;
    public GameObject healtext;             //회복량 텍스트 UI
    public GameObject Hp;              //체력 수치 나타내는 오브젝트 CurrentNumber
    public TextMeshProUGUI HpNumber;
    private GameObject HpBar;

    [Header("Effect UI")]
    private GameObject DeathEffect;
    public GameObject HitDirections;
    public GameObject HitTakenEffect;
    public GameObject ImmortalityIndicator;

    [Header("Ammo UI")]
    [SerializeField] private GameObject max_Ammo;
    [SerializeField] private GameObject current_Ammo;
    [SerializeField] private GameObject fill_Amount;
    private TextMeshProUGUI ammoMaxText;
    private TextMeshProUGUI ammoCurrentText;

    private Transform child;

    private void Start()
    {
        InitializeReferences();
        InitializeUI();
        SetupAmmoUI();
    }

    private void InitializeReferences()
    {
        child = transform;

        // Health UI 참조
        Health = child.Find("Health").gameObject;
        HitDirections = child.Find("HitDirections").gameObject;
        DeathEffect = child.Find("DeathEffect").gameObject;
        HealthBar = Health.transform.Find("HealthBar").gameObject;
        Hp = Health.transform.Find("HealthGroup").Find("CurrentNumber").gameObject;
        HpNumber = Hp.GetComponent<TextMeshProUGUI>();
        HpNumber.text = GameManager.Instance.player.GetComponent<PlayerController>().max_hp.ToString();
        
        // Effect UI 참조
        HitTakenEffect = child.Find("HitTakenEffect").gameObject;
        ImmortalityIndicator = child.Find("ImmortalityIndicator").gameObject;
        healtext = Health.transform.Find("Heal").gameObject;

        // Ammo UI 참조
        max_Ammo = child.Find("Weapons/CurrentWeapon/WeaponInfo/AmmoGroup/MaxNumber").gameObject;
        current_Ammo = child.Find("Weapons/CurrentWeapon/WeaponInfo/AmmoGroup/CurrentNumber").gameObject;
        fill_Amount = child.Find("Weapons/CurrentWeapon/WeaponBox/BarGroup/Progress").gameObject;
    }

    private void InitializeUI()
    {
        // UI 활성화
        Health.SetActive(true);
        child.Find("Nickname").gameObject.SetActive(true);
        child.Find("Weapons").gameObject.SetActive(true);
        child.Find("Crosshair").gameObject.SetActive(true);
        HealthBar.transform.Find("Background").Find("Deco").gameObject.SetActive(true);

        // UI 비활성화
        DeathEffect.SetActive(false);
        HitDirections.SetActive(false);
        child.Find("NoAmmoGroup").gameObject.SetActive(false);
        Health.transform.Find("Heal").gameObject.SetActive(false);
        HitTakenEffect.SetActive(false);
        ImmortalityIndicator.SetActive(false);
        healtext.SetActive(false);

        // 체력바 초기화
        HpBar = Health.transform.Find("HealthBar").transform.Find("Progress").gameObject;
        HpBar.GetComponent<Image>().fillAmount = 1.0f;

        menu.SetActive(false);
    }

    private void SetupAmmoUI()
    {
        ammoMaxText = max_Ammo.GetComponent<TextMeshProUGUI>();
        ammoCurrentText = current_Ammo.GetComponent<TextMeshProUGUI>();
        WeaponController weapon = GameManager.Instance.player.GetComponent<WeaponController>();
        UpdateAmmoUI(weapon.currentAmmo, weapon.maxAmmo);
    }

    public void UpdateAmmoUI(int currentAmmo, int maxAmmo)
    {
        ammoCurrentText.text = currentAmmo.ToString("D3");
        ammoMaxText.text = maxAmmo.ToString("D3");
        float fillAmount = currentAmmo > 0 ? (float)currentAmmo / maxAmmo : 0f;
        fill_Amount.GetComponent<Image>().fillAmount = fillAmount;
    }

    public void ResetAmmoUI(int maxAmmo)
    {
        fill_Amount.GetComponent<Image>().fillAmount = 1.0f;
        string ammoText = maxAmmo.ToString("D3");
        ammoMaxText.text = ammoText;
        ammoCurrentText.text = ammoText;
    }

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
            GameManager.Instance.player.GetComponent<PlayerController>().enabled = true;
            GameManager.Instance.player.GetComponent<WeaponController>().enabled = true;
        }
        else
        {
            menu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            GameManager.Instance.player.GetComponent<PlayerController>().enabled = false;
            GameManager.Instance.player.GetComponent<WeaponController>().enabled = false;
        }
    }
}
