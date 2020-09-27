using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject menuPanel, gameplayPanel, gameOverPanel, helpPanel, teamPanel, settingsPanel;
    public CanvasGroup tutorialPanel;
    public RectTransform logo;
    public Image soundImage, musicImage;
    public Sprite soundOn, soundOff, musicOn, musicOff;

    private bool settingsOpen = false;

    public static MenuController Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
        
        logo.DOPunchScale(new Vector3(0.04f, 0.04f, 0.04f), 1f, 1, 1f).SetEase(Ease.Linear).SetLoops(-1);
        logo.DOAnchorPosY(0f, 0.4f).SetEase(Ease.OutBack);

        tutorialPanel.alpha = 0f;
        StartCoroutine(openMenu());
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("Music") == 1 && GameObject.FindGameObjectWithTag("MenuMusic") == null)
            PlayMusic(SoundManager.Instance.menuMusic);

        updateSoundImages();
    }

    public void StartGame()
    {
        tutorialPanel.DOFade(1f, 1f).SetEase(Ease.Linear);
        tutorialPanel.DOFade(0f, 1f).SetEase(Ease.Linear).SetDelay(4f);

        menuPanel.SetActive(false);
        gameplayPanel.SetActive(true);
        GameManager.Instance.StartGame();
    }
    
    IEnumerator openMenu()
    {
        gameplayPanel.SetActive(false);
        menuPanel.SetActive(true);

        gameOverPanel.SetActive(false);
        gameOverPanel.transform.GetChild(0).localScale = Vector3.zero;

        helpPanel.SetActive(false);
        helpPanel.transform.GetChild(0).localScale = Vector3.zero;

        settingsPanel.SetActive(false);
        settingsPanel.transform.GetChild(0).localScale = Vector3.zero;

        teamPanel.SetActive(false);
        teamPanel.transform.GetChild(0).localScale = Vector3.zero;
        yield return new WaitForSeconds(0.1f);
    }

    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
        panel.transform.GetChild(0).DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
    }

    public void closePanel(GameObject panel) { StartCoroutine(closePanelCoroutine(panel)); }

    IEnumerator closePanelCoroutine(GameObject panel)
    {
        panel.transform.GetChild(0).GetComponent<RectTransform>().DOScale(0f, 0.4f).SetEase(Ease.InBack);
        yield return new WaitForSeconds(0.4f);
        panel.SetActive(false);
    }

    public void buttonUp(Button button)
    {
        button.transform.DOScale(1f, 0.1f);
    }

    public void buttonDown(Button button)
    {
        if (button.interactable)
        {
            button.transform.DOScale(0.9f, 0.1f);
            SoundManager.Instance.PlaySound(SoundManager.Instance.buttonClip);
        }
    }

    public void ChangeSoundStatus()
    {
        SoundManager.Instance.ChangeSoundStatus();
        updateSoundImages();
    }

    public void ChangeMusicStatus()
    {
        SoundManager.Instance.ChangeMusicStatus();
        updateSoundImages();
    }

    public void PlaySound(AudioClip clip)
    {
        SoundManager.Instance.PlaySound(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        SoundManager.Instance.PlayMusic(clip);
    }

    public void updateSoundImages()
    {
        if (PlayerPrefs.GetInt("Sound") == 1)
            soundImage.sprite = soundOn;
        else
            soundImage.sprite = soundOff;

        if (PlayerPrefs.GetInt("Music") == 1)
            musicImage.sprite = musicOn;
        else
            musicImage.sprite = musicOff;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}