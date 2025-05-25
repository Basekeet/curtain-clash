using UnityEngine;
using System.Text.RegularExpressions;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class ObjectSlotsHandling : MonoBehaviour
{
    
    private const int Width = 3;
    private const int Height = 4;

    public GameObject[,] slots = new GameObject[Width, Height];

    private static readonly Regex slotNameRegex = new Regex(@"Slot\((\d+),\s*(\d+)\)");

    void Awake()
    {
        InitSlots();
    }

    public void InitSlots()
    {
        foreach (Transform child in transform)
        {
            Match match = slotNameRegex.Match(child.name);
            if (match.Success)
            {
                int x = int.Parse(match.Groups[1].Value);
                int y = int.Parse(match.Groups[2].Value);

                if (x >= 0 && x < Width && y >= 0 && y < Height)
                {
                    slots[x, y] = child.gameObject;
                }
                else
                {
                    Debug.LogWarning($"Slot({x},{y}) is out of range!");
                }
            }
        }
    }

    // Метод для установки объекта в слот
    public void AssignToSlot(int x, int y, GameObject obj, bool savePos = false)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
        {
            Debug.LogWarning($"Invalid slot coordinates: ({x},{y})");
            return;
        }

        GameObject slot = slots[x, y];
        if (slot == null)
        {
            Debug.LogWarning($"No slot at ({x},{y})");
            return;
        }
        
        obj.transform.SetParent(slot.transform, false);
        slot.GetComponent<SlotEditorHelper>().UpdateChildren(savePos);
    }

#if UNITY_EDITOR
    [ContextMenu("Init Slots (Editor)")]
    private void InitSlotsEditor()
    {
        InitSlots();
        Debug.Log("Slots initialized in editor.");
    }
#endif
}

