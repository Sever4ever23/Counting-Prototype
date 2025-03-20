using UnityEngine;

public class FloatingClouds : MonoBehaviour
{
    private float floatSpeed;   // Скорость движения
    private float floatHeight;  // Высота колебаний
    private float sideMovement; // Насколько сильно облака качаются в стороны

    private Vector3 startPos;
    private float randomOffset; // Случайный сдвиг фазы

    void Start()
    {
        startPos = transform.position;
        randomOffset = Random.Range(0f, 2f * Mathf.PI); // Разная фаза для каждого облака
        floatSpeed = Random.Range(0.7f, 1.3f);  // Случайная скорость
        floatHeight = Random.Range(1.5f, 3f);  // Случайная высота колебаний
        sideMovement = Random.Range(0.1f, 0.3f); // Лёгкое движение в стороны
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed + randomOffset) * floatHeight;
        float newX = startPos.x + Mathf.Sin(Time.time * floatSpeed * 0.5f + randomOffset) * sideMovement;

        transform.position = new Vector3(newX, newY, transform.position.z);
    }
}
