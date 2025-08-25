using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardHandVFX : MonoBehaviour
{
    [Header("Layout Settings")]
    public float spacing = 2.0f;
    public float yOffset = 0f;
    public float moveDuration = 0.3f; // ⏱ Длительность анимации

    public List<GameObject> cards = new List<GameObject>();

    public void AddCard(GameObject card)
    {
        cards.Add(card);
        card.transform.SetParent(transform, false);
        RepositionCards();
        card.GetComponent<SlotEditorHelper>().UpdateChildren();
    }

    public void RemoveCard(GameObject card)
    {
        if (cards.Contains(card))
        {
            cards.Remove(card);
            card.SetActive(false);
            StartCoroutine(DelayDestroy(card));
            RepositionCards();
        }
    }

    private IEnumerator DelayDestroy(GameObject card)
    {
        yield return new WaitForSeconds(2.0f);
        Destroy(card);
    }

    public void Start()
    {
        RepositionCards();
    }

    private void RepositionCards()
    {
        int count = cards.Count;
        float totalWidth = (count - 1) * spacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < count; i++)
        {
            Vector3 targetPos = new Vector3(startX + i * spacing, yOffset, 0f);
            cards[i].transform.DOLocalMove(targetPos, moveDuration).SetEase(Ease.OutCubic);
        }
    }

    public List<GameObject> GetCards()
    {
        return cards;
    }

    public void ClearHand()
    {
        foreach (var card in cards)
        {
            Destroy(card);
        }
        cards.Clear();
    }
}