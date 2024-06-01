using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Transform audioLibrary;
    public AudioMixerGroup masterGroup;
    public Sound[] sounds;
    public bool intro = false;
    private int curThemeIndex;

    void Start()
    {
        foreach (Sound s in sounds)
        {
            s.source = audioLibrary.gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = false;
            if (s.group != null)
                s.source.outputAudioMixerGroup = s.group;
            else
                s.source.outputAudioMixerGroup = masterGroup;
        }

        if (GameManager.instance != null)
            PlayTheme("theme" + GameManager.instance.level);
        else if (intro)
            PlayTheme("theme");
    }
    public void PauseTheme()
    {
        if (sounds[curThemeIndex].paused)
            sounds[curThemeIndex].source.UnPause();
        else
            sounds[curThemeIndex].source.Pause();

        sounds[curThemeIndex].paused = !sounds[curThemeIndex].paused;
    }
    public bool Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null || !s.type.Equals(SoundType.SFX))
        {
            Debug.Log($"SFX: {name} does not exist !");
            return false;
        }
        s.source.Play();
        s.paused = false;
        return true;
    }
    public bool PlayTheme(string name)
    {
        if (name != "stop")
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            if (s == null || !s.type.Equals(SoundType.OST))
            {
                Debug.Log($"OST: {name} does not exist !");
                return false;
            }
            sounds[curThemeIndex].source.Stop();
            // Not Optimal
            curThemeIndex = sounds.ToList().IndexOf(s);
            s.paused = false;
            s.source.Play();
        }
        else if (sounds.Length > curThemeIndex)
            if (sounds[curThemeIndex].source != null)
                sounds[curThemeIndex].source.Stop();

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
    public void PlayAtRandom(string name, int bottom, int upper = -1)
    {
        List<Sound> list = new();
        foreach (Sound s in sounds)
            if (s.name.Contains(name))
                list.Add(s);

        if (upper < 0)
            Play(list[UnityEngine.Random.Range(bottom, list.Count)].name);
        else
            Play(list[UnityEngine.Random.Range(bottom, upper)].name);

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