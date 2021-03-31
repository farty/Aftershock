using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{   
    public int speed = 10;
    public int damage = 10;
    void Update()
    {
        transform.position = transform.position + transform.forward*speed*Time.deltaTime;
        Destroy(gameObject, 5);
    }
}
