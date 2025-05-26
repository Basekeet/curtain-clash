using System.Collections;
using UnityEngine;

public class MoveRightCard : BaseCard
{
    public override IEnumerator Activate()
    {
        bool isPlayerTurn = gameManager.isPlayerTurn;
        if (isPlayerTurn) yield return StartCoroutine(gameManager.RemoveCardFromHand(id));

        var position = isPlayerTurn ? gameManager.playerPosition : gameManager.enemyPosition;
        var direction = new Vector2Int(0, 1) ;
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
        var direction = new Vector2Int(0, 1);
        Vector2Int newPos = position + direction;
        return isPlayerTurn ? newPos.y < 2 : newPos.y > 1;
    }
    
    public override IEnumerator UpdateCard()
    {
        yield break;
    }
}