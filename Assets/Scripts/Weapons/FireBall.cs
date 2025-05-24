using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float lifeTime = 5f;
    public int flyingTowards = 1;
    public int damage = 1;
    public int speed = 1;
    public int knockback = 0;

    int cellSize = 8;

    int feildStartXTop = 0;
    int feildStartYTop = 0;
    int feildStartXBottom = 24;
    int feildStartYBottom = -16;

    bool isMoving = false;
    bool endMoving = false;

    Vector3 startPos;
    Vector3 endPos;
    public float duration = 3f;
    void Start()
    {
        TurnManager manager = FindFirstObjectByType<TurnManager>();
        feildStartXTop = manager.feildStartXTop;
        feildStartYTop = manager.feildStartYTop;
        feildStartXBottom = manager.feildStartXBottom;
        feildStartYBottom = manager.feildStartYBottom;
        cellSize = manager.cellSize;

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

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Проверяем попадание в объект-враг
        if (flyingTowards == 1)
        {
            Enemy enemy = collision.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, knockback); // Наносим единичный урон
                Destroy(gameObject);  // Уничтожаем пулю
            }
        }
        else
        {
            Player player = collision.collider.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage, knockback);
                Destroy(gameObject);
            }
        }
    }

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
        endPos = new Vector3(startPos.x + cellSize * flyingTowards * speed, startPos.y, startPos.z);
        // Начинаем движение
        isMoving = true;
    }
}
