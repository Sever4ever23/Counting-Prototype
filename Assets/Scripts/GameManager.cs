using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using YG;
using PlayerPrefs = RedefineYG.PlayerPrefs;

public class GameManager : MonoBehaviour
{
    public GameObject[] objectsToSpawn; // Префабы, которые будем спавнить
    private AudioManager audioManager;
    private Counter counterScript;
    private PanelFader panelFaderScript;

    public bool isGameActive; //Бул, который отвечает за то работает игра или нет
    public bool isPaused = false; // Отвечает за паузу
    public bool isAdditionalLifeUsed = false;

    //UI элементы
    public GameObject countersUI;
    public GameObject titleScreenUI;
    public GameObject gameOverScreen;
    public GameObject finalScore;
    public GameObject pauseMenuUI;
    public GameObject blur;
    public GameObject continueWithBonusLifeButton;

    [SerializeField]
    bool isSDKEnabled = YandexGame.SDKEnabled;

    // Параметры спавна
    public float spawnRate; // Частота спавна объектов (секунды)
    public float fallSpeed; // Скорость падения объектов
    public float spawnHeight = 20f; // Высота спавна объектов

    public float spawnXMin;// Левая граница спавна
    public float spawnXMax; // Права граница спавна
    public float spawnZ = 0f; // Координата спавна по оси Z

    // Параметры счетчика
    public int countPoints = 0; // Счетчик очков
    public int countLives; // Счетчик жизней
    public int goodPoints;
    public int badPoints;
    public int bestScore;

    // Параметры движения игрока
    public float moveSpeed;
    public float positionLimit; // Ограничение по X

    // Увеличение сложности, контроль шагов
    private int spawnedObjectCount = 0; // Сколько объектов заспавнено
    public int increaseEvery; // Каждые сколько объектов повышать сложность
    public float minSpawnRateLimit;

    // Насколько увеличивать параметры
    public float spawnRateStep;
    public float fallSpeedStep;
    public float moveSpeedStep;

    //Параметры рекламы
    private int gamesPlayedCount; //счетчик игр
    private const int adsEvryGame = 2; // Каждые сколько игр показывать рекламу
    public string rewardID = "ExtraLife"; // ID награды

    private void Start()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        counterScript = GameObject.Find("CollisionAndCountDetector").GetComponent<Counter>();
        panelFaderScript = blur.GetComponent<PanelFader>();

