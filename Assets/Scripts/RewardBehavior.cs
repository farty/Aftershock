using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardBehavior : MonoBehaviour
{
    public int reward = 10;
    bool goingUp = true;
    float startTween;
    float endTween;
    void Start()
    {
        startTween = transform.position.y;
        endTween = startTween+1;
    }
    void Update()
    {
        Animator();
    }

    void Animator()
    {
        transform.Rotate(0, Time.deltaTime * 360,  0, Space.Self);
        if(goingUp)
        {
            transform.Translate(0, 1f*Time.deltaTime, 0); 
            if(transform.position.y > endTween)
            {
                goingUp = false;
            }
        }
        else
        {
            transform.Translate(0, -1f*Time.deltaTime, 0);
            if(transform.position.y < startTween)
            {
                goingUp = true;
            }
        }
    }
    void OnTriggerEnter (Collider other)
    {
        if(other.GetComponent<PlayerController>())
        {
             other.GetComponent<PlayerController>()?.GetReward(reward);
             Destroy(gameObject);
        }
    }
        

}
