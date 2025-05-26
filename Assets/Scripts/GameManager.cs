using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public AudioClip confirmSound;
    public AudioClip denySound;
    public AudioClip declineSound;
    public AudioClip hitSound;
    
    public AudioManager audioManager;
    public WoodObjectVFX tutorial;
    public WoodObjectVFX gameName;
    public WoodObjectVFX wonManager;
    public Transform logicalCards;
    public DescriptionManager TurnManager;
    
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
    public List<CardData> defaultDeck;
    public List<CardData> deck;
    
    public DescriptionManager descriptionManager;

    public WoodObjectVFX suggestion;

    public Vector2Int playerPosition;
    public Vector2Int enemyPosition;

    public bool isPlayerTurn = true;
    public int PlayerHealth = 5;
    public int EnemyHealth = 5;

    public Dictionary<int, GameObject> idToCardsInhand = new();
    public Dictionary<int, CardData> idToCardsData = new();
    public Dictionary<int, BaseCard> idToLogicalCards = new();
    public Dictionary<int, List<GameObject>> idToCardsOnField = new();
    public int id = 0;

    public IEnumerator SoftShakeCardInField(int id)
    {
        foreach (var card in idToCardsOnField[id])
        {
            card.GetComponent<WoodObjectVFX>().Shake(0.5f);
        }
        yield return new WaitForSeconds(idToCardsOnField[id][0].GetComponent<WoodObjectVFX>().shakeDuration + 0.1f);
    }

    public IEnumerator ShakePlayer()
    {
        playerObj.GetComponent<WoodObjectVFX>().Shake(1.0f);
        yield return new WaitForSeconds(playerObj.GetComponent<WoodObjectVFX>().shakeDuration + 0.1f);
    }
    
    public IEnumerator ShakeEnemy()
    {
        enemyObj.GetComponent<WoodObjectVFX>().Shake(1.0f);
        yield return new WaitForSeconds(enemyObj.GetComponent<WoodObjectVFX>().shakeDuration + 0.1f);
    }

    private void Start()
    {
        
    }

    private IEnumerator GameLoop()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            while (true)
            {
                if (blockPlayerTurn)
                {
                    blockPlayerTurn = false;
                }
                else
                {
                    ShowPlayerTurn();
                    // 0. Добавляем карту в руку
                    yield return StartCoroutine(GivePlayerCard());
                    // 1. Ждём, пока игрок сыграет карту
                    yield return StartCoroutine(WaitForPlayerToPlayCard());
                }

                ShowCardTurn();
                // 2. Обновляем все карты на поле
                yield return StartCoroutine(UpdateCards());

                if (PlayerHealth <= 0)
                {
                    EndGame();
                    yield break;
                }

                if (EnemyHealth <= 0)
                {
                    yield return StartCoroutine(AddNewCardGame());
                    break;
                }
                // TODO: CHECK DEATH

                if (blockEnemyTurn)
                {
                    blockEnemyTurn = false;
                }
                else
                {
                    ShowEnemyTurn();
                    // 3. Враг делает ход
                    yield return StartCoroutine(EnemyTurn());
                }

                ShowCardTurn();
                // 4. Снова обновляем все карты
                yield return StartCoroutine(UpdateCards());
                if (PlayerHealth <= 0)
                {
                    EndGame();
                    yield break;
                }

                if (EnemyHealth <= 0)
                {
                    yield return StartCoroutine(AddNewCardGame());
                    break;
                }
            }

            if (i != enemies.Length - 1) yield return WaitChooseCard();
        }
        Debug.Log("Game over");
        WonGame();
    }

    private bool blockEnemyTurn = false;
    private bool blockPlayerTurn = false;

    public void BlockEnemyTurn()
    {
        blockEnemyTurn = true;
    }
    public void BlockPlayerTurn()
    {
        blockPlayerTurn = true;
    }

    
    private bool playerHasChooseCard = false;
    public IEnumerator WaitChooseCard()
    {
        while (!playerHasChooseCard)
        {
            yield return null;
        }
        playerHasChooseCard = false; 
    }

    public void OnPlayerChooseCard()
    {
        playerHasChooseCard = true;
    }

    private CardData GenRandomCard()
    {
        int ind = 0;
        ind = Random.Range(1, 11) <= 3 ? Random.Range(0, 4) : Random.Range(4, deck.Count);

        return deck[ind];
    }

    private IEnumerator GivePlayerCard()
    {
        
        AddCardToHand(GenRandomCard());
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator WaitForPlayerToPlayCard()
    {
        isPlayerTurn = true;
        // Ждём, пока игрок выберет и активирует карту
        while (!playerHasPlayedCard)
        {
            yield return null;
        }
        playerHasPlayedCard = false; // сбрасываем флаг
        isPlayerTurn = false;
    }

    private IEnumerator EnemyTurn()
    {
        BaseCard cardComponent = null;
        while (true)
        {
            CardData[] enemyCards = enemyObj.GetComponent<CardDataToPrefab>().availableCards;
            int ind = Random.Range(0, enemyCards.Length);
            int id = GetID();
            GameObject newObj = new GameObject("Card");
            newObj.transform.SetParent(logicalCards, false);
            cardComponent = AddNecessaryComponent(enemyCards[ind], newObj);
            cardComponent.Initialize(this, id);
            idToCardsData.Add(id, enemyCards[ind]);
            idToLogicalCards.Add(id, cardComponent);

            if (cardComponent.CanActivate())
            {
                break;
            }
        }
        yield return StartCoroutine(cardComponent.Activate());
        yield return new WaitForSeconds(0.5f);
    }

    private bool playerHasPlayedCard = false;

    // Этот метод вызывается, когда игрок активирует карту
    public void OnPlayerPlayedCard()
    {
        playerHasPlayedCard = true;
    }
    
    public IEnumerator UpdateCards()
    {
        foreach (var card in idToLogicalCards)
        {
            if (idToCardsOnField.ContainsKey(card.Key) && idToCardsOnField[card.Key] != null)
            {
                yield return StartCoroutine(card.Value.UpdateCard());
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    public int GetID()
    {
        return id++;
    }

    public IEnumerator DamageEnemy(int damage)
    {
        EnemyHealth = Math.Max(0, EnemyHealth - damage);
        PlaySoundHit();
        yield return ShakeEnemy();
        // TODO: CHECK DAMAGE
    }

    public IEnumerator DamagePlayer(int damage)
    {
        PlayerHealth = Math.Max(0, PlayerHealth - damage);
        PlaySoundHit();
        yield return ShakePlayer();
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
        foreach (var card in idToCardsInhand)
        {
            if (card.Value == null) continue;
            StartCoroutine(Wrap(RemoveCardFromHand(card.Key), ()=>completed++));
        }

        StartCoroutine(Wrap(ClearPlayer(), ()=>completed++));
        StartCoroutine(Wrap(ClearEnemy(), ()=>completed++));
        ClearField();
        yield return new WaitUntil(() => completed >= target);
    }

    public void InitAll()
    {
        EnemyHealth = 5;
        PlayerHealth = 5;
        StartCoroutine(SpawnPlayer(1, 0));
        StartCoroutine(SpawnEnemy(1, 3));

        foreach (var card in defaultDeck)
        {
            AddCardToHand(card);
        }
        AddCardToHand(GenRandomCard());
        AddCardToHand(GenRandomCard());
        
        
    }

    public IEnumerator ClearPlayer()
    {
        if (playerObj == null) yield return null;
        else
        {
            playerObj.GetComponent<WoodObjectVFX>().Disappear();
            yield return new WaitForSeconds(playerObj.GetComponent<WoodObjectVFX>().disappearDuration + 0.01f);
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
            yield return new WaitForSeconds(enemyObj.GetComponent<WoodObjectVFX>().disappearDuration + 0.01f);
            Destroy(enemyObj);
            enemyObj = null;
        }
    }

    public void ClearField()
    {
        foreach (var card in idToCardsOnField)
        {
            StartCoroutine(RemoveCardFromField(card.Key));
        }
        idToCardsOnField.Clear();
    }

    public IEnumerator SpawnPlayer(int x, int y)
    {
        playerPosition = new Vector2Int(x, y);
        playerObj = Instantiate(player, new Vector3(0, 0, 0), Quaternion.identity, transform.parent);
        playerObj.GetComponent<PlayerVFX>().gameManager = this;
        playerObj.GetComponent<PlayerVFX>().descriptionManager = descriptionManager;
        playerObj.GetComponent<WoodObjectVFX>().IsAppearOnStart = false;
        slots.AssignToSlot(x, y, playerObj);
        yield return new WaitForSeconds(0.01f);
        PlaySoundWoop();
        playerObj.GetComponent<WoodObjectVFX>().Appear();
    }

    public IEnumerator SpawnEnemy(int x, int y)
    {
        enemyPosition = new Vector2Int(x, y);
        enemyObj = Instantiate(enemies[currentEnemy], new Vector3(0, 0, 0), Quaternion.identity, transform.parent);
        enemyObj.GetComponent<EnemyVFX>().gameManager = this;
        enemyObj.GetComponent<EnemyVFX>().descriptionManager = descriptionManager;
        enemyObj.GetComponent<WoodObjectVFX>().IsAppearOnStart = false;
        slots.AssignToSlot(x, y, enemyObj);
        yield return new WaitForSeconds(0.01f);
        PlaySoundWoop();
        enemyObj.GetComponent<WoodObjectVFX>().Appear();
    }

    public IEnumerator MovePlayer(int x, int y)
    {
        PlaySoundWoop();
        playerObj.GetComponent<WoodObjectVFX>().Disappear();
        yield return new WaitForSeconds(0.5f);
        slots.AssignToSlot(x, y, playerObj, true);
        PlaySoundWoop();
        playerObj.GetComponent<WoodObjectVFX>().Appear();
        yield return new WaitForSeconds(0.5f);
        playerPosition = new Vector2Int(x, y);
    }
    public IEnumerator MoveEnemy(int x, int y)
    {
        PlaySoundWoop();
        enemyObj.GetComponent<WoodObjectVFX>().Disappear();
        yield return new WaitForSeconds(0.5f);
        slots.AssignToSlot(x, y, enemyObj, true);
        PlaySoundWoop();
        enemyObj.GetComponent<WoodObjectVFX>().Appear();
        yield return new WaitForSeconds(0.5f);
        enemyPosition = new Vector2Int(x, y);
    }

    public BaseCard AddNecessaryComponent(CardData cardData, GameObject card)
    {
        BaseCard result = null;

        switch (cardData.cardName)
        {
            case "Fireball":
                result = card.AddComponent<BaseAttackCard>();
                break;
            case "Stun":
                result = card.AddComponent<StunCard>();
                break;
            case "Lasso":
                result = card.AddComponent<LassoTrapCard>();
                break;
            case "Sword":
                result = card.AddComponent<SwordAttackCard>();
                break;
            case "Snake":
                result = card.AddComponent<SnakeAttackCard>();
                break;
            case "Push":
                result = card.AddComponent<PushCard>();
                break;
            case "Stand":
                result = card.AddComponent<StandAttackCard>();
                break;
            case "NothingCard":
                result = card.AddComponent<NothingCard>();
                break;
            // Movement cards
            case "MoveLeft":
                result = card.AddComponent<MoveLeftCard>();
                break;
            case "MoveRight":
                result = card.AddComponent<MoveRightCard>();
                break;
            case "MoveUp":
                result = card.AddComponent<MoveUpCard>();
                break;
            case "MoveDown":
                result = card.AddComponent<MoveDownCard>();
                break;

            default:
                result = card.AddComponent<BaseAttackCard>();
                Debug.Log($"Unknown card type: {cardData.cardName}");
                break;
        }

        return result;
    }


    public bool IsInteract = false;
    public void AddCardToHand(CardData cardData)
    {
        GameObject card = Instantiate(prefabCardSlotWithCard, new Vector3(0, -10, 0), Quaternion.identity, transform.parent);
        card.transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = cardData.artwork;
        card.transform.GetChild(0).GetComponent<CardVFX>().cardData = cardData;
        card.transform.GetChild(0).GetComponent<CardVFX>().descriptionManager = descriptionManager;
        card.transform.GetChild(0).transform.GetChild(0).transform.localRotation = Quaternion.Euler(0, 0, cardData.angle);
        

        int id = GetID();
        GameObject newObj = new GameObject("Card");
        newObj.transform.SetParent(logicalCards, false);
        var cardComponent = AddNecessaryComponent(cardData, newObj);
        cardComponent.Initialize(this, id);
        idToCardsInhand.Add(id, card);
        idToCardsData.Add(id, cardData);
        idToLogicalCards.Add(id, cardComponent);
        
        card.transform.GetChild(0).GetComponent<WoodObjectVFX>().OnClick += () =>
        {
            if (isPlayerTurn && !IsInteract)
            {
                IsInteract = true;
                StartCoroutine(Interact(cardComponent));
            }
        };
        cardHand.AddCard(card);
    }

    public IEnumerator Interact(BaseCard cardComponent)
    {
        yield return cardComponent.Activate();
        yield return new WaitForSeconds(0.5f);
        OnPlayerPlayedCard();
        IsInteract = false;
    }
    
    public void AddCardToHandForView(CardData cardData)
    {
        GameObject card = Instantiate(prefabCardSlotWithCard, new Vector3(0, -10, 0), Quaternion.identity, transform.parent);
        card.transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = cardData.artwork;
        card.transform.GetChild(0).transform.GetChild(0).transform.localRotation = Quaternion.Euler(0, 0, cardData.angle);
        card.transform.GetChild(0).GetComponent<CardVFX>().cardData = cardData;
        card.transform.GetChild(0).GetComponent<CardVFX>().descriptionManager = descriptionManager;
        card.transform.GetChild(0).GetComponent<WoodObjectVFX>().OnClick += () =>
        {
            deck.Add(cardData);
            StartCoroutine(FinishChoose());
        };
        
        int id = GetID();
        idToCardsInhand.Add(id, card);
        idToCardsData.Add(id, cardData);
        
        cardHand.AddCard(card);
    }
    
    public IEnumerator FinishChoose()
    {
        yield return StartCoroutine(ClearAll());
        OnPlayerChooseCard();
        NextLevel();
    }

    // IEnumerator RemoveAndSpawnCardWithAnimation(GameObject card, CardData cardData, int x, int y)
    // {
    //     yield return StartCoroutine(RemoveCardFromHand(card));
    //     yield return StartCoroutine(SpawnCardOnField(cardData, x, y));
    // }
    
    public IEnumerator RemoveCardFromHand(int id)
    {
        var card = idToCardsInhand[id];
        if (card == null)
        {
            yield break;
        }

        card.transform.GetChild(0).GetComponent<WoodObjectVFX>().Disappear();
        yield return new WaitForSeconds(card.transform.GetChild(0).GetComponent<WoodObjectVFX>().disappearDuration + 0.01f);
        cardHand.RemoveCard(card);
    }

    public IEnumerator RemoveCardFromField(int id)
    {
        var cards = idToCardsOnField[id];
        foreach (var card in cards)
        {
            if (card == null)
            {
                continue;
            }
            card.GetComponent<WoodObjectVFX>().Disappear();
        }
        PlaySoundWoop();
        yield return new WaitForSeconds(cards[0].GetComponent<WoodObjectVFX>().disappearDuration + 0.01f);
        foreach (var card in cards)
        {
            Destroy(card);
        }
        idToCardsOnField.Remove(id);
    }
    
    public IEnumerator SpawnCardOnField(int id, int x, int y)
    {
        var cardData = idToCardsData[id];
        GameObject card = Instantiate(prefabCard, new Vector3(0, 0, 0), Quaternion.identity, transform.parent);
        if (idToCardsOnField.ContainsKey(id))
        {
            idToCardsOnField[id].Add(card);
        }
        else
        {
            idToCardsOnField.Add(id, new List<GameObject> {card});
        }
        
        card.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = cardData.artwork;
        if (!isPlayerTurn)
        {
            if (cardData.angle % 180 != 0)
            {
                card.transform.GetChild(0).GetComponent<SpriteRenderer>().flipY = true;
            }
            else
            {
                card.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = true;
            }

        }
        card.transform.GetChild(0).transform.localRotation = Quaternion.Euler(0, 0, cardData.angle);
        
        card.GetComponent<CardVFX>().cardData = cardData;
        card.GetComponent<CardVFX>().descriptionManager = descriptionManager;
        card.GetComponent<WoodObjectVFX>().IsAppearOnStart = false;
        slots.AssignToSlot(x, y, card, false);
        yield return new WaitForSeconds(0.01f);
        PlaySoundWoop();
        card.GetComponent<WoodObjectVFX>().Appear();
        yield return new WaitForSeconds(card.GetComponent<WoodObjectVFX>().disappearDuration + 0.01f);;
    }

    public IEnumerator MoveCard(int id, int x, int y)
    {
        foreach (var card in idToCardsOnField[id])
        {
            PlaySoundWoop();
            card.GetComponent<WoodObjectVFX>().Disappear();
            yield return new WaitForSeconds(card.GetComponent<WoodObjectVFX>().disappearDuration + 0.01f);
            slots.AssignToSlot(x, y, card, true);
            PlaySoundWoop();
            card.GetComponent<WoodObjectVFX>().Appear();
        }
        yield return new WaitForSeconds(idToCardsOnField[id][0].GetComponent<WoodObjectVFX>().disappearDuration + 0.01f);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            tutorial.Disappear();
        }

        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     playerObj.GetComponent<WoodObjectVFX>().Shake();
        // }
        //
        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     StartCoroutine(AddNewCardGame());
        // }
        //
        // if (Input.GetKeyDown(KeyCode.D))
        // {
        //     StartCoroutine(ClearAll());
        // }
        //
        // if (Input.GetKeyDown(KeyCode.W))
        // {
        //     StartCoroutine(UpdateCards());
        // }
        //
        // if (Input.GetKeyDown(KeyCode.S))
        // {
        //     InitAll();
        // }
        //
        // if (Input.GetKeyDown(KeyCode.R))
        // {
        //     EnemyHealth = 0;
        // }
    }

    public void ShowPlayerTurn()
    {
        TurnManager.ChangeDescription("Your turn");
    }

    public void ShowEnemyTurn()
    {
        TurnManager.ChangeDescription("Enemy turn");
    }

    public void ShowCardTurn()
    {
        TurnManager.ChangeDescription("Card turn");
    }

    public void PlaySoundWoop()
    {
        audioManager.PlaySoundEffect(declineSound);
    }

    public void PlaySoundHit()
    {
        audioManager.PlaySoundEffect(hitSound);
    }

    private Coroutine coroutine;
    public void StartGame()
    {
        audioManager.NextTrack();
        gameName.Disappear();
        wonManager.Disappear();
        deck.Clear();
        foreach (var card in defaultDeck)
        {
            deck.Add(card);
        }
        
        InitAll();
        coroutine = StartCoroutine(GameLoop());
        suggestion.Disappear();
        curtainVFX.OpenCurtains();
        StartButton.Disappear();
        TurnManager.ShowDescription();
        tutorial.Appear();
    }
    public void NextLevel()
    {
        if (currentEnemy >= enemies.Length)
        {
            return;
        }
        InitAll();
        TurnManager.ShowDescription();
        suggestion.Disappear();
        curtainVFX.OpenCurtains();
        StartButton.Disappear();
    }

    public IEnumerator AddNewCardGame()
    {
        currentEnemy++;
        if (enemies.Length <= currentEnemy)
        {
            yield break;
        }

        TurnManager.HideDescription();
        yield return StartCoroutine(ClearAll());
        curtainVFX.CloseCurtains();

        List<int> indices = new List<int>();
        for (int i = 4; i < availableCards.Length; i++)
        {
            indices.Add(i);
        }

        for (int i = 0; i < indices.Count; i++)
        {
            int randIndex = Random.Range(i, indices.Count);
            (indices[i], indices[randIndex]) = (indices[randIndex], indices[i]);
        }

        for (int i = 0; i < Mathf.Min(3, indices.Count); i++)
        {
            AddCardToHandForView(availableCards[indices[i]]);
        }

        suggestion.Appear();
    }

    public void WonGame()
    {
        wonManager.Appear();
        EndGame();
    }

    public void EndGame()
    {
        
        suggestion.Disappear();
        TurnManager.HideDescription();
        curtainVFX.CloseCurtains();
        StartCoroutine(ClearAll());
        currentEnemy = 0;
        StartButton.Appear();
        gameName.Appear();
        audioManager.NextTrack();
    }
}
