
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using System.IO;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks
{
    bool isdead = false;

    [SerializeField] GameObject[] guns;
    [SerializeField] GameObject InGameUi;
    GameObject _inGameUi;
    [SerializeField] GameObject camController;
    
    int itemIndex;
    int previousItemIndex = -1;

    public int mouseSentivity = 100;
    float xRotation = 0f;

    public float moveSpeed = 12f;
    public float gravity = -20;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;
  
    CharacterController controller;
    public PhotonView PV;

    const float maxHealth = 100f;
    float currentHealth = maxHealth;
    
    PlayerManager playerManager;

    public InGameUi inGameUiController;
    ScoreCounter score;
    [SerializeField] GameObject rewardPrefab;
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        PV = GetComponent<PhotonView>();
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        if(PV.IsMine)
        {
            EquipItem(0);
            GameObject _inGameUi = Instantiate(InGameUi, Vector3.zero, Quaternion.identity);
            InGameUi = _inGameUi;
            inGameUiController = InGameUi.GetComponent<InGameUi>();
            score = inGameUiController.GetComponentInChildren<ScoreCounter>();
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
        }
    }
    void Update()
    {
        if(!PV.IsMine)
        return;
        
        PlayerMotor();
        CameraMotor();
        HealthBarController();
        
        for(int i = 0; i< guns.Length; i++)
        {
            if(Input.GetKeyDown((i+1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }
    }

    void PlayerMotor()
    {
        if(isdead == false)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
            if(isGrounded && velocity.y < 0)
            {
            velocity.y = -2f;
            }
         
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right*x+transform.forward*z;
            if(GetComponent<CharacterController>() != null)
            {
                controller.Move(move*moveSpeed*Time.deltaTime); 
            
                if(Input.GetButtonDown("Jump")&&isGrounded)
                {
                velocity.y = Mathf.Sqrt(jumpHeight*-1f*gravity);
                }
                velocity.y += gravity*Time.deltaTime;
                controller.Move(velocity*Time.deltaTime);
            }
        }
    } 
    void CameraMotor()
    {
        if(isdead == false)
        {
        float mouseX = Input.GetAxis("Mouse X") * mouseSentivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSentivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        camController.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up, mouseX);
        } 
    }    

    void EquipItem(int _index)
    {
        if(_index == previousItemIndex)        
            return;
        
        itemIndex = _index;
        guns[itemIndex].GetComponent<WeaponSwitcher>()?.SwitchWeapon();
        if(previousItemIndex != -1)
        {
            guns[previousItemIndex].GetComponent<WeaponSwitcher>()?.SwitchWeapon();
        }
        previousItemIndex = itemIndex;

        if(PV.IsMine)
            {
                Hashtable hash = new Hashtable();
                hash.Add("itemIndex", itemIndex);
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            } 
    }
    
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if(!PV.IsMine && targetPlayer == PV.Owner)
        {
            EquipItem((int)changedProps["itemIndex"]);
        }
    } 
    void HealthBarController()
    {   
        inGameUiController.SetMaxHealth(maxHealth);
        inGameUiController.SetHealth(currentHealth);
    }
    
    public void TakeDamage(float damage)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage)
    {
        if(!PV.IsMine)
        return;

       currentHealth -= damage;
       inGameUiController.TakeDamage();
       if(currentHealth <= 0)
       {
           Die();
       }
    }
    void Die()
    {
        rewardPrefab = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs",rewardPrefab.name),transform.position, Quaternion.identity);
        rewardPrefab.GetComponent<RewardBehavior>().reward = score.currentScore;
        MenuManager.Instance.OpenMenu("defeat"); 
        isdead = true;
        score.currentScore = 0;
        Cursor.visible = (true);
        Cursor.lockState = CursorLockMode.Confined;
    }
    
    public void GetReward(int reward)
    {
        if(PV.IsMine)
        {
            Debug.Log(reward);
            score.currentScore += reward;
        }
    }
}