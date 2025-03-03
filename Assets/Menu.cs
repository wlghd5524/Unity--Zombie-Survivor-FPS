using SimpleFPS;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    public GameObject SettingsView;           //SettingsView 오브젝트

    private void Start()
    {
        SettingsView = Instantiate(SettingsView,transform);
        
        SettingsView.transform.Find("PopUp").Find("ConfirmButton").GetComponent<Button>().onClick.AddListener(CloseSettings); 
        SettingsView.transform.Find("PopUp").Find("PopupFrame").Find("Background").Find("CloseButton").GetComponent<Button>().onClick.AddListener(CloseSettings);
        SettingsView.SetActive(false);
    }
    public void OpenSettings()
    {
        SettingsView.gameObject.SetActive(true);
    }

    public void CloseSettings()
    {
        SettingsView.gameObject.SetActive(false);
    }
}
