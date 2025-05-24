using UnityEngine;

public class CardDefenceMatrix : MonoBehaviour
{
    public Player player;      // Предмет-представитель снаряда
    public int durability = 2;
    void OnMouseDown()
    {
        TurnManager manager = FindFirstObjectByType<TurnManager>();
        if (manager.playersTurn)
        {
            player.ActivateShield(durability);
        }
    }
}
