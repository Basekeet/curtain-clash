using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public bool playersTurn = false;

    public void NextTurn()
    {
        playersTurn = !playersTurn;
        FireBall[] allExamples = FindObjectsByType<FireBall>(FindObjectsSortMode.None);
        print(allExamples.Length);
        foreach (var ex in allExamples)
        {
            ex.MoveAfterTurn();
        }
    }
}
