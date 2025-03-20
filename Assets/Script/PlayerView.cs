using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class PlayerView : MonoBehaviour
{
    [SerializeField]
    GameObject menu;                //�޴�
   
    [SerializeField]
    GameManager gameManager;

    public GameObject player = null;     //�÷��̾� ������Ʈ

    private Transform child;           //�ڽ� ������Ʈ
    private GameObject Health;         //Health ������Ʈ
    private GameObject DeathEffect;    //���ȿ�� ������Ʈ
    public GameObject HitDirections;   //hit ȿ��
    public GameObject HitTakenEffect;   //hit ȿ��(��ȭ����)
    public GameObject ImmortalityIndicator;     //ȸ��ȿ��
    public GameObject HealthBar;
    public GameObject healtext;             //ȸ���� �ؽ�Ʈ UI
    public GameObject Hp;              //ü�� ��ġ ��Ÿ���� ������Ʈ CurrentNumber
    public TextMeshProUGUI HpNumber;
    private GameObject HpBar;               //ü�¹� Progress

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

        //Ȱ��ȭ
        Health.SetActive(true);
        child.Find("Nickname").gameObject.SetActive(true);
        child.Find("Weapons").gameObject.SetActive(true);
        child.Find("Crosshair").gameObject.SetActive(true);
        HealthBar.transform.Find("Background").Find("Deco").gameObject.SetActive(true);

        //��Ȱ��ȭ
        DeathEffect.SetActive(false);
        HitDirections.SetActive(false);
        child.Find("NoAmmoGroup").gameObject.SetActive(false);
        Health.transform.Find("Heal").gameObject.SetActive(false);
        HitTakenEffect.SetActive(false);
        ImmortalityIndicator.SetActive(false);
        healtext.SetActive(false);

        HpBar = Health.transform.Find("HealthBar").transform.Find("Progress").gameObject;
        HpBar.GetComponent<Image>().fillAmount = 1.0f;

        menu.SetActive(false);              //�޴� ��Ȱ��ȭ
    }

    //����� �Լ�
    public void Damage(float current_hp)
    {
        Hp_Bar_Change(current_hp);

        //������Ʈ Ȱ��ȭ ��Ȱ��ȭ �ڵ�
        if (current_hp <= 25)
            StartCoroutine(ActivateTemporarily(HitTakenEffect));
        else
            StartCoroutine(ActivateTemporarily(HitDirections));      

    }

    IEnumerator ActivateTemporarily(GameObject ui_object, bool destory_ui = false)
    {
        ui_object.SetActive(true);  //������Ʈ Ȱ��ȭ
        yield return new WaitForSeconds(2.0f); // 1.5�� ���
        ui_object.SetActive(false); // ������Ʈ ��Ȱ��ȭ
    }

    //����Լ�
    public void Dead()
    {
        DeathEffect.SetActive(true);
    }

    //ȸ�� ȿ��
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

    //Hp_bar ����
    private void Hp_Bar_Change(float current_hp)
    {
        if (HpNumber == null)
            HpNumber = Hp.GetComponent<TextMeshProUGUI>();

        HpNumber.text = current_hp.ToString();        //���� ü�¼�ġ UI����

        if (HpBar == null)
            HpBar = transform.Find("Progress").gameObject;

        HpBar.GetComponent<Image>().fillAmount = current_hp / 100;     //ü�¹� ����
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
