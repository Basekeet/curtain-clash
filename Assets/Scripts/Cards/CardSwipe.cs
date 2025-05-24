using UnityEngine;

public class CardSwipe : MonoBehaviour
{
    int cellSize = 8;
    public Transform firePoint;          // �����, ������ ��������
    public int widthCells = 1;     // ������ �������
    public int heightCells = 3;     // ������ �������

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
        // ����� ������ ���� ��������������
        Vector3 bottomLeftCorner = firePoint.position - new Vector3(widthCells * cellSize, heightCells * cellSize, 0);

        // ������� ������ ���� ��������������
        Vector3 topRightCorner = firePoint.position + new Vector3(widthCells * cellSize, heightCells * cellSize, 0);
        //Destroy(gameObject); // ������� ������ ��� �����
        Collider2D[] hitColliders = Physics2D.OverlapAreaAll(bottomLeftCorner, topRightCorner);

        foreach (Collider2D collider in hitColliders)
        {
            // ���������, ������������ �� ��������� Enemy
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, knockback); // ������� ����
            }
        }
    }
}
