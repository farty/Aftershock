using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;
using System.IO;

public class enemy_1_behavior : MonoBehaviour
{
    public float retreatDistance = 2f;
    
    const float maxHealth = 20;
    [SerializeField]
    float currentHealth = maxHealth;

    public NavMeshAgent agent;
    [SerializeField]
    GameObject targetPlayer;
    PhotonView PV;
    [SerializeField]
    float distanceToTarget = 0f;
    [SerializeField]
    GameObject rewardPrefab;

    [SerializeField]
    int damage = 20;
    float timeBetweenAttacks;
    public float startTimeBetweenAttacks = 2;
    HealthManager healthManager;
    void Start()
    {
        timeBetweenAttacks = startTimeBetweenAttacks;
        PV = GetComponent<PhotonView>();
        agent = GetComponent<NavMeshAgent>();
        healthManager = GetComponent<HealthManager>();
    }

    void Update()
    {
        ChasePlayer();
        Attack();
        Die();
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
                if(currentEnemy.GetComponent<HealthManager>().isDead == false)
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
        if(healthManager.isDead)
        {
            if(PV.IsMine)
            {
                GiveReward(); 
                PhotonNetwork.Destroy(gameObject);
            }   
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
                    if(targetPlayer.GetComponent<HealthManager>().isDead == false)
                    {
                        targetPlayer.GetComponent<HealthManager>()?.TakeDamage(damage);
                    }
                }                                
            }            
        }
        else
        {
            timeBetweenAttacks+= Time.deltaTime;
        }
    }

    public void GiveReward()
    {
        PhotonNetwork.InstantiateSceneObject(Path.Combine("PhotonPrefabs" , rewardPrefab.name), transform.position, transform.rotation);
    }
}
