using System;
using System.Linq;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AuditoryAgent : Agent
{
    private AudioMemory _audioMemory;
    [SerializeField] private Transform _targetTransform;
    void Start()
    {
        _audioMemory = GetComponent<AudioMemory>();
    }

    public override void OnEpisodeBegin()
    {
        // Randomize the agent and target positions
        transform.localPosition = new Vector3(UnityEngine.Random.Range(-5f, 5f), -1.5f, UnityEngine.Random.Range(-5f, 5f));
        _targetTransform.localPosition = new Vector3(UnityEngine.Random.Range(-5f, 5f), 0.5f, UnityEngine.Random.Range(-5f, 5f));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float motion = actions.ContinuousActions[0];
        float rotation = actions.ContinuousActions[1];

        transform.position += transform.forward * Time.deltaTime * motion * 5f;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, rotation * 3, 0));
        float distanceToTarget = Vector3.Distance(transform.localPosition, _targetTransform.localPosition);

        // Punish the agent for being far from the target (also punishes the agent for excessive movement)
        AddReward(-distanceToTarget * Time.deltaTime);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        // Forward and backward movement
        continuousActions[0] = Input.GetAxis("Vertical");

        // Rotation
        continuousActions[1] = Input.GetAxis("Horizontal");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Normalize the audio data

        float[] leftEarObservations = NormalizeArray(_audioMemory.AudioMemoryLeft);
        float[] rightEarObservations = NormalizeArray(_audioMemory.AudioMemoryRight);

        // Add the audio data to the observation
        foreach (float observation in leftEarObservations)
        {
            sensor.AddObservation(observation);
        }

        foreach (float observation in rightEarObservations)
        {
            sensor.AddObservation(observation);
        }
    }

    public float[] NormalizeArray(float[] array)
    {
        float maximumAmplitude = array.Max(Math.Abs);

        // To avoid dividing by zero if there were no audio
        if (maximumAmplitude == 0)
        {
            return array;
        }

        return array.Select(x => x / maximumAmplitude).ToArray();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            SetReward(1f);
            EndEpisode();
        }
    }
}
