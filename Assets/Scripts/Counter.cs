using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Counter : MonoBehaviour
{
    private GameManager gameManager;
    public TextMeshProUGUI CounterText; // UI для отображения счета
    public TextMeshProUGUI LivesCounter; // UI для отображения жизней
    public TextMeshProUGUI FinalScore; // UI для отображения финального результата
    public TextMeshProUGUI BestScore; // UI для отображения лучшего результата


    public void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        DecreaseLives(0);
    }

    // Метод для обновления очков
    public void UpdateScore(int points)
    {
        gameManager.countPoints += points;
        UpdateUI();
    }

    // Метод для уменьшения жизней
    public void DecreaseLives(int livesLost)
    {
        gameManager.countLives -= livesLost; // Сначала отнимаем жизни

        if (gameManager.countLives <= 0) // Если жизней 0 или меньше – сразу завершаем игру
        {
            gameManager.countLives = 0; // Убеждаемся, что в UI не будет отрицательного значения
            gameManager.GameOver();
            //return; // Выходим из метода, чтобы UI не обновлялся
        }

        UpdateUI(); // UI обновляется только если игра не закончилась
    }


    // Обновление UI
    public void UpdateUI()
    {
        FinalScore.text = "Получено очков: " + gameManager.countPoints;
        BestScore.text = "Лучший результaт:" + gameManager.bestScore;

        if (CounterText != null)
        {
            CounterText.text = "Очки: " + gameManager.countPoints;

            if (LivesCounter != null)
            {
                LivesCounter.text = "Жизни: " + gameManager.countLives;
            }
        }
    }
}
