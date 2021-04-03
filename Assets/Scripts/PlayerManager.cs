using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;
    GameObject [] respawns;
    GameObject controller;
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
        if(respawns == null)
        {
            respawns = GameObject.FindGameObjectsWithTag("Respawn");
        }

        int i = Random.Range(0, respawns.Length);
        Vector3 spawn = respawns[i].transform.position;

        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs" , "PlayerController"), spawn, Quaternion.identity, 0, new object [] {PV.ViewID});
    }
}
