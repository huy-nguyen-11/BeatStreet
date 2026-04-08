using UnityEngine;

public class AudioBase : MonoBehaviour
{

    public static AudioBase Instance { get; private set; }
    public AudioSource audioBgrMussic;
    public AudioSource audioBgrSoundUI;
    public AudioSource audioBgrSoundGPL;

    public AudioClip audioMusicUI;
    public AudioClip[] audioMusicGPL;
    public AudioClip[] audioSoundUI;
    // Player
    public AudioClip[] audioSoundPlayer0;
    // Enemy
    public AudioClip[] audioSoundEnemy;
    // GameOver
    public AudioClip[] audioSoundGPL;
    //for daily reward
    public bool isCheckPlayed;

    //additional : field open level map
    public bool isOpenLevel = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        // Apply saved on/off and volume settings immediately so audio state is correct
        ApplySavedAudioSettings();
    }
    private void OnEnable()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {

    }
    public void GetMussic()
    {
        audioBgrMussic.volume = PlayerPrefs.GetFloat("Music");
        audioBgrSoundUI.volume = PlayerPrefs.GetFloat("Sound");
        audioBgrSoundGPL.volume = PlayerPrefs.GetFloat("Sound");
    }

    // New: apply saved on/off and volumes
    public void ApplySavedAudioSettings()
    {
        // volume defaults
        float musicVol = PlayerPrefs.GetFloat("Music", 1f);
        float soundVol = PlayerPrefs.GetFloat("Sound", 1f);

        // on/off flags default to ON (1)
        bool musicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        bool sfxOn = PlayerPrefs.GetInt("SFXOn", 1) == 1;

        if (audioBgrMussic != null)
        {
            audioBgrMussic.volume = musicVol;
            audioBgrMussic.mute = !musicOn;
        }

        if (audioBgrSoundUI != null)
        {
            audioBgrSoundUI.volume = soundVol;
            audioBgrSoundUI.mute = !sfxOn;
        }

        if (audioBgrSoundGPL != null)
        {
            audioBgrSoundGPL.volume = soundVol;
            audioBgrSoundGPL.mute = !sfxOn;
        }
    }

    // Convenience helpers to toggle and persist settings
    public void ToggleMusic(bool on)
    {
        PlayerPrefs.SetInt("MusicOn", on ? 1 : 0);
        PlayerPrefs.Save();
        if (audioBgrMussic != null)
            audioBgrMussic.mute = !on;
    }

    public void ToggleSFX(bool on)
    {
        PlayerPrefs.SetInt("SFXOn", on ? 1 : 0);
        PlayerPrefs.Save();
        if (audioBgrSoundUI != null)
            audioBgrSoundUI.mute = !on;
        if (audioBgrSoundGPL != null)
            audioBgrSoundGPL.mute = !on;
    }

    public void SetMusicUI()
    {
        audioBgrMussic.clip = audioMusicUI;
        audioBgrMussic.Play();
    }
    public void SetMusicGPL(int count)
    {
        audioBgrMussic.clip = audioMusicGPL[count];
        audioBgrMussic.Play();
    }
    public void SetVolumeMusic(float volume)
    {
        audioBgrMussic.volume = volume;
        PlayerPrefs.SetFloat("Music", volume);
        PlayerPrefs.Save();
    }

    public void SetVolumeSound(float volume)
    {
        if (audioBgrSoundUI != null) audioBgrSoundUI.volume = volume;
        if (audioBgrSoundGPL != null) audioBgrSoundGPL.volume = volume;
        PlayerPrefs.SetFloat("Sound", volume);
        PlayerPrefs.Save();
    }
    public void StopMusic()
    {
        audioBgrMussic.Stop();
    }

    public void SetBackgroundVolumeIfNotMuted(float volume, bool saveToPrefs = false)
    {
        if (audioBgrMussic == null) return;
        if (!audioBgrMussic.mute)
        {
            audioBgrMussic.volume = volume;
            if (saveToPrefs)
            {
                PlayerPrefs.SetFloat("Music", volume);
                PlayerPrefs.Save();
            }
        }
    }

    public void SetAudioUI(int count)
    {
        audioBgrSoundUI.PlayOneShot(audioSoundUI[count]);
    }
    public void AudioGPl(int count)
    {
        audioBgrSoundUI.PlayOneShot(audioSoundGPL[count]);
    }
    public void AudioPlayer(int count)
    {
        audioBgrSoundGPL.PlayOneShot(audioSoundPlayer0[count]);
    }
    public void AudioEnemy(int count)
    {
        audioBgrSoundGPL.PlayOneShot(audioSoundEnemy[count]);
    }
    public void AudioPlayerAtkHit()
    {
        //audioBgrSoundGPL.PlayOneShot(audioSoundPlayer0[11]);
    }
}
