using UnityEngine.Audio;
using UnityEngine;
public class SettingsMenu : MonoBehaviour
{
    public AudioMixer mixer;
    public void SetVolume (float volume)
    {
        mixer.SetFloat("MasterVolume", volume);
    }
    public void SetVolOST (float volume)
    {
        mixer.SetFloat("OST", volume);
    }
    public void SetVolSFX (float volume)
    {
        mixer.SetFloat("SFX", volume);
    }
}
