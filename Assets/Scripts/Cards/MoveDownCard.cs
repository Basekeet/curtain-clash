using System.Collections;
using UnityEngine;

public class MoveDownCard : BaseCard
{
    private bool isPlayerTurn;
    public override IEnumerator Activate()
    {
        isPlayerTurn = gameManager.isPlayerTurn;
        if (isPlayerTurn) yield return StartCoroutine(gameManager.RemoveCardFromHand(id));

        var position = isPlayerTurn ? gameManager.playerPosition : gameManager.enemyPosition;
        var direction = new Vector2Int(1, 0);
        Vector2Int newPos = position + direction;
        if (IsInsideBoard(newPos) && CanActivate())
        {
            if (isPlayerTurn)
                yield return StartCoroutine(gameManager.MovePlayer(newPos.x, newPos.y));
            else
                yield return StartCoroutine(gameManager.MoveEnemy(newPos.x, newPos.y));
        }
    }

    public override bool CanActivate()
    {
        bool isPlayerTurn = gameManager.isPlayerTurn;
        var position = isPlayerTurn ? gameManager.playerPosition : gameManager.enemyPosition;
        var direction =  new Vector2Int(1, 0);
        Vector2Int newPos = position + direction;
        return IsInsideBoard(newPos);
    }
    
    public override IEnumerator UpdateCard()
    {
        yield break;
    }
}