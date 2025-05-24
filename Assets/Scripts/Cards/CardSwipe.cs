using UnityEngine;

public class CardSwipe : MonoBehaviour
{
    int cellSize = 8;
    public Transform firePoint;          // Точка, откуда стреляют
    public int widthCells = 1;     // Ширина области
    public int heightCells = 3;     // Высота области

    public int damage = 3;
    public int knockback = 0;
    void Start()
    {
        TurnManager manager = FindFirstObjectByType<TurnManager>();
        cellSize = manager.cellSize;
    }

    void OnMouseDown()
    {
        TurnManager manager = FindFirstObjectByType<TurnManager>();
        if (manager.playersTurn)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Левый нижний угол прямоугольника
        Vector3 bottomLeftCorner = firePoint.position - new Vector3(widthCells * cellSize, heightCells * cellSize, 0);

        // Верхний правый угол прямоугольника
        Vector3 topRightCorner = firePoint.position + new Vector3(widthCells * cellSize, heightCells * cellSize, 0);
        //Destroy(gameObject); // Удаляем объект при клике
        Collider2D[] hitColliders = Physics2D.OverlapAreaAll(bottomLeftCorner, topRightCorner);

        foreach (Collider2D collider in hitColliders)
        {
            // Проверяем, присутствует ли компонент Enemy
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, knockback); // Наносим урон
            }
        }
    }
}
