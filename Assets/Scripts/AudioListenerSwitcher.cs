using System.Collections;
using UnityEngine;

/// <summary>
/// Only one AudioListenerSwitcher should be present in the scene.
/// This script switches between AudioListeners, enabling one at a time and,
/// storing the audio data in AudioMemory components attached to the AudioListeners.
/// 
/// The script assumes that all the AudioListener components are attached to GameObjects
/// that also have AudioMemory components.
/// </summary>
public class AudioListenerSwitcher : MonoBehaviour
{
    private AudioListener[] _audioListeners;
    private AudioMemory[] _audioMemories;
    private int _audioListenerCount;
    private readonly float _sampleRate = 44100f;
    private readonly int _sampleCount = 1024;

    void Start()
    {

        _audioListeners = FindObjectsOfType<AudioListener>();
        _audioMemories = new AudioMemory[_audioListeners.Length];
        for (int i = 0; i < _audioListeners.Length; i++)
        {
            _audioMemories[i] = _audioListeners[i].GetComponent<AudioMemory>();
        }
        _audioListenerCount = _audioListeners.Length;

        if (_audioListenerCount == 1)
        {
            _audioListeners[0].enabled = true;
        }
        else
        {
            for (int i = 1; i < _audioListenerCount; i++)
            {
                _audioListeners[i].enabled = false;
            }
            StartCoroutine(AudioListenerSwitchCoroutine());
        }

    }

    /// <summary>
    /// Switches between audio listeners, enabling one at a time
    /// </summary>
    /// <returns></returns>
    IEnumerator AudioListenerSwitchCoroutine()
    {
        while (true)
        {
            for (int i = 0; i < _audioListenerCount; i++)
            {
                _audioListeners[i].enabled = true;
                yield return new WaitForSeconds(_sampleCount / _sampleRate + 0.01f); // 0.01f is a small buffer to prevent overlap
                AudioListener.GetOutputData(_audioMemories[i].AudioMemoryLeft, 0);
                AudioListener.GetOutputData(_audioMemories[i].AudioMemoryRight, 1);
                _audioListeners[i].enabled = false;
            }
        }
    }
}
