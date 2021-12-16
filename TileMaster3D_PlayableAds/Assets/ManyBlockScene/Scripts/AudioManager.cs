using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public List<AudioSource> audioSources;
    private int _index = -1;


    public void AddAudio(List<AudioSource> list)
    {
        audioSources = list;
    }
    
    public AudioSource SearchAudio()
    {
        _index++;
        _index %= 5;
        return audioSources[_index];
    }
}
