using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SimpleFPS;

public class PlayerView : MonoBehaviour
{
    [SerializeField]
    GameObject menu;                //�޴�

    public GameObject player = null;     //�÷��̾� ������Ʈ

    private Transform child;           //�ڽ� ������Ʈ 
    private GameObject Health;         //Health ������Ʈ
    private GameObject DeathEffect;    //���ȿ�� ������Ʈ
    public GameObject HitDirections;   //hit ȿ��
    public GameObject HealthBar;
    public GameObject Hp;              //ü�� ��ġ ��Ÿ���� ������Ʈ CurrentNumber
    public TextMeshProUGUI HpNumber;
    private GameObject HpBar;               //ü�¹� Progress
    private float CurrentHp = 100.0f;      //����ü��
    private float MaxHp = 100.0f;          //�ִ�ü��
    private float MinHp = 0.0f;             //�ּ�ü��

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

        //Ȱ��ȭ
        Health.SetActive(true);
        child.Find("Nickname").gameObject.SetActive(true);
        child.Find("Weapons").gameObject.SetActive(true);
        child.Find("Crosshair").gameObject.SetActive(true);
        HealthBar.transform.Find("Background").Find("Deco").gameObject.SetActive(true);

        //��Ȱ��ȭ
        DeathEffect.SetActive(false);
        HitDirections.SetActive(false);
        child.Find("HitTakenEffect").gameObject.SetActive(false);
        child.Find("ImmortalityIndicator").gameObject.SetActive(false);
        child.Find("NoAmmoGroup").gameObject.SetActive(false);
        Health.transform.Find("Heal").gameObject.SetActive(false);

        HpBar = Health.transform.Find("HealthBar").transform.Find("Progress").gameObject;
        HpBar.GetComponent<Image>().fillAmount = 1.0f;

        menu.SetActive(false);              //�޴� ��Ȱ��ȭ
    }

    public void Damage(float damage)
    {
        if (CurrentHp > MaxHp)
            return;

        if (HpNumber == null)
            HpNumber = Hp.GetComponent<TextMeshProUGUI>();

        CurrentHp -= damage;                         //���� ü�¿��� ������������ŭ ����
        HpNumber.text = CurrentHp.ToString();        //���� ü�¼�ġ UI����

        if (CurrentHp <= MinHp)                      //���� ü���� 0���� �۾����� ���
             Dead();


        if (HpBar == null)
            HpBar = transform.Find("Progress").gameObject;

        HpBar.GetComponent<Image>().fillAmount = CurrentHp / 100;     //ü�¹� ����


        StartCoroutine(ActivateTemporarily());      //������Ʈ Ȱ��ȭ ��Ȱ��ȭ �ڵ�
    }

    IEnumerator ActivateTemporarily()
    {
        HitDirections.SetActive(true); // ������Ʈ Ȱ��ȭ
        yield return new WaitForSeconds(1.5f); // 1.5�� ���
        HitDirections.SetActive(false); // ������Ʈ ��Ȱ��ȭ
    }

    //����Լ�
    public void Dead()
    {
        DeathEffect.SetActive(true);
        //���� �����ڵ�
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
        }

        else
        {
            menu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
