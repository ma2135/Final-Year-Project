using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager uiManager;

    [SerializeField] private Canvas HUDCanvas;
    [SerializeField] private GameObject handObj;
    [SerializeField] private GameObject cardPrefab;
    [Header("Player's Hand")]
    [SerializeField] private GameObject leftMarker;
    [SerializeField] private GameObject rightMarker;

    [SerializeField] public int maxHandSize = 5;
    private int currHandSize = 0;

    List<GameObject> hand = new List<GameObject>();
    [SerializeField] private GameObject[] cardSlots;
    [SerializeField] private CardObject[] handCards;

    [Header("Inventory")]
    [SerializeField] private ItemSlot[] equipSlots;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private ItemSlot[] inventorySlots;
    [SerializeField] private ItemSlot itemPrefab;
    [HideInInspector] public UnitObject currentUnit;
    [SerializeField] private Image unitImage = null;
    private Party displayParty = null;
    private int unitIndex = 0;

    [Header("Encounter")]
    [SerializeField] private GameObject playerTurnText;
    [SerializeField] private GameObject enemyTurnText;


    private void Start()
    {
        uiManager = this;
        handCards = new CardObject[maxHandSize];
        for (int i = 0; i < handCards.Length; i++)
        {
            cardSlots[i].SetActive(false);
        }
    }

    public IEnumerator DrawCard(DeckObject deck)
    {
        if (GameManager.gameManager == null)
        {
            yield break;
        }
        if (deck.GetDeckSize() < 1)
        {
            Debug.LogAssertionFormat("Deck size: {0}", deck.GetDeckSize());
            yield break;
        }
        if (currHandSize < maxHandSize - 1)
        {
            //should only ever draw 1 card
            CardObject card = deck.GetXCards(1)[0];
            Debug.LogFormat("playerHandSize = {0} || cardSlots.Length = {1}", handCards, cardSlots.Length);
            //bool flag = false;
            for (int i = 0; i < cardSlots.Length; i++)
            {
                if (handCards[i] == null)
                {
                    handCards[i] = card;
                    Debug.LogFormat("Placing card ({1}) to hand position {0}", i, card.name);
                    cardSlots[i].transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>().text = card.name;
                    cardSlots[i].transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).GetComponent<TMP_Text>().text = card.cost.ToString();
                    cardSlots[i].transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).GetComponent<TMP_Text>().text = card.range.ToString();
                    cardSlots[i].transform.GetChild(0).transform.GetChild(3).transform.GetChild(0).GetComponent<TMP_Text>().text = card.description.ToString();
                    //flag = true;
                    cardSlots[i].SetActive(true);
                    
                    break;
                }
            }
            /*
            if (flag)
            {
                Debug.LogAssertionFormat("All card slots used");
            }
            */
        }
    }

    public IEnumerator DrawStartingHand(DeckObject deck, int handSize)
    {
        int cardsDrawn = 0;
        while (cardsDrawn < handSize)
        {
            StartCoroutine(DrawCard(deck));
            cardsDrawn++;
        }
        yield break;
    }

    public void PlayCard(int cardIndex)
    {
        CardObject card = handCards[cardIndex];
        Debug.LogFormat("Playing card ({0})", card.name);
        StartCoroutine(EncounterManager.encounterManager.PlayCard(cardIndex));
    }

    public void RemoveCardFromHand(int cardIndex)
    {
        CardObject discardCard = handCards[cardIndex];
        handCards[cardIndex] = null;
        cardSlots[cardIndex].SetActive(false);
    }


    public void StartEncounter()
    {
        StartCoroutine(EncounterManager.encounterManager.StartEncounter());
    }



    public void DisplayInventory()
    {
        displayParty = GameManager.gameManager.GetPlayerParty();
        unitIndex = 0;
        if (displayParty.size !> 0)
        {
            Debug.LogAssertionFormat("Player party size: {0}", displayParty.size);
        }
        else
        {
            DisplayUnit(displayParty.GetUnit(0));
        }
    }
    public void CycleUnitUp()
    {
        unitIndex += 1;
        if (unitIndex > displayParty.size)
        {
            unitIndex = 0;
        }
        DisplayUnit(displayParty.GetUnit(unitIndex));
    }
    public void CycleUnitDown()
    {
        unitIndex -= 1;
        if (unitIndex > 0)
        {
            unitIndex = displayParty.size;
        }
        DisplayUnit(displayParty.GetUnit(unitIndex));
    }

    public void DisplayUnit(UnitObject unit)
    {
        currentUnit = unit;
        for (int i = 0; i <= equipSlots.Length; i++ )
        {
            equipSlots[i].SetItemObject(unit.GetEquipment()[i]);
        }
        unitImage.sprite = unit.GetUnitSprite();
    }

    public void DisplayCurrentTurn(bool player)
    {
        if (player)
        {
            playerTurnText.SetActive(true);
            enemyTurnText.SetActive(false);
        }
        else
        {
            enemyTurnText.SetActive(true);
            playerTurnText.SetActive(false);
        }
    }

    public void EndPlayerTurn()
    {
        StartCoroutine(EncounterManager.encounterManager.EndTurn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
