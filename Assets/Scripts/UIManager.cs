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

    [SerializeField] private int maxHandSize = 5;
    private int currHandSize = 0;

    List<GameObject> hand = new List<GameObject>();
    [SerializeField] private GameObject[] cardSlots;
    [SerializeField] private bool[] freeCardSlots;

    [Header("Inventory")]
    [SerializeField] private ItemSlot[] equipSlots;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private ItemSlot[] inventorySlots;
    [SerializeField] private ItemSlot itemPrefab;
    [HideInInspector] public UnitObject currentUnit;
    [SerializeField] private Image unitImage = null;
    private Party displayParty = null;
    private int unitIndex = 0;



    private void Start()
    {
        uiManager = this;
        freeCardSlots = new bool[maxHandSize];
        for (int i = 0; i < freeCardSlots.Length; i++) 
        { 
            freeCardSlots[i] = true;
            cardSlots[i].SetActive(false);
        }
    }


    public void DrawCard()
    {
        if (GameManager.gameManager == null)
        {
            return;
        }
        if (GameManager.gameManager.GetPlayerDeckSize() < 1)
        {
            Debug.LogAssertionFormat("Deck size: {0}", GameManager.gameManager.GetPlayerDeckSize());
            return;
        }
        if (currHandSize < maxHandSize - 1)
        {
            //should only ever draw 1 card
            CardObject card = GameManager.gameManager.GetTopDeck(null);

            bool flag = false;
            for (int i = 0; i < cardSlots.Length; i++)
            {
                if (freeCardSlots[i])
                {
                    cardSlots[i].transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>().text = card.name;
                    cardSlots[i].transform.GetChild(0).transform.GetChild(1).GetComponent<TMP_Text>().text = card.cost.ToString();
                    freeCardSlots[i] = false;
                    flag = true;
                    cardSlots[i].SetActive(true);
                    break;
                }
            }
            if (flag)
            {
                Debug.LogAssertionFormat("All card slots used");
            }
        }
    }

    public void DisplayInventory()
    {
        displayParty = GameManager.gameManager.GetPlayerParty();
        unitIndex = 0;
        if (displayParty.size !> 0)
        {
            Debug.LogAssertionFormat("Plater party size: {0}", displayParty.size);
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


    // Update is called once per frame
    void Update()
    {
        
    }
}
