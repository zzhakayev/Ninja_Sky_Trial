using UnityEngine;
using UnityEngine.Audio;

// A Resources reference keeps the existing mixer in the build without moving it.
public class GameAudioConfig : ScriptableObject
{
    public AudioMixer mixer;
}
