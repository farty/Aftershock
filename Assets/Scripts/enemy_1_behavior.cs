using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;


public class enemy_1_behavior : MonoBehaviour, IDamageable
{
    public float retreatDistance = 2f;
    const float maxHealth = 10f;
    float currentHealth = maxHealth;

    public NavMeshAgent agent;
    GameObject targetPlayer;
    PhotonView PV;
    [SerializeField]
    float distanceToTarget = 0f;
    void Start()
    {
        PV = GetComponent<PhotonView>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if(PV.IsMine)
        {

            float distanceToClosestEnemy = Mathf.Infinity;
            GameObject closestEnemy = null;
            GameObject [] allEnemies = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject currentEnemy in allEnemies)
            {
                float distanceToEnemy = (currentEnemy.transform.position - this.transform.position).sqrMagnitude;
                if(distanceToEnemy<distanceToClosestEnemy)
                {
                    distanceToClosestEnemy = distanceToEnemy;
                    closestEnemy = currentEnemy;
                    targetPlayer = currentEnemy;
                    distanceToTarget = Vector3.Distance(transform.position, targetPlayer.transform.position);
                }
            }
            if(Vector3.Distance(transform.position, targetPlayer.transform.position)> retreatDistance) 
            {
                agent.SetDestination(targetPlayer.transform.position);
            }

            else
            {
                if(agent.destination != transform.position)
                {
                    agent.SetDestination(transform.position);
                }                
            }            
        }        
    }

    public void TakeDamage(float damage)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
        Debug.Log(damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage)
    {
        if(!PV.IsMine)
        return;
        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if(PV.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }        
    }
}
