using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float speed = -0.1f;
    public GameObject agent;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 velocity = rb.velocity;
        velocity.z = speed;
        rb.velocity = velocity;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag.Equals("ColliderWall"))
        {
            agent.GetComponent<CubeAgentJumper>().JumpedOverObstacle();
            
            // Destroy ourself
            Destroy(gameObject);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag.Equals("ColliderWall"))
        {
            agent.GetComponent<CubeAgentJumper>().JumpedOverObstacle();
            
            // Destroy ourself
            Destroy(gameObject);
        }
    }
}
