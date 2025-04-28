using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using YG;

public class GameManager : MonoBehaviour
{
    // + Добавляем подписку на событие инициализации SDK
    private void OnEnable() => YandexGame.GetDataEvent += OnSDKInitialized;
    private void OnDisable() => YandexGame.GetDataEvent -= OnSDKInitialized;

    public GameObject[] objectsToSpawn; // Префабы, которые будем спавнить
    private AudioManager audioManager;

    public bool isGameActive; //Бул, который отвечает за то работает игра или нет
    public bool isPaused = false; // Отвечает за паузу

    //UI элементы
    public GameObject countersUI;
    public GameObject titleScreenUI;
    public GameObject gameOverScreen;
    public GameObject finalScore;
    public GameObject pauseMenuUI;

    // Параметры спавна
    public float spawnRate; // Частота спавна объектов (секунды)
    public float fallSpeed; // Скорость падения объектов
    public float spawnHeight = 20f; // Высота спавна объектов

    public float spawnXMin;// Левая граница спавна
    public float spawnXMax; // Права граница спавна
    public float spawnZ = 0f; // Координата спавна по оси Z

    // Параметры счетчика
    public int countPoints = 0; // Счетчик очков
    public int countLives = 3; // Счетчик жизней

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
    private int gameRestartCount; //счетчик игр
    private const int adEachGame = 4; // Каждые сколько игр показывать рекламу

    // + Новый метод инициализации SDK
    private void Start()
    {
        if (YandexGame.SDKEnabled)
            OnSDKInitialized();
        else
            Debug.Log("Ожидание инициализации Яндекс SDK...");
        gameRestartCount = PlayerPrefs.GetInt("GameRestartCount", 0);
    }

    // + Метод обработки успешной инициализации
    void OnSDKInitialized()
    {
        // Переносим сюда основную инициализацию
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        audioManager.PlayMusic(audioManager.menuMusic);
        titleScreenUI.SetActive(true);
        pauseMenuUI.SetActive(false);
        countersUI.SetActive(false);
        gameOverScreen.SetActive(false);

        Debug.Log("Яндекс SDK инициализирован!");
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
                spawnRateStep = 0.1f;
                fallSpeedStep = 0.2f;
                moveSpeedStep = 0.3f;
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
                spawnRateStep = 0.1f;
                fallSpeedStep = 0.2f;
                moveSpeedStep = 0.5f;
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
                spawnRateStep = 0.09f;
                fallSpeedStep = 0.2f;
                moveSpeedStep = 0.6f;
                increaseEvery = 7;
                break;
        }
        StartGame();
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

    public void StartGame()
    {
        audioManager.PlayMusic(audioManager.gameMusic);
        isGameActive = true;
        countPoints = 0;
        ResumeGame();

        titleScreenUI.SetActive(false);
        countersUI.SetActive(true);

        StartCoroutine(StartSpawningWithDelay(2f));
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
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        pauseMenuUI.SetActive(false);
        audioManager.ResumeMusic();
    }

    public void GameOver()
    {
        isGameActive = false;
        countersUI.SetActive(false);
        gameOverScreen.SetActive(true);
        audioManager.PlaySFX(audioManager.gameOver);
        audioManager.StopMusic();

        if (gameRestartCount <= 3)
        {
            gameRestartCount++;
        }
        PlayerPrefs.SetInt("GameRestartCount", gameRestartCount); // Сохраняем количество рестартов

    }

    public void RestartGame()
    {
        
        if (gameRestartCount >= adEachGame)
        {
            gameRestartCount = 0;
            PlayerPrefs.SetInt("GameRestartCount", gameRestartCount); // Сохраняем количество рестартов

            if (YandexGame.SDKEnabled)
            {
                YG2.InterstitialAdvShow();
                Debug.Log("Реклама показана! Вроде..");
            } 
        }


        Debug.Log($"Счетчик рестартов равен {gameRestartCount}, adEachGame {adEachGame}");

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}