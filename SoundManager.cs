using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    AudioSource BGM;
    AudioSource UI;
    AudioSource SFX;

    void Start()
    {
        GameObject obj = new GameObject("BGM");
        BGM = obj.AddComponent<AudioSource>();
        BGM.transform.SetParent(transform);
        BGM.volume = PlayerPrefs.GetFloat("BGM");

        obj = new GameObject("UI");
        UI = obj.AddComponent<AudioSource>();
        UI.transform.SetParent(transform);
        UI.volume=PlayerPrefs.GetFloat("UI");

        obj = new GameObject("SFX");
        SFX = obj.AddComponent<AudioSource>();
        SFX.transform.SetParent(transform);
        SFX.volume=PlayerPrefs.GetFloat("SFX");
    }

    public void SetVolume(AudioChannel channel, float value)
    {
        PlayerPrefs.SetFloat(channel.ToString(), value);
        switch (channel)
        {
            case AudioChannel.BGM:
                BGM.volume = value;
                break;
            case AudioChannel.UI:
                UI.volume = value;
                break;
            case AudioChannel.SFX:
                SFX.volume = value;
                break;
        }
    }

    public void SetSound(AudioChannel channel, string name)
    {
        switch (channel)
        {
            case AudioChannel.BGM:
                if (BGM.clip != null&&BGM.clip.name.CompareTo(name) != 0)
                {
                    BGM.Stop();
                    BGM.clip = Resources.Load<AudioClip>($"Sound/{name}");
                    BGM.Play();
                }
                break;
            case AudioChannel.UI:
                if (UI.clip != null && UI.clip.name.CompareTo(name) != 0)
                {
                    UI.Stop();
                    UI.clip = Resources.Load<AudioClip>($"Sound/{name}");
                    UI.Play();
                }
                break;
            case AudioChannel.SFX:
                if (SFX.clip != null && SFX.clip.name.CompareTo(name) != 0)
                {
                    SFX.Stop();
                    SFX.clip = Resources.Load<AudioClip>($"Sound/{name}");
                    SFX.Play();
                }
                break;
        }
    }
}
