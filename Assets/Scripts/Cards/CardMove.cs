using UnityEngine;

public class CardMove : MonoBehaviour
{
    public GameObject player; // ��������� ������ �� ������ ������� ����� ���������
    public int StepCellsRangeByY;
    public int StepCellsRangeByX;
    public float duration = 1f; // ����� �������� (1 �������)

    int cellSize = 8;
    int feildStartXTop = 0;
    int feildStartYTop = 0;
    int feildStartXBottom = 24;
    int feildStartYBottom = -16;

    private void Start()
    {
        TurnManager manager = FindFirstObjectByType<TurnManager>();
        feildStartXTop = manager.feildStartXTop;
        feildStartYTop = manager.feildStartYTop;
        feildStartXBottom = manager.feildStartXBottom / 2;
        feildStartYBottom = manager.feildStartYBottom;
        cellSize = manager.cellSize;
    }
    void OnMouseDown()
    {
        TurnManager manager = FindFirstObjectByType<TurnManager>();
        if (manager.playersTurn)
        {
            Move();
        }
        
    }
    void Move()
    {
        Vector3 startPos = player.transform.position;
        // ������������ �������� ������� (�� 2 ����� ����)
        Vector3 endPos = new Vector3(startPos.x + StepCellsRangeByX * cellSize, startPos.y + StepCellsRangeByY * cellSize, startPos.z);

        if (!(endPos.x < feildStartXTop - 1 || endPos.x > feildStartXBottom + 1 || endPos.y > feildStartYTop + 1 || endPos.y < feildStartYBottom - 1))
        {
            // �������� ��������
            Player playerSript = player.GetComponent<Player>();
            playerSript.MoveCardActivation(endPos, duration);
            //Destroy(gameObject); // ������� ������ ��� �����
        }
    }
}
