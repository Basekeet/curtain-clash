using UnityEngine;

public class CardFireBall : MonoBehaviour
{
    public GameObject bulletPrefab;      // Предмет-представитель снаряда
    public Transform firePoint;          // Точка, откуда стреляют
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
        // Создаем снаряд на точке выстрела 
        Vector3 bulletPosition = new Vector3(firePoint.position.x + cellSize, firePoint.position.y, firePoint.position.z);
        GameObject bulletInstance = Instantiate(bulletPrefab, bulletPosition, firePoint.rotation);
        //Destroy(gameObject); // Удаляем объект при клике
    }
}
