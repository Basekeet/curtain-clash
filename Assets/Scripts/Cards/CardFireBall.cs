using UnityEngine;

public class CardFireBall : MonoBehaviour
{
    public GameObject bulletPrefab;      // �������-������������� �������
    public Transform firePoint;          // �����, ������ ��������
    public int cellSize = 8;
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
