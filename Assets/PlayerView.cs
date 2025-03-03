using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SimpleFPS;

public class PlayerView : MonoBehaviour
{
    [SerializeField]
    GameObject menu;                //메뉴

    private Transform child;           //자식 오브젝트 
    private GameObject Health;         //Health 오브젝트
    private GameObject DeathEffect;    //사망효과 오브젝트
    public GameObject HitDirections;   //hit 효과
    public GameObject HealthBar;
    public GameObject Hp;              //체력 수치 나타내는 오브젝트 CurrentNumber
    public TextMeshProUGUI HpNumber;
    private GameObject HpBar;               //체력바 Progress
    
    private float CurrentHp = 100.0f;      //현재체력
    private float MaxHp = 100.0f;          //최대체력
    private float MinHp = 0.0f;             //최소체력

    private void Start()
    {
        child = transform;

        Health = child.Find("Health").gameObject;
        HitDirections = child.Find("HitDirections").gameObject;
        DeathEffect = child.Find("DeathEffect").gameObject;
        HealthBar = Health.transform.Find("HealthBar").gameObject;
        Hp = Health.transform.Find("HealthGroup").Find("CurrentNumber").gameObject;
        HpNumber = Hp.GetComponent<TextMeshProUGUI>();
        HpNumber.text = "100";

        //활성화
        Health.SetActive(true);
        child.Find("Nickname").gameObject.SetActive(true);
        child.Find("Weapons").gameObject.SetActive(true);
        child.Find("Crosshair").gameObject.SetActive(true);
        HealthBar.transform.Find("Background").Find("Deco").gameObject.SetActive(true);

        //비활성화
        DeathEffect.SetActive(false);
        HitDirections.SetActive(false);
        child.Find("HitTakenEffect").gameObject.SetActive(false);
        child.Find("ImmortalityIndicator").gameObject.SetActive(false);
        child.Find("NoAmmoGroup").gameObject.SetActive(false);
        Health.transform.Find("Heal").gameObject.SetActive(false);

        HpBar = Health.transform.Find("HealthBar").transform.Find("Progress").gameObject;
        HpBar.GetComponent<Image>().fillAmount = 1.0f;

        menu = Instantiate(menu);               //인스턴스 생성
        menu.SetActive(false);              //메뉴 비활성화
    }

    private void Update()
    {
        InPutKey();
    }
    public void Damage(float damage)
    {
        if (CurrentHp > MaxHp)
            return;

        if (HpNumber == null)
            HpNumber = Hp.GetComponent<TextMeshProUGUI>();

        CurrentHp -= damage;                         //현재 체력에서 데미지받은만큼 감소
        HpNumber.text = CurrentHp.ToString();        //현재 체력수치 UI변경

        if (CurrentHp <= MinHp)                      //현재 체력이 0보다 작아지면 사망
             Dead();


        if (HpBar == null)
            HpBar = transform.Find("Progress").gameObject;

        HpBar.GetComponent<Image>().fillAmount = CurrentHp / 100;     //체력바 조절


        StartCoroutine(ActivateTemporarily());      //오브젝트 활성화 비활성화 코드
    }

    IEnumerator ActivateTemporarily()
    {
        HitDirections.SetActive(true); // 오브젝트 활성화
        yield return new WaitForSeconds(1.5f); // 1.5초 대기
        HitDirections.SetActive(false); // 오브젝트 비활성화
    }

    //사망함수
    public void Dead()
    {
        DeathEffect.SetActive(true);
        //게임 종료코드
    }

    public void InPutKey()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            if (menu == null)
                return;

            if (menu.activeSelf)
            {
                menu.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            else
            {
                menu.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}
