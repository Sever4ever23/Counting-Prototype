using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    private GameManager gameManager;
    public float horizontalInput;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        if (gameManager.isGameActive)
        {
            horizontalInput = Input.GetAxis("Horizontal");

            // Двигаем куб
            transform.Translate(Vector3.right * gameManager.moveSpeed * Time.deltaTime * horizontalInput);

            // Если он вышел за границы, вручную возвращаем его обратно
            if (transform.position.x > gameManager.positionLimit)
                transform.position = new Vector3(gameManager.positionLimit, transform.position.y, transform.position.z);
            else if (transform.position.x < -gameManager.positionLimit)
                transform.position = new Vector3(-gameManager.positionLimit, transform.position.y, transform.position.z);
        }
    }
}
