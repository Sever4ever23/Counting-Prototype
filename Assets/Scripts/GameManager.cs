using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject[] objectsToSpawn; // Префабы, которые будем спавнить
    private AudioManager audioManager;

    public bool isGameActive; //Бул, который отвечает за то работает игра или нет

    //UI элементы
    public GameObject countersUI;
    public GameObject titleScreenUI;
    public GameObject gameOverScreen;
    public GameObject finalScore;


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

    // Метод установки сложности
    public void SetDifficulty(int difficulty)
    {
        switch (difficulty)
        {
            case 1: // Легкая сложность
                spawnRate = 2.3f;
                fallSpeed = 4f;
                countLives = 4;
                spawnXMin = -10f;
                spawnXMax = 10f;
                moveSpeed = 11f;
                positionLimit = 14f;
                break;
            case 2: // Средняя сложность
                spawnRate = 1.9f;
                fallSpeed = 5.5f;
                countLives = 3;
                spawnXMin = -14f;
                spawnXMax = 14f;
                moveSpeed = 16f;
                positionLimit = 15.5f;
                break;
            case 3: // Тяжелая сложность
                spawnRate = 1.4f;
                fallSpeed = 6.5f;
                countLives = 2;
                spawnXMin = -18f;
                spawnXMax = 18f;
                moveSpeed = 20f;
                positionLimit = 17f;
                break;
        }
        StartGame();
    }

    // Запуск игры (игрок может двигаться сразу)
    private void Start ()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        audioManager.PlayMusic(audioManager.menuMusic);

    }

    public void StartGame()
    {
        audioManager.PlayMusic(audioManager.gameMusic);
        isGameActive = true;
        countPoints = 0;

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
