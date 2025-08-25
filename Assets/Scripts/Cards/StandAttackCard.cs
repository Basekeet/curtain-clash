using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandAttackCard : BaseCard
{
    private List<Vector2Int> spawnedPositions = new List<Vector2Int>();
    private Vector2Int direction;
    private bool isActivated = false;

    public override IEnumerator Activate()
    {
        if (isActivated)
            yield break;

        isActivated = true;

        bool isPlayerTurn = gameManager.isPlayerTurn;
        Vector2Int origin = isPlayerTurn ? gameManager.playerPosition : gameManager.enemyPosition;

        direction = isPlayerTurn ? new Vector2Int(0, 1) : new Vector2Int(0, -1);

        if (isPlayerTurn) yield return StartCoroutine(gameManager.RemoveCardFromHand(id));

        for (int i = -1; i <= 1; i++)
        {
            Vector2Int spawnPos = origin + direction;
            spawnPos += new Vector2Int(i, 0);

            if (IsInsideBoard(spawnPos))
            {
                // Спавн стенда (можно использовать другой id для стенда)
                yield return StartCoroutine(gameManager.SpawnCardOnField(id, spawnPos.x, spawnPos.y));
                spawnedPositions.Add(spawnPos);
            }
        }

       
    }
    public override bool CanActivate()
    {
        return true;
    }
    public override IEnumerator UpdateCard()
    {
        yield return StartCoroutine(gameManager.SoftShakeCardInField(id));
        foreach (var pos in spawnedPositions)
        {
            if (gameManager.playerPosition == pos)
            {
                yield return StartCoroutine(gameManager.DamagePlayer(1));
            }
            else if (gameManager.enemyPosition == pos)
            {
                yield return StartCoroutine(gameManager.DamageEnemy(1));
            }
        }

        yield return gameManager.RemoveCardFromField(id);

        spawnedPositions.Clear();
    }

}