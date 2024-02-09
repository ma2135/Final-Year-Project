using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Armour Object", menuName = "Inventory/Items/Equipment/Create Armour")]

public class ArmourObject : EquipmentObject
{
    private void Awake()
    {
        itemType = ItemType.Equipment;
        equipmentSlot = EquipmentSlot.Torso;
    }
}
