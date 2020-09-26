using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public bool isGameOver, isGameStarted;
    public Text eggCountText, gameOverEggText;
    public int eggCount;
    
    public static GameManager Instance;

    private float startPos, endPos;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        Application.targetFrameRate = 60;

        eggCount = 0;
        eggCountText.text = "x" + eggCount;

    }
    private void Start()
    {
    }

    private void Update()
    {
    }

    public void StartGame()
    {
        isGameOver = false;
        isGameStarted = true;
    }

    public void GameOver()
    {
        if (!isGameOver && isGameStarted)
        {
            CameraController.Instance.virtualCamera.m_Follow = null;
            Shake.instance.StartShake();
            isGameOver = true;

            MenuController.Instance.OpenPanel(MenuController.Instance.gameOverPanel);
            SoundManager.Instance.PlaySound(SoundManager.Instance.gameOverClip);
            gameOverEggText.text = "x" + eggCount;
        }
    }
    
    IEnumerator WaitForOpenPanel(GameObject panel, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        panel.SetActive(true);
    }

    public void CollectEgg()
    {
        eggCount++;
        eggCountText.text = "x"+ eggCount;
    }

    public void RestartGame()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
