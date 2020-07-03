using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public Image fadePlane;
    public GameObject gameOverUI;

    public RectTransform newWaveBanner;
    public Text newWaveTitle;
    public Text newWaveEnemyCount;
    public Text scoreUI;
    public Text gameoverScoreUI;
    public RectTransform healthBar;


    Spawner spawner;
    Player player;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        player.OnDeath += OnGameOver;
        spawner = FindObjectOfType<Spawner>();
        spawner.OnNewWave += OnNewWave;
    }

    private void Update()
    {
        scoreUI.text = ScoreKeeper.score.ToString("D6");
        float healthPercent = 0;
        if(player != null)
        {
            healthPercent = player.health / player.startingHealth;
        }
        healthBar.localScale = new Vector3(healthPercent, 1, 1);
    }

    void OnNewWave(int waveNumber)
    {
        string[] numbers = { "One", "Two", "Three", "Four", "Five" };
        newWaveTitle.text = "- Wave " + numbers[waveNumber - 1] + " -";

        string enemyCountString = ((spawner.waves[waveNumber - 1].infinite) ? "Enemies : Infinite" : "Enemies : " + spawner.waves[waveNumber - 1].enemyCount);
        newWaveEnemyCount.text = enemyCountString;

        //debug 모드에서 너무 빨리 넘어가면 문제 생김. 이런 경우 이미 진행중인 코루틴 중단.
        StopCoroutine("AnimateNewWaveBanner");
        StartCoroutine("AnimateNewWaveBanner");
    }

    IEnumerator AnimateNewWaveBanner()
    {
        float delayTime = 1.5f;
        float speed = 3f;
        float animatePercent = 0;
        int direction = 1;

        float endDelayTime = Time.time + 1 / speed + delayTime;

        while(animatePercent >= 0)
        {
            animatePercent += Time.deltaTime * speed * direction;
            if(animatePercent >= 1)
            {
                animatePercent = 1;
                if(Time.time > endDelayTime)
                {
                    direction = -1;
                }
            }

            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-170, 35, animatePercent);
            yield return null;
        }
    }

    void OnGameOver()
    {
        StartCoroutine(Fade(Color.clear, new Color(0f,0f,0f,0.9f),1));
        Cursor.visible = true;
        gameoverScoreUI.text = scoreUI.text;

        scoreUI.gameObject.SetActive(false);
        healthBar.gameObject.SetActive(false);
        gameOverUI.gameObject.SetActive(true);
    }

    IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;

        while(percent < 1)
        {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    //UI Input
    public void StartNewGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
