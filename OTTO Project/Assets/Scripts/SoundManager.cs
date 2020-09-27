using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{

    public AudioClip gameStartedClip;
    public AudioClip gameOverClip;
    public AudioClip shipKidnapClip;
    public AudioClip shipMovingClip;
    public AudioClip menuMusic;
    public AudioClip buttonClip;
    public AudioClip grabClip;
    public AudioClip dropClip;

    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
            Destroy(gameObject);

        if (!PlayerPrefs.HasKey("Music"))
            PlayerPrefs.SetInt("Music", 1);
        if (!PlayerPrefs.HasKey("Sound"))
            PlayerPrefs.SetInt("Sound", 1);

    }

    public void PlaySound(AudioClip audioClip)
    {
        if (PlayerPrefs.GetInt("Sound") == 1)
        {
            GameObject soundGameObject = new GameObject(audioClip.name);
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();

            audioSource.PlayOneShot(audioClip);
            Destroy(soundGameObject, audioClip.length);
        }
    }

    public void PlayMusic(AudioClip audioClip)
    {
        if (PlayerPrefs.GetInt("Music") == 1)
        {
            GameObject soundGameObject = new GameObject(audioClip.name);
            DontDestroyOnLoad(soundGameObject);
            soundGameObject.tag = "MenuMusic";
            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();

            audioSource.loop = true;
            audioSource.volume = 0.5f;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }

    public void ChangeSoundStatus()
    {
        PlayerPrefs.SetInt("Sound", PlayerPrefs.GetInt("Sound") == 0 ? 1 : 0);
    }

    public void ChangeMusicStatus()
    {
        PlayerPrefs.SetInt("Music", PlayerPrefs.GetInt("Music") == 0 ? 1 : 0);

        if (PlayerPrefs.GetInt("Music") == 0)
            GameObject.FindGameObjectWithTag("MenuMusic").GetComponent<AudioSource>().volume = 0;
        else
            GameObject.FindGameObjectWithTag("MenuMusic").GetComponent<AudioSource>().volume = 0.5f;
    }
}