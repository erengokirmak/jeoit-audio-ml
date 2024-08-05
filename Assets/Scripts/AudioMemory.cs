using UnityEngine;

/// <summary>
/// This script stores the audio data from the AudioListener component.
/// </summary>
public class AudioMemory : MonoBehaviour
{
    public float[] AudioMemoryLeft = new float[1024];
    public float[] AudioMemoryRight = new float[1024];

    void Start()
    {
        for (int i = 0; i < 1024; i++)
        {
            AudioMemoryLeft[i] = 0;
            AudioMemoryRight[i] = 0;
        }
    }
}
