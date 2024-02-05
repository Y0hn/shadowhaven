using UnityEngine.Audio;
using UnityEngine;
public class SettingsMenu : MonoBehaviour
{
    public AudioMixer mixer;
    public void SetVolume (float volume)
    {
        mixer.SetFloat("MasterVolume", volume);
    }
}
