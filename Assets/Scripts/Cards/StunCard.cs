using System.Collections;
using UnityEngine;

public class StunCard : BaseCard
{
    private Vector2Int position;
    private Vector2Int direction;
    private bool isActivated = false;
    private int updatesCount = 0;
    private bool isPlayerTurn;
    public override IEnumerator Activate()
    {
        if (isActivated)
            yield break;

        isActivated = true;

        isPlayerTurn = gameManager.isPlayerTurn;
        direction = isPlayerTurn ? new Vector2Int(0, 1) : new Vector2Int(0, -1);
        Vector2Int origin = isPlayerTurn ? gameManager.playerPosition : gameManager.enemyPosition;

        position = origin + direction * 2;

        if (isPlayerTurn) yield return StartCoroutine(gameManager.RemoveCardFromHand(id));
        yield return StartCoroutine(gameManager.SpawnCardOnField(id, position.x, position.y));
    }
    public override bool CanActivate()
    {
        return true;
    }
    public override IEnumerator UpdateCard()
    {
        yield return StartCoroutine(gameManager.SoftShakeCardInField(id));
        updatesCount++;

        if (updatesCount >= 2)
        {
            if (position == gameManager.playerPosition)
            {
                gameManager.BlockPlayerTurn();
            }

            if (position == gameManager.enemyPosition)
            {
                gameManager.BlockEnemyTurn();
            }

            yield return StartCoroutine(gameManager.RemoveCardFromField(id));
            yield break;
        }

        yield return null;
    }
}