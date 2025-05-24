using UnityEngine;

public class CardFreezeTrap : MonoBehaviour
{
    public GameObject bulletPrefab;      // �������-������������� �������
    public Transform firePoint;          // �����, ������ ��������
    int cellSize = 8;
    private void Start()
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
        // ������� ������ �� ����� �������� 
        Vector3 bulletPosition = new Vector3(firePoint.position.x + cellSize, firePoint.position.y, firePoint.position.z);
        GameObject bulletInstance = Instantiate(bulletPrefab, bulletPosition, firePoint.rotation);
        //Destroy(gameObject); // ������� ������ ��� �����
    }
}
