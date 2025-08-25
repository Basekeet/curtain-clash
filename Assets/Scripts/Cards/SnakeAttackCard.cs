using System.Collections;
using UnityEngine;

public class SnakeAttackCard : BaseCard
{
    private Vector2Int position;
    private Vector2Int direction;

    private bool isActivated = false;

    public override IEnumerator Activate()
    {
        if (isActivated)
            yield break;

        isActivated = true;
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
        if (gameManager.enemyPosition == position)
        {
            yield return StartCoroutine(gameManager.DamageEnemy(1));
            yield return StartCoroutine(gameManager.RemoveCardFromField(id));
            yield break;
        }

        if (gameManager.playerPosition == position)
        {
            yield return StartCoroutine(gameManager.DamagePlayer(1));
            yield return StartCoroutine(gameManager.RemoveCardFromField(id));
            yield break;
        }
        for (int i = 0; i < 1; i++)
        {
            position += direction;
            // Проверка выхода за пределы поля (пример: 3x4)
            if (position.x < 0 || position.x > 2 || position.y < 0 || position.y > 3)
            {
                yield return StartCoroutine(gameManager.RemoveCardFromField(id));
                yield break;
            }
            
            yield return StartCoroutine(gameManager.MoveCard(id, position.x, position.y));
             
            if (gameManager.enemyPosition == position)
            {
                yield return StartCoroutine(gameManager.DamageEnemy(1));
                yield return StartCoroutine(gameManager.RemoveCardFromField(id));
                yield break;
            }

            if (gameManager.playerPosition == position)
            {
                yield return StartCoroutine(gameManager.DamagePlayer(1));
                yield return StartCoroutine(gameManager.RemoveCardFromField(id));
                yield break;
            }
        }
    }
}