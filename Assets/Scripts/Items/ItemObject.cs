using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class ItemObject : ScriptableObject
{

    [SerializeField] protected ItemType itemType;
    [SerializeField] public Sprite sprite;
    [SerializeField] public EquipmentSlot equipmentSlot;
    [SerializeField] protected List<CardObject> cards;

    public ItemType GetItemType() { return itemType; }

    public List<CardObject> GetCards() { return cards; }

}
