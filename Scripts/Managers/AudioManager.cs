using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public Sound[] sounds;

    void Start()
    {
        Play("Theme");
    }
    void Update()
    {

    }

    void Awake()
    {
        // Singleton
        if (instance == null)
            instance = this;
        else
        {
            Debug.Log("More than one Instance of AudioManager!");
            Destroy(gameObject);
            return;
        }

        // DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = false;
        }
    }
    public bool Play (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log($"Sound: {name} does not exist !");
            return false;
        }
        s.source.Play();
        return true;
    }
    public void PlayAtRandom (string name)
    {
        List<Sound> list = new();
        foreach(Sound s in sounds)
            if (s.name.Contains(name))
                list.Add(s);

        Play(list[UnityEngine.Random.Range(0, list.Count)].name);
    }
    public void SetSoundVolume(SoundType type, float vol)
    {
        // vol = (0; 1)
        foreach (Sound s in sounds)
        {
            if (s.type == type)
            {
                s.volume = vol;
            }
        }
    }
}