using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Helmet Object", menuName = "Inventory/Items/Equipment/Create Helmet")]

public class HelmetObject : EquipmentObject
{
    private void Awake()
    {
        itemType = ItemType.Equipment;
        equipmentSlot = EquipmentSlot.Head;
    }
}
