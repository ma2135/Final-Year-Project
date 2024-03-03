using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Utility Object", menuName = "Inventory/Items/Create Utility Item")]

public class UtilityObject : EquipmentObject
{

    private void Awake()
    {
        itemType = ItemType.Utility;
        equipmentSlot = EquipmentSlot.Utility;
    }


}
