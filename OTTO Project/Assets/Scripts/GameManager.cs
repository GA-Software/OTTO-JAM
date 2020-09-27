using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public bool isGameOver, isGameStarted;
    public Text eggCountText, gameOverEggText, chickenCountText, gameOverChickenText, instructText;
    public int eggCount;
    public List<Player> chickens;
    public GameObject particleEffect;
    
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
        instructText.GetComponent<RectTransform>().DOPunchScale(new Vector3(0.04f, 0.04f, 0.04f), 1f, 1, 1f).SetEase(Ease.Linear).SetLoops(-1);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
            ScreenCapture.CaptureScreenshot("FarmDefense" + Random.Range(1, 1000) + ".png");
    }

    private void Start()
    {
        chickens.AddRange(FindObjectsOfType<Player>());
        ControlChickenCount();
    }

    public IEnumerator playParticle(Vector3 position)
    {
        GameObject GO = Instantiate(particleEffect, position - Vector3.up, particleEffect.transform.rotation);
        yield return new WaitForSeconds(1f);
        Destroy(GO);
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

            MenuController.Instance.gameplayPanel.SetActive(false);
            MenuController.Instance.OpenPanel(MenuController.Instance.gameOverPanel);
            SoundManager.Instance.PlaySound(SoundManager.Instance.gameOverClip);
            gameOverEggText.text = "x" + eggCount;
            gameOverChickenText.text = "x" + chickens.Count;
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

    public void ControlChickenCount()
    {
        chickenCountText.text = "x" + chickens.Count;
    }

    public void RestartGame()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
