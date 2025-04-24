using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
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
                //Настройки прогрессии сложности
                minSpawnRateLimit = 0.9f;
                spawnRateStep = 0.1f;
                fallSpeedStep = 0.2f;
                moveSpeedStep = 0.3f;
                increaseEvery = 8; // Каждые сколько объектов повышать сложность
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
                //Настройки прогрессии сложности 
                spawnRateStep = 0.1f;
                fallSpeedStep = 0.2f;
                moveSpeedStep = 0.5f;
                increaseEvery = 7; // Каждые сколько объектов повышать сложность
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
                //Настройки прогрессии сложности 
                spawnRateStep = 0.09f;
                fallSpeedStep = 0.2f;
                moveSpeedStep = 0.6f;
                increaseEvery = 6; // Каждые сколько объектов повышать сложность
                break;
        }
        StartGame();
    }

    public void IncreaseDifficultySimple()
    {
        spawnRate = Mathf.Max(minSpawnRateLimit, spawnRate - spawnRateStep); // уменьшаем интервал, но не ниже 0.5
        fallSpeed += fallSpeedStep;
        moveSpeed += moveSpeedStep;

        Debug.Log($"Сложность увеличена! spawnRate: {spawnRate:F2}, fallSpeed: {fallSpeed:F2}, moveSpeed: {moveSpeed:F2}");
    }


    // Запуск игры (игрок может двигаться сразу)
    private void Start ()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        audioManager.PlayMusic(audioManager.menuMusic);
        titleScreenUI.SetActive(true);
        pauseMenuUI.SetActive(false);
        countersUI.SetActive(false);
        gameOverScreen.SetActive(false);

    }

    private void Update()
    {
        if (isGameActive && Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
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

        titleScreenUI.SetActive(false); // Убираем меню выбора сложности
        countersUI.SetActive(true); // Показываем UI счетчиков

        StartCoroutine(StartSpawningWithDelay(2f)); // Запускаем спавн через 2 секунды
    }

    // Корутину для задержки спавна
    IEnumerator StartSpawningWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Ждём перед началом спавна
        StartCoroutine(SpawnObjects()); // Запускаем спавн объектов
    }


    IEnumerator SpawnObjects() // Собсно херня, которая спавгит объекты
    {
        while (isGameActive == true)
        {
            SpawnObject(); // Вызываем спавн
            yield return new WaitForSeconds(spawnRate); // Ждём перед следующим спавном
        }
    }

    // Метод для спавна одного объекта
    void SpawnObject()
    {
        if (objectsToSpawn.Length == 0) return; // Если массив пуст, ничего не спавним

        // Случайная позиция спауна
        Vector3 spawnPosition = new Vector3(Random.Range(spawnXMin, spawnXMax), spawnHeight, spawnZ);

        // Создаём объект
        GameObject newObject = Instantiate(objectsToSpawn[Random.Range(0, objectsToSpawn.Length)], spawnPosition, Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));

        // Устанавливаем скорость падения, если у объекта есть Rigidbody
        Rigidbody rb = newObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.down * fallSpeed; // Падение
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
        pauseMenuUI.SetActive(true); // покажем меню паузы
        audioManager.PauseMusic();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        pauseMenuUI.SetActive(false); // спрячем меню
        audioManager.ResumeMusic();
    }

    public void GameOver()
    {
        isGameActive = false;
        countersUI.SetActive(false);
        gameOverScreen.SetActive(true);

        audioManager.PlaySFX(audioManager.gameOver);
        audioManager.StopMusic();
        
    }


    public void RestartGame()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
