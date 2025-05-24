using UnityEngine;

public class CardLasso : MonoBehaviour
{
    public int turns = 0;

    void OnMouseDown()
    {
        TurnManager manager = FindFirstObjectByType<TurnManager>();
        if (manager.playersTurn)
        {
            Activate();
        }
    }
    void Activate()
    {
        TurnManager manager = FindFirstObjectByType<TurnManager>();
        manager.ActivateDelayedLasso(turns);
    }
}
