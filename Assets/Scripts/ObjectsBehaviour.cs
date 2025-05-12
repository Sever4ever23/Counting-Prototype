using UnityEngine;

public class Fruit : MonoBehaviour
{
    public GameObject goodParticleEffect;
    public GameObject badParticleEffect;
    private Counter counterScript; // Ссылка на скрипт Counter
    private AudioManager audioManager;
    private GameManager gameManager;
    private bool isHandled = false; // Флаг для проверки, был ли объект уже обработан

    private void Start()
    {
        // Инициализируем скрипт Counter с помощью поиска объекта в сцене
        counterScript = GameObject.Find("CollisionAndCountDetector").GetComponent<Counter>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();


        // Проверяем, что мы успешно нашли скрипт
        if (counterScript == null)
        {
            Debug.LogError("Не удалось найти скрипт Counter на объекте CollisionAndCountDetector");
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 randomTorque = new Vector3(
                Random.Range(-50f, 50f),
                Random.Range(-50f, 50f),
                Random.Range(-50f, 50f)
            );

            rb.AddTorque(randomTorque);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Если объект уже был обработан, игнорируем дальнейшие коллизии
        if (isHandled) return;

        // Проверка на столкновение с корзиной или землёй
        if (collision.gameObject.CompareTag("Basket") && gameManager.isGameActive)
        {
            HandleFruitInBasket(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Ground") && gameManager.isGameActive)
        {
            HandleFruitOnGround();
        } else if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Basket") && !gameManager.isGameActive)
        {
            HandleAfterGameOver();
        }
    }

    // Логика для фрукта, попавшего в корзину
    private void HandleFruitInBasket(GameObject basket)
    {
        // Если фрукт хороший, увеличиваем счёт
        if (gameObject.CompareTag("Good Fruit") && gameManager.isGameActive)
        {
            counterScript.UpdateScore(gameManager.goodPoints); // Увеличиваем счёт на n для хорошего фрукта
            // Проигрываем анимацию для хорошего фрукта
            Instantiate(goodParticleEffect, transform.position, Quaternion.identity);
            audioManager.PlaySFX(audioManager.successSFX);
        }
        // Если фрукт плохой, уменьшаем счёт на 3 и уменьшаем жизни
        else if (gameObject.CompareTag("Bad Fruit") && gameManager.isGameActive)
        {
            counterScript.UpdateScore(-gameManager.badPoints); // Уменьшаем счёт на n для плохого фрукта
            counterScript.DecreaseLives(1); // Уменьшаем жизни на 1
            // Проигрываем анимацию для плохого фрукта
            Instantiate(badParticleEffect, transform.position, Quaternion.identity);
            audioManager.PlaySFX(audioManager.failSFX);
        } 

        // После обработки ставим флаг, что фрукт обработан
        isHandled = true;
        // Удаляем фрукт
        Destroy(gameObject);
    }

    // Логика для фрукта, упавшего на землю
    private void HandleFruitOnGround()
    {
        // Если фрукт хороший, уменьшаем счёт на 1
        if (gameObject.CompareTag("Good Fruit"))
        {
            counterScript.UpdateScore(0); // Уменьшаем счёт на 0 для хорошего фрукта, который упал
            counterScript.DecreaseLives(1); // Уменьшаем жизни на 1


            // Проигрываем анимацию для хорошего фрукта
            Instantiate(badParticleEffect, transform.position, Quaternion.identity);
            audioManager.PlaySFX(audioManager.failSFX);
        }
 
        // После обработки ставим флаг, что фрукт обработан
        isHandled = true;
        // Удаляем фрукт через 1 секунду с анимацией
        Destroy(gameObject, 2f);
    }

    private void HandleAfterGameOver()
    {
        Destroy(gameObject, 1f);
    }
}
