
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] Item[] items;
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
    PhotonView PV;

    const float maxHealth = 100f;
    float currentHealth = maxHealth;
    
    PlayerManager playerManager;

    public InGameUi inGameUiController;
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
        WeaponSwitcher();
        HealthBarController();
        if(Input.GetMouseButtonDown(0))
        {
            items[itemIndex].Use();
        }
    }

    void PlayerMotor(){

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        if(isGrounded && velocity.y < 0){
            velocity.y = -2f;
        }
         
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right*x+transform.forward*z;
        if(GetComponent<CharacterController>() != null){
            
            controller.Move(move*moveSpeed*Time.deltaTime); 
            
            if(Input.GetButtonDown("Jump")&&isGrounded){
                velocity.y = Mathf.Sqrt(jumpHeight*-1f*gravity);
            }
            velocity.y += gravity*Time.deltaTime;
            controller.Move(velocity*Time.deltaTime);
        }
    } 
    void CameraMotor()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSentivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSentivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        camController.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up, mouseX);
    }    

    void EquipItem(int _index)
    {
        if(_index == previousItemIndex)        
            return;
        
        itemIndex = _index;
        items[itemIndex].itemGameObject.SetActive(true);
        if(previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);
        }
        previousItemIndex = itemIndex;

        if(PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    void WeaponSwitcher()
    {
        for (int i = 0; i<items.Length; i++)
        {
            if(Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if(!PV.IsMine && targetPlayer == PV.Owner)
        {
            EquipItem((int)changedProps["itemIndex"]);
        }
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
        playerManager.Die();
    }

    void HealthBarController()
    {
        
        inGameUiController.SetMaxHealth(maxHealth);
        inGameUiController.SetHealth(currentHealth);
    }
}