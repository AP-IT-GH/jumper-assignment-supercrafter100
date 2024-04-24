using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

public class CubeAgentJumper : Agent
{
    public GameObject obstacle;
    public Vector3 obstacleSpawnpoint;

    public float jumpMultiplier = 1;
    public int score = 0;
    public float minSpeed = 0.01f;
    public float maxSpeed = 0.1f;
    
    public override void OnEpisodeBegin()
    {

        foreach (Transform tf in transform.parent)
        {
            if (tf.gameObject.tag == "obstacle")
                Destroy(tf.gameObject);
        }
        
        SpawnObstacle();
        
        // Reset position and orientation of our agent
        transform.localPosition = new Vector3(0, 0, -13.19f);
        transform.localRotation = Quaternion.identity;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;

        score = 0;
    }

    public void SpawnObstacle()
    {
        GameObject createdObstacle = Instantiate(obstacle, gameObject.transform.parent);
        createdObstacle.transform.localPosition = obstacleSpawnpoint;
        createdObstacle.transform.localRotation = Quaternion.identity;
        
        createdObstacle.GetComponent<Obstacle>().speed = -Random.Range(minSpeed, maxSpeed);
        createdObstacle.GetComponent<Obstacle>().agent = gameObject;
    }

    public void JumpedOverObstacle()
    {
        score++;
        AddReward(1f);
        SpawnObstacle();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Movement
        int action = actions.DiscreteActions[0];

        if (action == 1 && IsGrounded())
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            Vector3 velocity = rigidbody.velocity;
            
            velocity.y += jumpMultiplier;
            rigidbody.velocity = velocity;
            AddReward(-0.1f);
        }

        if (transform.position.y < 0)
        {
            // Death
            AddReward(-1f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = Input.GetMouseButtonDown(0) ? 1 : 0;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "obstacle")
        {
            // Agent hit the obstacle, restart
            Destroy(other.gameObject);
            AddReward(-1f);
            EndEpisode();
        }
    }
    
    public bool IsGrounded() {
        RaycastHit hit;
        float rayLength = 1.1f; // Adjust based on your character's size
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength)) {
            return true;
        }
        return false;
    }
}
