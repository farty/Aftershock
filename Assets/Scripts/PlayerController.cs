
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using System.IO;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks
{    
    [SerializeField] GameObject[] guns;
    [SerializeField] GameObject InGameUi;
    
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
    public HealthManager healthManager;
    
    
    PlayerManager playerManager;

    public InGameUi inGameUiController;
    ScoreCounter score;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        PV = GetComponent<PhotonView>();
        healthManager = GetComponent<HealthManager>();
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        if(PV.IsMine)
        {
            EquipItem(0);
            InGameUi = Instantiate(InGameUi, Vector3.zero, Quaternion.identity);            
            inGameUiController = InGameUi.GetComponent<InGameUi>();
            inGameUiController.healthManager = GetComponent<HealthManager>();
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
        DeadCheck();
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
        if(healthManager.isDead == false)
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
        if(healthManager.isDead == false)
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
    
    public void GetReward(int reward)
    {
        if(PV.IsMine)
        {
            if(healthManager.isDead == false)
            {
                Debug.Log(reward);
                score.currentScore += reward;
            }            
        }
    }

    void DeadCheck()
    {
        if(healthManager.isDead)
        {
           inGameUiController.ExitLevel();
        }
    }
}