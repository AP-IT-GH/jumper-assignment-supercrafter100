using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

public class CubeAgentRaysChallenge1 : Agent
{
    public GameObject target;
    public GameObject goal;
    
    public float speedMultiplier = 0.1f;
    public float rotationMultiplier = 5;
    public bool hasFoundEnemy = false;

    public bool triggeredGoalEntry = false;

    public DateTime lastSuccessfulInteraction;
    
    public override void OnEpisodeBegin()
    {
        target.SetActive(true);
        // Reset position and orientation of our agent
        if (this.transform.position.y < 0)
        {
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
            this.transform.localRotation = Quaternion.identity;
        }

        target.transform.localPosition = new Vector3(Random.value * 6 - 4, 0.5f, Random.value * 8 - 4);
        hasFoundEnemy = false;
        triggeredGoalEntry = false;
        
        lastSuccessfulInteraction = DateTime.Now;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(hasFoundEnemy);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Movement
        Vector3 controlSignal = Vector3.zero;
        controlSignal.z = actions.ContinuousActions[0];
        transform.Translate(controlSignal * speedMultiplier);
        
        transform.Rotate(0.0f, rotationMultiplier * actions.ContinuousActions[1], 0.0f);
        
        // Reward
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, target.transform.localPosition);
        
        // Target reached
        if (distanceToTarget < 1.42f && !hasFoundEnemy)
        {
            SetReward(0.5f);
            target.SetActive(false);
            hasFoundEnemy = true;
            lastSuccessfulInteraction = DateTime.Now;
        }
        
        if (hasFoundEnemy && triggeredGoalEntry)
        {
            SetReward(1.0f);
            EndEpisode();
        }
        
        // Fell from the platform?
        else if (this.transform.localPosition.y < 0)
        {
            EndEpisode();
        }
        
        // Give negative reward if nothing happened for too long
        /*TimeSpan difference = DateTime.Now - lastSuccessfulInteraction;
        if (difference.TotalSeconds > 5)
        {
            SetReward(-0.5f);
            lastSuccessfulInteraction = DateTime.Now;
        }*/
        AddReward(-1f/MaxStep);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Vertical");
        continuousActionsOut[1] = Input.GetAxis("Horizontal");
    }

    private void OnTriggerEnter(Collider other)
    {
        triggeredGoalEntry = true;
    }

    private void OnTriggerExit(Collider other)
    {
        triggeredGoalEntry = false;
    }
}
