using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://www.youtube.com/watch?v=_IqTeruf3-s&list=PLJWSdH2kAe_Ij7d7ZFR2NIW8QCJE74CyT&ab_channel=CodingWithUnity
[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Create Inventory")]
public class Inventory : ScriptableObject
{
    [SerializeField] private List<InvItemSlot> slots = new List<InvItemSlot>();

    public void AddItem(ItemObject item, int amount)
    {
        bool flag = false;
        foreach (InvItemSlot slot in slots)
        {
            if (slot.GetItem() == item)
            {
                flag = true;
                slot.AddAmount(amount);
                break;
            }
        }
        if (!flag)
        {
            slots.Add(new InvItemSlot(item, 1));
        }
    }

    public InvItemSlot[] GetItems()
    {
        return slots.ToArray();
    }


    public void AddInventory(Inventory inventory)
    {
        foreach (InvItemSlot slot in inventory.GetItems())
        {
            slots.Add(slot);
        }
    }
}

[System.Serializable]
public class InvItemSlot
{
    [SerializeField] ItemObject item;
    [SerializeField] int amount = 1;

    public InvItemSlot(ItemObject item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }

    public ItemObject GetItem() { return item; }
    public int GetAmount() { return amount; }

    public void AddAmount(int amount) { this.amount += amount; }
}
