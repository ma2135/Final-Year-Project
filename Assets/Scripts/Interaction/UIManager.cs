using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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
    }

    public IEnumerator SetCardToPosition(CardObject card, int position)
    {
        if (card == null)
        {
            Debug.LogAssertionFormat("Card is null");
            yield break;
        }
        Debug.LogFormat("Placing card ({1}) to hand position {0}", position, card.name);
        cardSlots[position].transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>().text = card.name;
        cardSlots[position].transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).GetComponent<TMP_Text>().text = card.cost.ToString();
        cardSlots[position].transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).GetComponent<TMP_Text>().text = card.range.ToString();
        cardSlots[position].transform.GetChild(0).transform.GetChild(3).transform.GetChild(0).GetComponent<TMP_Text>().text = card.description.ToString();
        cardSlots[position].SetActive(true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cardIndex"></param>
    public void PlayCard(int cardIndex)
    {
        CardObject card = EncounterManager.encounterManager.playersHand[cardIndex];
        Debug.LogFormat("Playing card ({0}) at ({1})", card.name, cardIndex);
        StartCoroutine(EncounterManager.encounterManager.PlayCard(cardIndex));
    }

    public void RemoveCardFromHand(int cardIndex)
    {
        CardObject discardCard = EncounterManager.encounterManager.playersHand[cardIndex];
        EncounterManager.encounterManager.playersHand[cardIndex] = null;
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
            Debug.LogFormat("Player party size: {0}", displayParty.size);
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
