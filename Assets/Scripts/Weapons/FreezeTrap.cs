using UnityEngine;

public class FreezeTrap : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    int cellSize = 8;
    void Start()
    {
        TurnManager manager = FindFirstObjectByType<TurnManager>();
        cellSize = manager.cellSize;

        SnapToNearestCellCenter();
    }
    void SnapToNearestCellCenter()
    {
        int gridX = Mathf.RoundToInt(transform.position.x / cellSize);
        int gridY = Mathf.RoundToInt(transform.position.y / cellSize);
        transform.position = new Vector3(gridX * cellSize, gridY * cellSize, transform.position.z);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy enemy = collision.collider.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.Stun(2); // Наносим единичный урон
            Destroy(gameObject);  // Уничтожаем пулю
        }
    }
}
