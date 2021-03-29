using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;


public class enemy_1_behavior : MonoBehaviour, IDamageable
{
    public float retreatDistance = 2f;
    
    const float maxHealth = 10f;
    [SerializeField]
    float currentHealth = maxHealth;

    public NavMeshAgent agent;
    [SerializeField]
    GameObject targetPlayer;
    PhotonView PV;
    [SerializeField]
    float distanceToTarget = 0f;

    float timeBetweenAttacks;
    public float startTimeBetweenAttacks = 2;
    void Start()
    {
        timeBetweenAttacks = startTimeBetweenAttacks;
        PV = GetComponent<PhotonView>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        ChasePlayer();
        Attack();
    }
    void ChasePlayer()
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

    void Attack()
    {
        
        if(timeBetweenAttacks >= startTimeBetweenAttacks) 
        {
            if(distanceToTarget<=retreatDistance)
            {
                if(targetPlayer != null)
                {
                    timeBetweenAttacks = 0;
                    targetPlayer.GetComponent<IDamageable>()?.TakeDamage(10);
                }                                
            }            
        }
        else
        {
            timeBetweenAttacks+= Time.deltaTime;
        }
    }
}
