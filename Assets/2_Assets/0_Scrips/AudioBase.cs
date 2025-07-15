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
    public AudioClip[] audioSoundPlayer;
    // Enemy
    public AudioClip[] audioSoundEnemy;
    // GameOver
    public AudioClip[] audioSoundGPL;
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
    public void StopMusic()
    {
        audioBgrMussic.Stop();
    }
    public void SetVolumeSound(float volume)
    {
        audioBgrSoundUI.volume = volume;
        audioBgrSoundGPL.volume = volume;
        PlayerPrefs.SetFloat("Sound", volume);
        PlayerPrefs.Save();
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
        audioBgrSoundGPL.PlayOneShot(audioSoundPlayer[count]);
    }
    public void AudioEnemy(int count)
    {
        audioBgrSoundGPL.PlayOneShot(audioSoundEnemy[count]);
    }
    public void AudioPlayerAtkHit()
    {
        audioBgrSoundGPL.PlayOneShot(audioSoundPlayer[11]);
    }
}
