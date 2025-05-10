using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [Header("Countdown")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private int countDownTime;
    [SerializeField] private Transform buttonsContainer;

    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Ball")]
    [SerializeField] private float startXPos = -0.5f;
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private float[] movePoints;

    [Header("PausePanel")]
    [SerializeField] private GameObject pausePanel;

    [Header("GameOver Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI currentResultText;
    [SerializeField] private TextMeshProUGUI bestResultText;

    [Header("Background")]
    [SerializeField] private Image bgImage;

    [Header("Spawner")]
    [SerializeField] private GameObject[] wallPrefabs;
    [SerializeField] private GameObject[] waweObjectPrefabs;
    [SerializeField] private float waveObjectSpawnChance = 0.3f;
    [SerializeField] private float timeToWaitForSpawnWaveObject = 1f;
    [SerializeField] private Wave[] waves;
    [SerializeField] private float spawnTime;
    [SerializeField] private Vector3 startScale;
    [SerializeField] private Vector3 endScale;

    [Header("Effects")]
    [SerializeField] private GameObject fireEffect;
    [SerializeField] private Transform fireSpawnPoint;
    [SerializeField] private GameObject explosionEffect;

    private BallData selectedBall;
    private LevelData selectedLevel;

    private float elapsedTime = 0f;
    private bool isTimerRunning;

    private List<Image> buttonsImages = new List<Image>();
    private bool canInput = false;
    private bool isGameOver = false;

    private int currentScore = 0;

    private void Awake()
    {
        selectedLevel = Array.Find(Resources.LoadAll<LevelData>("LevelDatas"), l => l.levelNumber - 1 == PlayerPrefs.GetInt("SelectedLevel", 0));
        selectedBall = Array.Find(Resources.LoadAll<BallData>("BallDatas"), b => b.BallNumber - 1 == PlayerPrefs.GetInt("SelectedBall", 0));
    }

    private void Start()
    {
        gameOverPanel.SetActive(false);

        Unpause();

        SoundManager.Instance.PlayBackgroundMusic(selectedLevel.backgroundClip);
        GetComponent<SpriteRenderer>().sprite = selectedBall.ballSprite;
        bgImage.sprite = selectedLevel.backgroundSprite;

        transform.position = new Vector3(startXPos, transform.localPosition.y, transform.localPosition.z);

        for (int i = 0; i < buttonsContainer.childCount; i++)
        {
            Button button = buttonsContainer.GetChild(i).GetComponent<Button>();
            float xPoint = movePoints[i];
            button.onClick.AddListener(() => MoveBall(xPoint));

            Image image = buttonsContainer.GetChild(i).GetComponent<Image>();
            buttonsImages.Add(image);
        }

        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        canInput = false;

        countdownText.gameObject.SetActive(true);

        for (int i = countDownTime; i > 0; i--)
        {
            countdownText.text = $"{i}...";
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "GO!";
        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);
        foreach (Image image in buttonsImages)
        {
            image.color = new Color(0, 0, 0, 0);
        }

        canInput = true;

        StartCoroutine(Spawn());
        StartCoroutine(StartTimer());
    }

    private IEnumerator Spawn()
    {
        while(!isGameOver)
        {
            Wave wave = waves[Random.Range(0, waves.Length)];

            GameObject wallObject = Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Length)], wave.startPoint.position, Quaternion.identity);

            SetUpWaveObject(ref wallObject, wave);

            if (Random.value <= waveObjectSpawnChance)
            {
                StartCoroutine(SpawnWaveObject(wave));
            }

            yield return new WaitForSeconds(spawnTime);
        }
    }

    private void SetUpWaveObject(ref GameObject newObject, Wave wave)
    {
        newObject.transform.localScale = startScale;

        if (wave.isMirror)
            newObject.transform.localScale = new Vector3(-newObject.transform.localScale.x, newObject.transform.localScale.y, newObject.transform.localScale.z);

        WaveObject waveObject = newObject.GetComponent<WaveObject>();
        waveObject.ChangeWallColor(selectedLevel.obstacleColor);
        waveObject.MoveTo(wave.endPoint.position);
        waveObject.ScaleTo(new Vector3(endScale.x * Mathf.Sign(newObject.transform.localScale.x), endScale.y, endScale.z));
    }

    private IEnumerator SpawnWaveObject(Wave wave)
    {
        yield return new WaitForSeconds(timeToWaitForSpawnWaveObject);

        if (isGameOver)
            yield break;

        GameObject waveObject = Instantiate(waweObjectPrefabs[Random.Range(0, waweObjectPrefabs.Length)], wave.startPoint.position, Quaternion.identity);
        SetUpWaveObject(ref waveObject, wave);
    }

    private IEnumerator StartTimer()
    {
        isTimerRunning = true;
        elapsedTime = 0f;

        while (isTimerRunning)
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);

            timerText.text = $"{minutes}:{seconds:D2}";

            yield return new WaitForSeconds(1f);

            elapsedTime++;

            currentScore += 1000;
        }
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    public void ResetTimer()
    {
        isTimerRunning = false;
        elapsedTime = 0f;
        timerText.text = "0:00";
    }

    private void MoveBall(float x)
    {
        if (!canInput)
            return;

        canInput = false;

        transform.DOMove(new Vector3(x, transform.position.y, transform.position.z), moveDuration)
                 .SetEase(Ease.InOutQuad)
                 .OnComplete(() => canInput = true);
    }

    public void Pause()
    {
        pausePanel.SetActive(true);
        canInput = false;
        Time.timeScale = 0;
    }

    public void Unpause()
    {
        pausePanel.SetActive(false);
        canInput = true;
        Time.timeScale = 1;
    }

    public void AddScore(int points)
    {
        currentScore += points;
        if (currentScore < 0)
            currentScore = 0;
    }

    public void MultiplyScore(int multiplier)
    {
        Instantiate(fireEffect, fireSpawnPoint.position, Quaternion.identity);
        currentScore *= multiplier;
        if (currentScore < 0)
            currentScore = 0;
    }

    public void GameOver()
    {
        Instantiate(explosionEffect, transform.position, Quaternion.identity);

        canInput = false;
        isGameOver = true;

        transform.DOKill();

        StopTimer();

        WaveObject[] waveObjects = FindObjectsOfType<WaveObject>();

        foreach (WaveObject waveObject in waveObjects)
        {
            waveObject.StopAnimation();
        }

        string currentTime = timerText.text;
        string bestTime = PlayerPrefs.GetString("Time", "0:00");            

        int currentTimeSeconds = TimeToSeconds(currentTime);
        int bestTimeSeconds = TimeToSeconds(bestTime);

        if (currentTimeSeconds > bestTimeSeconds || bestTimeSeconds == 0)
        {
            bestTime = currentTime;
            PlayerPrefs.SetString("Time", currentTime);
        }

        int bestScore = PlayerPrefs.GetInt("Score", 0);

        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            PlayerPrefs.SetInt("Score", currentScore);
        }
        
        UpdateResult(ref currentResultText, currentTime, currentScore);
        UpdateResult(ref bestResultText, bestTime, bestScore);

        gameOverPanel.SetActive(true);

        SoundManager.Instance.Vibrate();
    }

    public static void UpdateResult(ref TextMeshProUGUI resultText, string time, int score)
    {
        string[] timeParts = time.Split(':');
        int seconds = timeParts.Length > 0 ? int.Parse(timeParts[0]) : 0;
        int minutes = timeParts.Length > 1 ? int.Parse(timeParts[1]) : 0; ;
        resultText.text = $"{seconds} minute {minutes} seconds \n" +
            $"{score} points";
    }

    private int TimeToSeconds(string time)
    {
        string[] parts = time.Split(':');
        int minutes = int.Parse(parts[0]);
        int seconds = int.Parse(parts[1]);
        return minutes * 60 + seconds;
    }
}

[Serializable]
public class Wave
{
    public Transform startPoint;
    public Transform endPoint;
    public bool isMirror;
}
