using System.Collections;
using UnityEngine;

public class LassoTrapCard : BaseCard
{
    private int turnsWaited = 0;
    private Vector2Int[] trapPositions;

    private bool isActivated = false;
    private bool isPlayerTurn = false;
    public override IEnumerator Activate()
    {
        isPlayerTurn = gameManager.isPlayerTurn;
        if (isActivated)
            yield break;

        isActivated = true;


        var lastY = isPlayerTurn ? 3 : 0;
        // Все клетки последнего столбца (x = 2) на поле 3x4 (по y от 0 до 3)
        trapPositions = new Vector2Int[3];
        for (int x = 0; x < 3; x++)
        {
            trapPositions[x] = new Vector2Int(x, lastY);
            yield return StartCoroutine(gameManager.SpawnCardOnField(id, x, lastY));
        }

        if (isPlayerTurn) yield return StartCoroutine(gameManager.RemoveCardFromHand(id));
    }

    public override bool CanActivate()
    {
        return true;
    }
    
    public override IEnumerator UpdateCard()
    {
        yield return StartCoroutine(gameManager.SoftShakeCardInField(id));
        turnsWaited++;

        if (turnsWaited >= 4)
        {
            var beforeLastY = isPlayerTurn ? 2 : 1;
            // Если враг на последнем столбце — вытолкнуть его на предпоследний
            if (isPlayerTurn && gameManager.enemyPosition.y == 3)
            {
                Vector2Int targetPos = new Vector2Int(gameManager.enemyPosition.x, beforeLastY);
                yield return StartCoroutine(gameManager.MoveEnemy(targetPos.x, targetPos.y));
            }
            if (!isPlayerTurn && gameManager.playerPosition.y == 0)
            {
                Vector2Int targetPos = new Vector2Int(gameManager.playerPosition.x, beforeLastY);
                yield return StartCoroutine(gameManager.MovePlayer(targetPos.x, targetPos.y));
            } 

            yield return StartCoroutine(gameManager.RemoveCardFromField(id));
            yield break;
        }

        yield return null;
    }
}