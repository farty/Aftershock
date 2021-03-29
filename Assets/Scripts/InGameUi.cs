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
    [SerializeField] Image overlayImage;
    public Slider slider;

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
    }
    void Update()
    {
        DamageIndicator();
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
     public void Continue()
    {
        Debug.Log("continue");
        Cursor.lockState = CursorLockMode.Locked;
        MenuManager.Instance.OpenMenu("in game ui");        
    }
    public void ExitLevel()
    {
        GameObject roomManager = GameObject.FindWithTag("room_manager");
        Destroy(roomManager);
        StartCoroutine(DisconnectAndLoad());
    }

    IEnumerator DisconnectAndLoad()
    {
        PhotonNetwork.Disconnect();
        while(PhotonNetwork.IsConnected)
        yield return null;
        SceneManager.LoadScene(0); 
    }
    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
    }
    public void SetHealth (float health)
    {
        slider.value = health;
    }

    void DamageIndicator()
    {
        if(a>0)
        {
            Debug.Log(a);
            a-=0.1f;
        } 
        Color c = new Color(r,g,b,a);  
        overlayImage.color = c;      
    }

    public void TakeDamage()
    {
        a = 1;
    }
}
