using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class SingleShotgun : MonoBehaviourPunCallbacks
{
    [SerializeField] 
    GameObject itemGameObject;
    [SerializeField] 
    GameObject bulletSpawnPoint;
    [SerializeField] 
    GameObject bullet;
    [SerializeField] 
    GameObject owner;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }
    public void Shoot()
    {
        if(owner.GetComponent<PlayerController>().PV.IsMine)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs" , bullet.name), bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.rotation);
        }                    
    } 
}