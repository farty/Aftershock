using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;


public class LevelController : MonoBehaviourPunCallbacks

{
    public enum SpawnState {SPAWNING, WAITING, COUNTING};
    [System.Serializable]
    public class Wave
    {
        public string name;
        public Transform enemy;
        public int count;
        public float rate;
    }
    public GameObject[] spawnPoints;

    public Wave[] waves;
    private int nextWave;

    public float timeBetweenWaves = 5f;
    public float waveCountown;


    private SpawnState state = SpawnState.COUNTING;
    void Start()
    {
        waveCountown = timeBetweenWaves;
    }

    void Update() 
    {
        if(state == SpawnState.WAITING)
        {
            if(!EnemyIsAlive())
            {
                WaveCompleted();
                return;
            }

            else
            {
                return;
            }
        }


        if(waveCountown <= 0)
        {
            if(state != SpawnState.SPAWNING)
            {
                StartCoroutine(SpawnWave(waves[nextWave]));
            }
        }

        else
        {
            waveCountown -= Time.deltaTime;
        }

        void WaveCompleted()
        {
            state = SpawnState.COUNTING;
            waveCountown = timeBetweenWaves;            
            
            if(nextWave+1 > waves.Length-1)
            {
                nextWave = 0;
                Debug.Log("All waves completed");
            }
            else
            {
                 nextWave++;
            }
        }
        bool EnemyIsAlive()
        {
            if(GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                return false;
            }

            return true;
        }
    }

    IEnumerator SpawnWave(Wave _wave)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            state = SpawnState.SPAWNING;
            Debug.Log("Spawning wave"+ _wave.name);
            for(int i = 0; i<_wave.count; i++)
            {
                SpanwEnemy(_wave.enemy);
                yield return new WaitForSeconds (1f/_wave.rate);
            }

        state = SpawnState.WAITING;
        yield break;

        }
        
    }
    void SpanwEnemy(Transform _enemy)
    {
        int i = Random.Range(0, spawnPoints.Length);
        Vector3 spawn = spawnPoints[i].transform.position;
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs",_enemy.name),spawn, Quaternion.identity);
        Debug.Log("Spawning enemy" + _enemy.name);
        
    }
}
