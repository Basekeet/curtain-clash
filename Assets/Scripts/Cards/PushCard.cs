using System.Collections;
using UnityEngine;

public class PushCard : BaseCard
{
    private Vector2Int position;
    private Vector2Int direction;
    private bool isActivated = false;
    private int turnCounter = 0; // Счётчик ходов

    public override IEnumerator Activate()
    {
        if (isActivated)
            yield break;

        isActivated = true;

        bool isPlayerTurn = gameManager.isPlayerTurn;
        direction = isPlayerTurn ? new Vector2Int(0, 1) : new Vector2Int(0, -1);
        Vector2Int origin = isPlayerTurn ? gameManager.playerPosition : gameManager.enemyPosition;

        position = origin + direction;

        if (isPlayerTurn)
            yield return StartCoroutine(gameManager.RemoveCardFromHand(id));
        yield return StartCoroutine(gameManager.SpawnCardOnField(id, position.x, position.y));
    }

    public override bool CanActivate()
    {
        return true;
    }
    
    public override IEnumerator UpdateCard()
    {
        yield return StartCoroutine(gameManager.SoftShakeCardInField(id));
        turnCounter++;
        if (turnCounter % 2 != 0)
            yield break; // Действуем только на чётные ходы

        Vector2Int nextPosition = position + direction;

        if (!IsInsideBoard(nextPosition))
        {
            yield return StartCoroutine(gameManager.RemoveCardFromField(id));
            yield break;
        }

        if (gameManager.playerPosition == nextPosition)
        {
            Vector2Int pushTarget = gameManager.playerPosition + direction;
            if (IsInsideBoard(pushTarget))
            {
                yield return StartCoroutine(gameManager.MovePlayer(pushTarget.x, pushTarget.y));
            }
        }
        else if (gameManager.enemyPosition == nextPosition)
        {
            Vector2Int pushTarget = gameManager.enemyPosition + direction;
            if (IsInsideBoard(pushTarget))
            {
                yield return StartCoroutine(gameManager.MoveEnemy(pushTarget.x, pushTarget.y));
            }
        }

        position = nextPosition;
        yield return StartCoroutine(gameManager.MoveCard(id, position.x, position.y));
        if (position.y == 3 || position.y == 0)
        {
            yield return StartCoroutine(gameManager.RemoveCardFromField(id));
            yield break;
        }
    }
}
