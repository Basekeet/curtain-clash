using UnityEngine;

public class CardDefenceMatrix : MonoBehaviour
{
    public Player player;      // �������-������������� �������
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
