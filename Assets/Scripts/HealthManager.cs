using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using System.IO;

public class HealthManager : MonoBehaviourPunCallbacks
{
    
    public PhotonView PV;
    public PlayerController playerController;
    public bool isDead = false;
    public float maxHealth = 100f;
    public float currentHealth;
    
    void Start()
    {
        PV = GetComponent<PhotonView>();
        playerController = GetComponent<PlayerController>();
        currentHealth = maxHealth;
    }
    public void TakeDamage(float damage)
    {
        if(PV.IsMine) 
        {
            PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
        }    
    }
    [PunRPC]
    void RPC_TakeDamage(float damage)
    {
        
        currentHealth -= damage;
        Debug.Log(damage);
        if(currentHealth <= 0)
        {
            isDead = true;
        }
    }
}