        audioManager.PlayMusic(audioManager.menuMusic);
        titleScreenUI.SetActive(true);
        pauseMenuUI.SetActive(false);
        countersUI.SetActive(false);
        gameOverScreen.SetActive(false);
        gamesPlayedCount = PlayerPrefs.GetInt("GamesPlayedCount", 0);
        Debug.Log("Start - succes");
        ShowInternalAd();
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
    }

    // Метод установки сложности
    public void SetDifficulty(int difficulty)
    {
        switch (difficulty)
        {
            case 1: // Легкая сложность
                spawnRate = 2.3f;
                fallSpeed = 4.5f;
                countLives = 4;
                spawnXMin = -11f;
                spawnXMax = 11f;
                moveSpeed = 18f;
                positionLimit = 14f;
                minSpawnRateLimit = 0.9f;
                goodPoints = 1;
                badPoints = 3;
                spawnRateStep = 0.1f;
                fallSpeedStep = 0.2f;
                moveSpeedStep = 0.4f;
                increaseEvery = 9;
                break;
            case 2: // Средняя сложность
                spawnRate = 1.9f;
                fallSpeed = 5.5f;
                countLives = 3;
                spawnXMin = -14f;
                spawnXMax = 14f;
                moveSpeed = 20f;
                positionLimit = 15.5f;
                minSpawnRateLimit = 0.7f;
                goodPoints = 2;
                badPoints = 6;
                spawnRateStep = 0.1f;
                fallSpeedStep = 0.2f;
                moveSpeedStep = 0.6f;
                increaseEvery = 8;
                break;
            case 3: // Тяжелая сложность
                spawnRate = 1.4f;
                fallSpeed = 6.5f;
                countLives = 2;
                spawnXMin = -18f;
                spawnXMax = 18f;
                moveSpeed = 22f;
                positionLimit = 17f;
                minSpawnRateLimit = 0.5f;
                goodPoints = 3;
                badPoints = 9;
                spawnRateStep = 0.09f;
                fallSpeedStep = 0.2f;
                moveSpeedStep = 0.7f;
                increaseEvery = 7;
                break;
        }
        StartGame();
        Debug.Log("SetDifficulty - succes");
    }

    public void IncreaseDifficultySimple()
    {
        spawnRate = Mathf.Max(minSpawnRateLimit, spawnRate - spawnRateStep);
        fallSpeed += fallSpeedStep;
        moveSpeed += moveSpeedStep;
        Debug.Log($"Сложность увеличена! spawnRate: {spawnRate:F2}, fallSpeed: {fallSpeed:F2}, moveSpeed: {moveSpeed:F2}");
    }

    private void Update()
    {
        if (isGameActive && Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }

        if (gameOverScreen.activeSelf && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)))
        {
            RestartGame();
        }
    }

    private void StartGame()
    {
        counterScript.UpdateUI();
        audioManager.PlayMusic(audioManager.gameMusic);
        isGameActive = true;
        countPoints = 0;

        titleScreenUI.SetActive(false);
        countersUI.SetActive(true);

        StartCoroutine(StartSpawningWithDelay(2f));
        Debug.Log("StartGame - succes");
    }

    public void ShowInternalAd()
    {
        if (gamesPlayedCount >= adsEvryGame)
        {
            gamesPlayedCount = 0;
            PlayerPrefs.SetInt("GamesPlayedCount", gamesPlayedCount);

            if (YandexGame.SDKEnabled)
            {
                YG2.InterstitialAdvShow();
                Debug.Log("Попытка показа рекламы...");
            }
        }
    }

    public void ShowRewardedAd()
    {
        Debug.Log("ShowRewardedAd - start");

        YG2.RewardedAdvShow(rewardID, () =>
        {
            RewardForAd();
            ContinueGame();

        });
        Debug.Log("ShowRewardedAd - succes");
    }

    public void RewardForAd()
    {
        // Получаем награду за просмотр
        countLives++;
        counterScript.UpdateUI();

        Debug.Log("Награда получена! Жизней: " + countLives);
    }



    IEnumerator StartSpawningWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(SpawnObjects());
    }

    IEnumerator SpawnObjects()
    {
        while (isGameActive == true)
        {
            SpawnObject();
            yield return new WaitForSeconds(spawnRate);
        }
    }

    void SpawnObject()
    {
        if (objectsToSpawn.Length == 0) return;

        Vector3 spawnPosition = new Vector3(Random.Range(spawnXMin, spawnXMax), spawnHeight, spawnZ);
        GameObject newObject = Instantiate(objectsToSpawn[Random.Range(0, objectsToSpawn.Length)], spawnPosition,
            Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));

        Rigidbody rb = newObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.down * fallSpeed;
        }

        spawnedObjectCount++;

        if (spawnedObjectCount % increaseEvery == 0)
        {
            IncreaseDifficultySimple();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
        pauseMenuUI.SetActive(true);
        audioManager.PauseMusic();
        Debug.Log("Pause  - sucess");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        pauseMenuUI.SetActive(false);
        audioManager.ResumeMusic();
        Debug.Log("Resume  - sucess");
    }

    public void GameOver()
    {
        isGameActive = false;
        countersUI.SetActive(false);
        gameOverScreen.SetActive(true);
        if (!isAdditionalLifeUsed)
        {
            continueWithBonusLifeButton.SetActive(true);
            isAdditionalLifeUsed = true;
        } else
        {
            continueWithBonusLifeButton.SetActive(false);
        }
        audioManager.PlaySFX(audioManager.gameOver);
        audioManager.PauseMusic();
        Debug.Log("GameOver - succes");
        panelFaderScript.FadeIn();

        if (countPoints > bestScore)
        {
            bestScore = countPoints;
            PlayerPrefs.SetInt("BestScore", bestScore);
            PlayerPrefs.Save();
            counterScript.UpdateUI();

        }
    }
    public void ContinueGame()
    {
        isGameActive = true;
        audioManager.ResumeMusic();
        countersUI.SetActive(true);
        gameOverScreen.SetActive(false);
        panelFaderScript.FadeOut();
        audioManager.StopSFX();
       

        // Перезапускаем спавн объектов
        StartCoroutine(StartSpawningWithDelay(1.5f));
        Debug.Log("ContinueGame - succes");
    }

    public void RestartGame()
    {
        if (gamesPlayedCount <= adsEvryGame)
        {
            gamesPlayedCount++;
        }

        PlayerPrefs.SetInt("GamesPlayedCount", gamesPlayedCount);

        Debug.Log($"Счетчик игр равен {gamesPlayedCount}, реклама каждые {adsEvryGame} игры");

        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("RestartGame - succes");
    }
  
}