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
    
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }
    public void Shoot()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs" , bullet.name), bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.rotation);            
    } 
}
