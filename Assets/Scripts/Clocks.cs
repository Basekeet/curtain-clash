using UnityEngine;

public class Clocks : MonoBehaviour
{

    // Update is called once per frame
    void OnMouseDown()
    {
        TurnManager manager = FindFirstObjectByType<TurnManager>();
        manager.NextTurn();
    }
}
