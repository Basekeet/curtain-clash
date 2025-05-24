using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnManager : MonoBehaviour
{
    public bool playersTurn = false;

    public int cellSize = 8;
    public int feildStartXTop = 0;
    public int feildStartYTop = 0;
    public int feildStartXBottom = 24;
    public int feildStartYBottom = -16;

    List<int> lassos = new List<int>();

    private void Update()
    {
        CheckVictoryCondition();
        CheckLoseCondition();
    }

    void CheckLoseCondition()
    {
        Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);
        if (players.Length == 0)
        {
            SceneManager.LoadScene("Defeat"); // Название сцены победы
        }
    }
    void CheckVictoryCondition()
    {
        // Проверяем наличие врагов на сцене
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        // Если врагов больше нет, победа
        if (enemies.Length == 0)
        {
            SceneManager.LoadScene("Victory"); // Название сцены победы
        }
    }


    public void NextTurn()
    {
        playersTurn = !playersTurn;
        if (playersTurn)
        {
            print("Your Turn");
        }
        else
        {
            print("Enemy Turn");
        }

        MoveAllFireballs();
        
        if (!playersTurn)
        {
            EnemyTurn();
        }

        ActivateLassos();

        
    }

    void MoveAllFireballs()
    {
        FireBall[] allExamples = FindObjectsByType<FireBall>(FindObjectsSortMode.None);
        foreach (var ex in allExamples)
        {
            ex.MoveAfterTurn();
        }
    }

    void EnemyTurn()
    {
        Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (var enemy in allEnemies)
        {
            enemy.Stun(-1);
            enemy.EnemyMakeDesision();
        }
    }
    
    void ActivateLassos()
    {
        for (int i = 0; i < lassos.Count; i++)
        {
            lassos[i]--;
            if (lassos[i] == 0)
            {
                Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
                foreach (var enemy in allEnemies)
                {
                    enemy.Pull();
                }
            }
            lassos.RemoveAll(x => x == 0);
        }
    }

    public void ActivateDelayedLasso(int turns)
    {
        lassos.Add(turns);
    }
}
