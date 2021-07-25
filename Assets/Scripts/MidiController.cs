using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidiController : MonoBehaviour
{
    public List<AudioClip> padAudioClips = new();

    // Start is called before the first frame update
    public void PlayPad(int index, AudioSource source)
    {
        source.PlayOneShot(padAudioClips[index]);
    }
}
