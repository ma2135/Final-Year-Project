using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shield Object", menuName = "Inventory/Items/Equipment/Create Shield")]

public class ShieldObject : EquipmentObject
{

    private void Awake()
    {
        itemType = ItemType.Equipment;
        equipmentSlot = EquipmentSlot.LeftHand;
    }
}
