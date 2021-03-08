using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;
    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start() 
    {
        if(PV.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        Vector3 spawn = new Vector3 (0, 10f, 0);
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs" , "PlayerController"), spawn, Quaternion.identity);
    }
    
    
}
