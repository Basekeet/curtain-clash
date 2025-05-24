using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Enemy : MonoBehaviour
{
    public float speed = 3f; // Скорость перемещения персонажа

    public int maxHealth = 3;
    int currentHealth;
    int stunedForTurns = 0;

    float cellSize = 8;
    int feildStartXTop = 0;
    int feildStartYTop = 0;
    int feildStartXBottom = 24;
    int feildStartYBottom = -16;

    public GameObject bulletPrefab;

    bool endMoving = false;
    bool isMoving = false;
    Vector3 endPos;
    Vector3 startPos;
    public float duration = 3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        TurnManager manager = FindFirstObjectByType<TurnManager>();
        cellSize = manager.cellSize;
        feildStartXTop = (manager.feildStartXBottom - manager.feildStartXTop) / 2;
        feildStartYTop = manager.feildStartYTop;
        feildStartXBottom = manager.feildStartXBottom;
        feildStartYBottom = manager.feildStartYBottom;
        SnapToNearestCellCenter(); // Выравниваемся в центр ближайшей клетки при старте
    }

    // Update is called once per frame
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
            endMoving = false;
        }
    }

    void SnapToNearestCellCenter()
    {
        int gridX = Mathf.RoundToInt(transform.position.x / cellSize);
        int gridY = Mathf.RoundToInt(transform.position.y / cellSize);
        transform.position = new Vector3(gridX * cellSize, gridY * cellSize, transform.position.z);
    }

    public void TakeDamage(int amount, int knockback)
    {
        currentHealth -= amount;
        print($"{name}'s health: {currentHealth}/{maxHealth}");
        Move(knockback, 0);

        if (currentHealth <= 0)
        {
            Die(); // Если здоровье кончилось, уничтожить врага
        }
    }
    void Die()
    {
        Destroy(gameObject); // Уничтожаем врага
    }

    public void Stun(int turns)
    {
        stunedForTurns += turns;
        if(stunedForTurns < 0)
        {
            stunedForTurns = 0;
        }
    }

    public void Pull()
    {
        startPos = transform.position;
        endPos = new Vector3(feildStartXTop, transform.position.y, transform.position.z);
        isMoving = true;
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

    void Cast()
    {
        Vector3 bulletPosition = new Vector3(transform.position.x - cellSize, transform.position.y, transform.position.z);
        GameObject bulletInstance = Instantiate(bulletPrefab, bulletPosition, Quaternion.Euler(new Vector3(0f, 180f, 0f)));
        //bulletInstance.transform.rotation = Quaternion.Euler(180f, 0f, 0f);
        FireBall fireBall = bulletInstance.GetComponent<FireBall>();

        if (fireBall != null)
        {
            fireBall.flyingTowards = -1;
        }
    }

    public void EnemyMakeDesision()
    {
        if (stunedForTurns == 0)
        {
            // Генерируем случайное число от 1 до 3
            int randomAction = Random.Range(1, 4); // диапазон от 1 до 3 включительно
            int randomDirection = Random.value < 0.5f ? -1 : 1;

            switch (randomAction)
            {
                case 1:
                     Move(1* randomDirection, 0);
                    break;
                case 2:
                    Move(0, 1 * randomDirection);
                    break;
                case 3:
                    Cast();
                    break;
                default:
                    Debug.LogError("Некорректное случайное число");
                    break;
            }
        }
    }
}
