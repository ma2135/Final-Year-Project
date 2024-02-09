using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Party", menuName = "Create Party")]

public class Party : ScriptableObject
{
    [SerializeField] private List<UnitObject> unitList = new List<UnitObject>();
    [SerializeField] private Inventory inventory;
    [SerializeField] private DeckObject deck;

    public void OnValidate()
    {
        UpdateParty();
    }

    public void UpdateParty()
    {
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


}
