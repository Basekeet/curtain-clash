using UnityEngine;
using UnityEngine.EventSystems;

public class HoverBlocker : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Просто поглощаем событие — ничего не делаем
        Debug.Log("Hover blocked by this object");
    }
}