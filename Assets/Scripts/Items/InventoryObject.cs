using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://www.youtube.com/watch?v=_IqTeruf3-s&list=PLJWSdH2kAe_Ij7d7ZFR2NIW8QCJE74CyT&ab_channel=CodingWithUnity
[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Create Inventory")]
public class InventoryObject : ScriptableObject
{
    [SerializeField] private List<InvItemSlot> slots = new List<InvItemSlot>();
    // item used to add items to the inventory in the inspector
    [SerializeField] private ItemObject item = null;

    private void OnValidate()
    {
        if (item != null)
        {
            AddItem(item, 1);
            item = null;
        }
    }

    public InvItemSlot ItemInInventory(ItemObject item)
    {
        foreach (InvItemSlot slot in slots)
        {
            if (slot.GetItem() == item)
            {
                return slot;
            }
        }
        return null;
    }

    public void RemoveItem(ItemObject item, int amount)
    {
        InvItemSlot slot = ItemInInventory(item);
        if (slot != null)
        {
            slot.AddAmount(-amount);
        }
        if (slot.GetAmount() <= 0)
        {
            slots.Remove(slot);
        }
    }

    public void AddItem(ItemObject item, int amount)
    {
        InvItemSlot slot = ItemInInventory(item);
        if (slot != null)
        {
            slot.AddAmount(amount);
        }
        else
        {
            slots.Add(new InvItemSlot(item, amount));
        }
        /*
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
        */
    }

    public InvItemSlot[] GetItems()
    {
        return slots.ToArray();
    }

    public void AddInventory(InventoryObject inventory)
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

    public void AddAmount(int amount) 
    { 
        this.amount += amount; 
    }

}
