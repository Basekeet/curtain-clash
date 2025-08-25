using System.Collections;
using UnityEngine;

public class BaseAttackCard : BaseCard
{
    private Vector2Int position;
    private Vector2Int direction;
    private bool IsActivated = false;

    private int updateCounter = 0;

    public override IEnumerator Activate()
    {
        if (IsActivated)
            yield break;

        IsActivated = true;
        bool isPlayerTurn = gameManager.isPlayerTurn;

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
        yield return StartCoroutine(gameManager.SoftShakeCardInField(id));
        updateCounter++;
        
        if (gameManager.enemyPosition == position)
        {
            yield return StartCoroutine(gameManager.DamageEnemy(2));
            yield return StartCoroutine(gameManager.RemoveCardFromField(id));
            yield break;
        }

        if (gameManager.playerPosition == position)
        {
            yield return StartCoroutine(gameManager.DamagePlayer(2));
            yield return StartCoroutine(gameManager.RemoveCardFromField(id));
            yield break;
        }

        // Движение происходит только раз в два хода
        if (updateCounter % 2 != 0)
            yield break;

        for (int i = 0; i < 1; i++)
        {
            position += direction;

            // Проверка выхода за пределы поля
            if (position.y < 0 || position.y > 3)
            {
                yield return StartCoroutine(gameManager.RemoveCardFromField(id));
                yield break;
            }

            yield return StartCoroutine(gameManager.MoveCard(id, position.x, position.y));

            // Урон врагу или игроку
            if (gameManager.enemyPosition == position)
            {
                yield return StartCoroutine(gameManager.DamageEnemy(2));
                yield return StartCoroutine(gameManager.RemoveCardFromField(id));
                yield break;
            }

            if (gameManager.playerPosition == position)
            {
                yield return StartCoroutine(gameManager.DamagePlayer(2));
                yield return StartCoroutine(gameManager.RemoveCardFromField(id));
                yield break;
            }
        }
    }

}