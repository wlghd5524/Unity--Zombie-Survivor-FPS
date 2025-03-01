using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHpBar : MonoBehaviour
{
    [SerializeField]
    private GameObject HpBar;       //HP바 프리팹
    private Vector2 createPoint = new Vector2(300,50);  //생성 위치
    private int MaxHP = 100;        //최대 HP
    private int MinHP = 0;          //최소 HP
    private float CurrentHp = 100.0f;    //현재 HP
    TextMeshProUGUI text = null;   //체력 숫자
    void Start()
    {
        if (HpBar == null)
            return;
        HpBar = Instantiate(HpBar, createPoint, Quaternion.identity, GameObject.Find("Canvas").transform);        //HP바 프리팹 생성
        HpBar.transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount = 1.0f;
        text = HpBar.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        text.text = "100 / 100"; //체력 100으로 설정
        if (text == null)
            return;
    }

 
    public void Damage(int damage)
    {
        CurrentHp -= damage;        //현재 체력에서 데미지 입은만큼 감소

        HpBar.transform.GetChild(0).gameObject.GetComponent<Image>().fillAmount = CurrentHp/100;      //체력 게이지 조절
        text.text = CurrentHp.ToString() + " / 100";
        if (CurrentHp <= MinHP)
            return;         //나중에 게임종료 코드 추가

    }
}
