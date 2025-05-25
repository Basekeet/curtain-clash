using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    
    public CurtainVFX curtainVFX;
    [FormerlySerializedAs("woodObjectVFX")] public WoodObjectVFX StartButton;
    
    public GameObject player;
    public GameObject[] enemies;
    private int currentEnemy = 0;
    
    public ObjectSlotsHandling slots;
    private GameObject playerObj;
    private GameObject enemyObj;

    public GameObject prefabCard;
    public GameObject prefabCardSlotWithCard;
    public CardHandVFX cardHand;

    public CardData[] availableCards;
    
    public DescriptionManager descriptionManager;

    public WoodObjectVFX suggestion;
    
    public Vector2Int playerPosition;
    public Vector2Int enemyPosition;
    
    public List<GameObject> cardsOnField = new();
    
    void Start()
    {

        // SpawnCard(availableCards[ind], 2, 0);;
        
        // GameObject card = Instantiate(prefabCard, new Vector3(0, 0, 0), Quaternion.identity, transform.parent);
        // slots.AssignToSlot(2, 0, card);
    }
    IEnumerator Wrap(IEnumerator coroutine, Action onComplete)
    {
        yield return StartCoroutine(coroutine);
        onComplete?.Invoke();
    }
    public IEnumerator ClearAll()
    {
        var completed = 0;
        var target = 2 + cardHand.cards.Count;
        foreach (var card in cardHand.cards)
        {
            StartCoroutine(Wrap(RemoveCardFromHand(card), ()=>completed++));
        }

        StartCoroutine(Wrap(ClearPlayer(), ()=>completed++));
        StartCoroutine(Wrap(ClearEnemy(), ()=>completed++));
        ClearField();
        yield return new WaitUntil(() => completed == target);
    }

    public void InitAll()
    {
        StartCoroutine(SpawnPlayer(0, 0));
        StartCoroutine(SpawnEnemy(0, 3));
        
        int ind = Random.Range(0, availableCards.Length);
        AddCardToHand(availableCards[ind]);
        ind = Random.Range(0, availableCards.Length);
        AddCardToHand(availableCards[ind]);
        ind = Random.Range(0, availableCards.Length);
        AddCardToHand(availableCards[ind]);
        ind = Random.Range(0, availableCards.Length);
        AddCardToHand(availableCards[ind]);
        ind = Random.Range(0, availableCards.Length);
        AddCardToHand(availableCards[ind]);
        ind = Random.Range(0, availableCards.Length);
        AddCardToHand(availableCards[ind]);
    }

    public IEnumerator ClearPlayer()
    {
        if (playerObj == null) yield return null;
        else
        {
            playerObj.GetComponent<WoodObjectVFX>().Disappear();
            yield return new WaitForSeconds(0.5f);
            Destroy(playerObj);
            playerObj = null;
        }
    }
    public IEnumerator ClearEnemy()
    {
        if (enemyObj == null) yield return null;
        else
        {
            enemyObj.GetComponent<WoodObjectVFX>().Disappear();
            yield return new WaitForSeconds(enemyObj.GetComponent<WoodObjectVFX>().disappearDuration + 0.1f);
            Destroy(enemyObj);
            enemyObj = null;
        }
    }

    public void ClearField()
    {
        foreach (var card in cardsOnField)
        {
            StartCoroutine(RemoveCardFromField(card));
        }
        cardsOnField.Clear();
    }

    public IEnumerator SpawnPlayer(int x, int y)
    {
        playerObj = Instantiate(player, new Vector3(0, 0, 0), Quaternion.identity, transform.parent);
        playerObj.GetComponent<WoodObjectVFX>().IsAppearOnStart = false;
        slots.AssignToSlot(x, y, playerObj);
        yield return new WaitForSeconds(0.1f);
        playerObj.GetComponent<WoodObjectVFX>().Appear();
    }

    public IEnumerator SpawnEnemy(int x, int y)
    {
        enemyObj = Instantiate(enemies[currentEnemy], new Vector3(0, 0, 0), Quaternion.identity, transform.parent);
        enemyObj.GetComponent<WoodObjectVFX>().IsAppearOnStart = false;
        slots.AssignToSlot(x, y, enemyObj);
        yield return new WaitForSeconds(0.1f);
        enemyObj.GetComponent<WoodObjectVFX>().Appear();
    }

    public IEnumerator MovePlayer(int x, int y)
    {
        playerObj.GetComponent<WoodObjectVFX>().Disappear();
        yield return new WaitForSeconds(0.5f);
        slots.AssignToSlot(x, y, playerObj, true);
        playerObj.GetComponent<WoodObjectVFX>().Appear();
        yield return new WaitForSeconds(0.5f);
    }
    public IEnumerator MoveEnemy(int x, int y)
    {
        enemyObj.GetComponent<WoodObjectVFX>().Disappear();
        yield return new WaitForSeconds(0.5f);
        slots.AssignToSlot(x, y, enemyObj, true);
        enemyObj.GetComponent<WoodObjectVFX>().Appear();
        yield return new WaitForSeconds(0.5f);
    }

    public void AddCardToHand(CardData cardData)
    {
        GameObject card = Instantiate(prefabCardSlotWithCard, new Vector3(0, -10, 0), Quaternion.identity, transform.parent);
        card.transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = cardData.artwork;
        card.transform.GetChild(0).GetComponent<CardVFX>().cardData = cardData;
        card.transform.GetChild(0).GetComponent<CardVFX>().descriptionManager = descriptionManager;
        card.transform.GetChild(0).GetComponent<WoodObjectVFX>().OnClick += () =>
        {
            StartCoroutine(RemoveAndSpawnCardWithAnimation(card, cardData, playerPosition.x, playerPosition.y + 1));;
        };
        cardHand.AddCard(card);
    }
    
    public void AddCardToHandForView(CardData cardData)
    {
        GameObject card = Instantiate(prefabCardSlotWithCard, new Vector3(0, -10, 0), Quaternion.identity, transform.parent);
        card.transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = cardData.artwork;
        card.transform.GetChild(0).GetComponent<CardVFX>().cardData = cardData;
        card.transform.GetChild(0).GetComponent<CardVFX>().descriptionManager = descriptionManager;
        card.transform.GetChild(0).GetComponent<WoodObjectVFX>().OnClick += () =>
        {
            Debug.Log(cardData);
            StartCoroutine(FinishChoose());
        };
        cardHand.AddCard(card);
    }
    
    public IEnumerator FinishChoose()
    {
        yield return StartCoroutine(ClearAll());
        NextLevel();
    }

    IEnumerator RemoveAndSpawnCardWithAnimation(GameObject card, CardData cardData, int x, int y)
    {
        yield return StartCoroutine(RemoveCardFromHand(card));
        yield return StartCoroutine(SpawnCardOnField(cardData, x, y));
    }
    
    IEnumerator RemoveCardFromHand(GameObject card)
    {
        card.transform.GetChild(0).GetComponent<WoodObjectVFX>().Disappear();
        yield return new WaitForSeconds(card.transform.GetChild(0).GetComponent<WoodObjectVFX>().disappearDuration + 0.1f);
        cardHand.RemoveCard(card);
    }

    IEnumerator RemoveCardFromField(GameObject card)
    {
        card.GetComponent<WoodObjectVFX>().Disappear();
        yield return new WaitForSeconds(card.GetComponent<WoodObjectVFX>().disappearDuration + 0.1f);
        cardsOnField.Remove(card);
        Destroy(card);
    }
    
    public IEnumerator SpawnCardOnField(CardData cardData, int x, int y)
    {
        GameObject card = Instantiate(prefabCard, new Vector3(0, 0, 0), Quaternion.identity, transform.parent);
        card.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = cardData.artwork;
        card.GetComponent<CardVFX>().cardData = cardData;
        card.GetComponent<CardVFX>().descriptionManager = descriptionManager;
        card.GetComponent<WoodObjectVFX>().IsAppearOnStart = false;
        slots.AssignToSlot(x, y, card, false);
        yield return new WaitForSeconds(0.1f);
        card.GetComponent<WoodObjectVFX>().Appear();
        cardsOnField.Add(card);
    }

    public IEnumerator MoveCard(GameObject card, int x, int y)
    {
        card.GetComponent<WoodObjectVFX>().Disappear();
        yield return new WaitForSeconds(0.5f);
        slots.AssignToSlot(x, y, card, true);
        card.GetComponent<WoodObjectVFX>().Appear();
        yield return new WaitForSeconds(0.5f);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerObj.GetComponent<WoodObjectVFX>().Shake();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(AddNewCardGame());
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(ClearAll());
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            InitAll();
        }
    }
    
    
    public void StartGame()
    {
        InitAll();
        suggestion.Disappear();
        curtainVFX.OpenCurtains();
        StartButton.Disappear();
    }
    public void NextLevel()
    {
        currentEnemy++;
        InitAll();
        suggestion.Disappear();
        curtainVFX.OpenCurtains();
        StartButton.Disappear();
    }

    public IEnumerator AddNewCardGame()
    {
        yield return StartCoroutine(ClearAll());
        curtainVFX.CloseCurtains();
        int ind = Random.Range(0, availableCards.Length);
        AddCardToHandForView(availableCards[ind]);
        ind = Random.Range(0, availableCards.Length);
        AddCardToHandForView(availableCards[ind]);
        ind = Random.Range(0, availableCards.Length);
        AddCardToHandForView(availableCards[ind]);
        suggestion.Appear();
    }

    public void EndGame()
    {
        curtainVFX.CloseCurtains();
        StartButton.Appear();
    }
}
