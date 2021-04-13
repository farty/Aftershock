using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class SingleShotgun : MonoBehaviourPunCallbacks
{
    [SerializeField] 
    GameObject bulletSpawnPoint;
    [SerializeField] 
    GameObject bullet;
    [SerializeField] 
    GameObject owner;
    [SerializeField] 
    float reloadTime;
    float reloadCur;
    [SerializeField]
    int bulletsMax;
    int bulletsCur;
    [SerializeField]
    float timeBetweenShots;
    float curTimeBetweenShots;

    bool isReloaded;
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
            if(isReloaded)
            {

            }
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs" , bullet.name), bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.rotation);
        }                    
    } 
}