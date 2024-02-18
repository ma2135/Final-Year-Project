using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Party", menuName = "Units/Create Party")]

public class Party : ScriptableObject
{
    [SerializeField] private List<UnitObject> unitList = new List<UnitObject>();
    [SerializeField] private Inventory inventory;
    [SerializeField] private DeckObject deck;
    public int size = 0;

    public void OnValidate()
    {
        UpdateParty();
    }

    private void Awake()
    {
        if (inventory == null)
        {
            inventory = CreateInstance<Inventory>();
        }
        if (deck == null)
        {
            deck = CreateInstance<DeckObject>();
        }
    }

    public void UpdateParty()
    {
        size = unitList.Count;
        UpdateInventory();
        UpdateDeck();
    }

    private void UpdateInventory()
    {
        if (inventory == null)
        {
            inventory = new Inventory();
        }
        foreach(UnitObject unit in unitList)
        {
            if (unit != null)
            {
                foreach(EquipmentObject item in unit.GetEquipment())
                {
                    if (item != null)
                    {
                        inventory.AddItem(item, 1);
                    }
                }
            }
        }
    }

    private void UpdateDeck()
    {
        if (deck == null)
        {
            deck = new DeckObject();
        }
        foreach (UnitObject unit in unitList)
        {
            if (unit!= null && unit.GetUnitDeck() != null)
            {
                deck.AddCards(unit.GetUnitDeck().GetXCards(unit.GetUnitDeck().GetDeckSize()));
            }
        }
    }

    public UnitObject GetUnit(int index)
    {
        return unitList[index];
    }
    public List<UnitObject> GetAllUnits()
    {
        return unitList;
    }
}
