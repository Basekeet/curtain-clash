using System.Collections;
using UnityEngine;

public class SwordAttackCard : BaseCard
{
    private Vector2Int position;
    private Vector2Int direction;
    private int turnCounter = 0;  // Счётчик ходов

    private bool isActivated = false;

    public override IEnumerator Activate()
    {
        if (isActivated)
            yield break;

        isActivated = true;
        bool isPlayerTurn = gameManager.isPlayerTurn;

        // Справа для игрока, слева — для врага
        direction = isPlayerTurn ? new Vector2Int(0, 1) : new Vector2Int(0, -1);
        Vector2Int origin = isPlayerTurn ? gameManager.playerPosition : gameManager.enemyPosition;

        position = origin + direction;

        if (isPlayerTurn) yield return StartCoroutine(gameManager.RemoveCardFromHand(id));
        yield return StartCoroutine(gameManager.SpawnCardOnField(id, position.x, position.y));
    }
    public override bool CanActivate()
    {
        return true;
    }
    public override IEnumerator UpdateCard()
    {
        if (gameManager.enemyPosition == position)
        {
            yield return StartCoroutine(gameManager.DamageEnemy(3));
            yield return StartCoroutine(gameManager.RemoveCardFromField(id));
            yield break;
        }

        if (gameManager.playerPosition == position)
        {
            yield return StartCoroutine(gameManager.DamagePlayer(3));
            yield return StartCoroutine(gameManager.RemoveCardFromField(id));
            yield break;
        }
        
        yield return StartCoroutine(gameManager.SoftShakeCardInField(id));
        turnCounter++;

        if (turnCounter % 3 == 0)
        {
            position += direction;

            // Проверка выхода за границы поля (предположим, 3x4)
            if (position.x < 0 || position.x > 2 || position.y < 0 || position.y > 3)
            {
                yield return StartCoroutine(gameManager.RemoveCardFromField(id));
                yield break;
            }
            yield return StartCoroutine(gameManager.MoveCard(id, position.x, position.y));

            // Проверка на попадание
            if (gameManager.enemyPosition == position)
            {
                yield return StartCoroutine(gameManager.DamageEnemy(3));
                yield return StartCoroutine(gameManager.RemoveCardFromField(id));
                yield break;
            }

            if (gameManager.playerPosition == position)
            {
                yield return StartCoroutine(gameManager.DamagePlayer(3));
                yield return StartCoroutine(gameManager.RemoveCardFromField(id));
                yield break;
            }
        }
        else
        {
            // Не двигаемся в этом ходу, просто ждем следующего вызова UpdateCard
            yield break;
        }
    }
}
