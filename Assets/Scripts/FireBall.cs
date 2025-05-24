using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float lifeTime = 5f;
    public int cellSize = 8;

    public int feildStartXTop = 0;
    public int feildStartYTop = 0;
    public int feildStartXBottom = 24;
    public int feildStartYBottom = -16;

    bool isMoving = false;
    bool endMoving = false;

    Vector3 startPos;
    Vector3 endPos;
    public float duration = 1f;
    void Start()
    {
        SnapToNearestCellCenter();
    }
    private void Update()
    {
        CheckOnField();

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
            endMoving = false;
        }
    }
    //void OnCollisionEnter2D(Collision2D collision)
    //{
    //    // Любые дополнительные эффекты при попадании снаряда
    //    Debug.Log("Попадание в цель!");

    //    // Удаляем снаряд после попадания
    //    Destroy(gameObject);
    //}

    void SnapToNearestCellCenter()
    {
        int gridX = Mathf.RoundToInt(transform.position.x / cellSize);
        int gridY = Mathf.RoundToInt(transform.position.y / cellSize);
        transform.position = new Vector3(gridX * cellSize, gridY * cellSize, transform.position.z);
    }
    void CheckOnField()
    {
        if (transform.position.x < feildStartXTop - 1 || transform.position.x > feildStartXBottom + 1 || transform.position.y > feildStartYTop + 1 || transform.position.y < feildStartYBottom - 1)
        {
            Destroy(gameObject);
        }
    }

    public void MoveAfterTurn()
    {
        startPos = transform.position;
        // Рассчитываем конечную позицию (на 2 блока ниже)
        endPos = new Vector3(startPos.x + cellSize, startPos.y, startPos.z);
        // Начинаем движение
        isMoving = true;
    }
}
