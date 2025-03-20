using UnityEngine;
using UnityEngine.UI;

public class DifficultyButton : MonoBehaviour
{
    public int difficultyValue; // Уровень сложности (1, 2 или 3)
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>(); // Найти GameManager в сцене
        GetComponent<Button>().onClick.AddListener(() => gameManager.SetDifficulty(difficultyValue));
    }
}
