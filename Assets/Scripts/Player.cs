using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 3f; // Скорость перемещения персонажа

    public float snapThreshold = 0.01f;

    float cellSize = 8;
    int feildStartXTop = 0;
    int feildStartYTop = 0;
    int feildStartXBottom = 24;
    int feildStartYBottom = -16;

    public int maxHealth = 3;
    int currentHealth;
    int shieldCapasity = 0;

    bool endMoving = false;
    bool isMoving = false;
    Vector3 endPos;
    Vector3 startPos;
    float duration;
    void Start()
    {
        TurnManager manager = FindFirstObjectByType<TurnManager>();
        cellSize = manager.cellSize;
        feildStartXTop = manager.feildStartXTop;
        feildStartYTop = manager.feildStartYTop;
        feildStartXBottom = manager.feildStartXBottom / 2;
        feildStartYBottom = manager.feildStartYBottom;

        SnapToNearestCellCenter(); // Выравниваемся в центр ближайшей клетки при старте
        currentHealth = maxHealth;
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

    public void ActivateShield(int durability)
    {
        shieldCapasity += durability;
    }

    private void Move(int stepByX, int stepByY)
    {
        startPos = transform.position;
        endPos = new Vector3(transform.position.x + cellSize * stepByX, transform.position.y + cellSize * stepByY, transform.position.z);
        if (endPos.x < feildStartXTop - 7 || endPos.x > feildStartXBottom + 7 || endPos.y > feildStartYTop + 7 || endPos.y < feildStartYBottom - 7)
        {
            endPos = new Vector3(transform.position.x - cellSize * stepByX, transform.position.y - cellSize * stepByY, transform.position.z);
        }
        isMoving = true;
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

    public void TakeDamage(int amount, int knockback)
    {
        shieldCapasity -= amount;
        if (shieldCapasity < 0)
        {
            currentHealth += shieldCapasity;
            shieldCapasity = 0;
        }
        print($"{name}'s health: {currentHealth}/{maxHealth}");
        Move(-knockback, 0);

        if (currentHealth <= 0)
        {
            Die(); // Если здоровье кончилось, уничтожить врага
        }
    }
    void Die()
    {
        Destroy(gameObject); // Уничтожаем врага
    }
}
