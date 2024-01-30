using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    /*Sound()
    {
        name = "sound";
        loop = false;
        volume = 1;
        pitch = 1;
    }*/
    public string name;
    public AudioClip clip;
    public SoundType type = SoundType.SFX;
    public bool loop = false;
    [Range(0f, 1f)]
    public float volume = 1;
    [Range(0.1f, 3f)]
    public float pitch = 1;

    [HideInInspector]
    public AudioSource source;
}
public enum SoundType
{
    SFX, OST
}