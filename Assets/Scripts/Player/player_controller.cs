using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class player_controller : MonoBehaviour
{

    [SerializeField] GameObject camController;
    public int mouseSentivity = 100;
    public Transform playerBody;
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
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if(!PV.IsMine)
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
}
