using System.Collections;
using UnityEngine;

public abstract class BaseCard : MonoBehaviour
{
    protected bool IsInsideBoard(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x <= 2 && pos.y >= 0 && pos.y <= 3;
    }
    protected GameManager gameManager;
    public int id;

    public void Initialize(GameManager manager, int id)
    {
        gameManager = manager;
        this.id = id;
    }

    public abstract bool CanActivate();
    public abstract IEnumerator Activate();
    public abstract IEnumerator UpdateCard();
}