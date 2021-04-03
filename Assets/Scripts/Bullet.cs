using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{   
    public int speed = 10;
    public int damage = 10;

    Vector3 mPrevPos;
    void Start()
    {
        mPrevPos = transform.position;
    }
    void Update()
    {
        mPrevPos = transform.position;
        transform.Translate(0.0f, 0.0f, speed*Time.deltaTime);
        RaycastHit[] hits = Physics.RaycastAll(new Ray(mPrevPos, (transform.position - mPrevPos).normalized), (transform.position - mPrevPos).magnitude);
        for(int i = 0; i< hits.Length; i++ )
        {
            hits[i].collider.gameObject.GetComponent<PlayerController>()?.TakeDamage(damage);
            hits[i].collider.gameObject.GetComponent<enemy_1_behavior>()?.TakeDamage(damage);
            if(i>0)
            {
                Destroy(gameObject);
            }
        }
        Destroy(gameObject, 5);
    }
}
