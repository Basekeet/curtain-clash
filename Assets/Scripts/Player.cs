using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f; // Скорость перемещения персонажа

    public float cellSize = 8;
    public float snapThreshold = 0.01f;
    bool endMoving = false;

    bool isMoving = false;
    Vector3 endPos;
    Vector3 startPos;
    float duration;
    void Start()
    {
        SnapToNearestCellCenter(); // Выравниваемся в центр ближайшей клетки при старте
    }
    void Update()
    {

        if (isMoving)
        {
            // Лерпом вычисляем промежуточную позицию
            float t = Mathf.SmoothStep(0, 1, Time.timeSinceLevelLoad % duration / duration);
            transform.position = Vector3.Lerp(startPos, endPos, t);

            // Проверяем завершение анимации
            if (Vector3.Distance(transform.position, endPos) <= 0.01f)
            {
                isMoving = false;
                endMoving = true;
            }
        }
        if (endMoving)
        {
            SnapToNearestCellCenter();
            endMoving= false;
        }
    }

    public void MoveCardActivation(Vector3 givenEndPos, float givenDuration)
    {
        duration = givenDuration;
        endPos = givenEndPos;
        startPos = transform.position;
        isMoving = true;
    }
    void SnapToNearestCellCenter()
    {
        int gridX = Mathf.RoundToInt(transform.position.x / cellSize);
        int gridY = Mathf.RoundToInt(transform.position.y / cellSize);
        transform.position = new Vector3(gridX * cellSize, gridY * cellSize, transform.position.z);
    }
}
