using UnityEngine;

public class CardMove : MonoBehaviour
{
    public GameObject player; // ��������� ������ �� ������ ������� ����� ���������
    public int StepCellsRangeByY;
    public int StepCellsRangeByX;
    public int cardSize = 8;
    public float duration = 1f; // ����� �������� (1 �������)

    public int feildStartXTop = 0;
    public int feildStartYTop = 0;
    public int feildStartXBottom = 24;
    public int feildStartYBottom = -16;
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
        Vector3 endPos = new Vector3(startPos.x + StepCellsRangeByX * cardSize, startPos.y + StepCellsRangeByY * cardSize, startPos.z);

        if (!(endPos.x < feildStartXTop - 1 || endPos.x > feildStartXBottom + 1 || endPos.y > feildStartYTop + 1 || endPos.y < feildStartYBottom - 1))
        {
            // �������� ��������
            Player playerSript = player.GetComponent<Player>();
            playerSript.MoveCardActivation(endPos, duration);
            //Destroy(gameObject); // ������� ������ ��� �����
        }
    }
}
