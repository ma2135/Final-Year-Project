using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Party", menuName = "Units/Create Party")]

public class Party : ScriptableObject
{
    [SerializeField] private List<UnitObject> unitList = new List<UnitObject>();
    [SerializeField] private InventoryObject inventory;
    [SerializeField] private DeckObject deck;
    [SerializeField] int deckSize = 0;
    public int size = 0;
    private int delay = 0;
    private int maxDelay = 20;

    [SerializeField] private bool update = false;


    private void OnValidate()
    {
        if (update)
        {
            UpdateParty();
            update = false;
        }
    }

    public void SetUp()
    {
        if (inventory == null)
        {
            Debug.LogFormat("Creating new inventory in party: {0}", this.name);
            inventory = ScriptableObject.CreateInstance<InventoryObject>();
        }
        if (deck == null)
        {
            Debug.LogFormat("Creating new deck in party: {0}", this.name);
            deck = ScriptableObject.CreateInstance<DeckObject>();
        }
        size = unitList.Count;
        Debug.LogFormat("size: {0}", size);
        Debug.LogFormat(deck.DeckToString());
        UpdateParty();
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
            inventory = new InventoryObject();
        }
        /*
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
        */
    }

    private void UpdateDeck()
    {
        if (deck == null)
        {
            deck = new DeckObject();
        }
        else
        {
            deck.ClearDeck();
            Debug.LogFormat("Deck cleared");
            foreach (UnitObject unit in unitList)
            {
                if (unit != null)
                {
                    EquipmentObject[] equipment = unit.GetEquipment();

                    for (int i = 0; i < equipment.Length; i++)
                    {
                        if (equipment[i] != null)
                        {
                            Debug.LogFormat("Adding {0} card(s) to party deck from the item: {1}", equipment[i].GetCards(), equipment);
                            deck.AddCards(equipment[i].GetCards());
                        }
                    }
                }
            }
        }
        deckSize = deck.GetDeckSize();
        Debug.LogFormat("Deck size: {0}", deckSize);
    }

    public UnitObject GetUnit(int index)
    {
        return unitList[index];
    }
    public List<UnitObject> GetAllUnits()
    {
        return unitList;
    }

    public DeckObject GetDeck()
    {
        return deck;
    }

    /// <summary>
    /// Remove unit for the party
    /// </summary>
    /// <param name="unit"></param>
    public void RemoveUnit(UnitObject unit)
    {
        if (unit != null || !unitList.Contains(unit))
        {
            Debug.LogErrorFormat("Cannot remove unit. Either it does not exist or the unit was not a member of hte party specified");
        }
        else
        {
            unitList.Remove(unit);
        }
    }

    /// <summary>
    /// Remove unit from the party while keeping equipment
    /// </summary>
    /// <param name="unit"></param>
    public void KillUnit(UnitObject unit)
    {
        Debug.LogFormat("Unit ({0}) was killed in battle", unit.name.ToString());
        if (unit == null || !unitList.Contains(unit))
        {
            Debug.LogErrorFormat("Cannot remove unit. Either it does not exist or the unit was not a member of the party specified");
        }
        else
        {
            foreach(EquipmentObject equipment in unit.GetEquipment())
            {
                inventory.AddItem(equipment, 1);
            }
            unit.KillUnit();
            //unitList.Remove(unit);
        }
    }

}
