using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUi : MonoBehaviour
{
    [SerializeField] GameObject inGameMenu;
    [SerializeField] GameObject defeatMenu;
    [SerializeField] PlayerController playcerController;
    [SerializeField] Image overlayImage;
    public HealthManager healthManager;
    public Slider slider;
    float sliderMaxHealth;
    float sliderCurHealth;
    float r;
    float g;
    float b;
    float a;

    void Start()
    {
        r = overlayImage.color.r;
        g = overlayImage.color.g;
        b = overlayImage.color.b;
        a = 0;
        
        playcerController = GetComponent<PlayerController>();
        sliderCurHealth = healthManager.currentHealth;
        sliderMaxHealth = healthManager.maxHealth;
    }
    void Update()
    {
        if(defeatMenu.activeSelf)
        return;
        
        DamageIndicator();
        EscapeButtonChecker();
        ManageHealth();

    }
     public void Continue()
    {
        Cursor.lockState = CursorLockMode.Locked;
        MenuManager.Instance.OpenMenu("in game ui"); 
    }
    public void ExitLevel()
    {
        
        StartCoroutine(DisconnectAndLoad());
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    IEnumerator DisconnectAndLoad()
    {
        PhotonNetwork.Disconnect();
        while(PhotonNetwork.IsConnected)
        
        yield return null;
        SceneManager.LoadScene(0); 
        GameObject roomManager = GameObject.FindWithTag("room_manager");
        Destroy(roomManager);
    }
    
    void DamageIndicator()
    {
        if(a>0)
        {
            a-=0.1f;
        } 
        Color c = new Color(r,g,b,a);  
        overlayImage.color = c;      
    }

    public void TakeDamage()
    {
        a = 1;
    }

    void ManageHealth()
    {
        if(healthManager.currentHealth<sliderCurHealth)
        {
            TakeDamage();
            sliderCurHealth = healthManager.currentHealth;
            slider.value = sliderCurHealth;
        }
        sliderCurHealth = healthManager.currentHealth;
        slider.maxValue = sliderMaxHealth;
    }

    void EscapeButtonChecker()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            {            
            if(inGameMenu.activeSelf == false)
            {
                Cursor.visible = (true);
                Cursor.lockState = CursorLockMode.Confined;
                MenuManager.Instance.OpenMenu("in game menu");                
            }
            else
            {
                Cursor.visible = (false);
                Cursor.lockState = CursorLockMode.Locked;
                MenuManager.Instance.OpenMenu("in game ui");                
            }
        }
    }
}
