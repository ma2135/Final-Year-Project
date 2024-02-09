using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Healing Object", menuName = "Inventory/Items/Create Healing Item")]

public class UtilityObject : ItemObject
{

    private void Awake()
    {
        itemType = ItemType.Utility;
        equipmentSlot = EquipmentSlot.Utility;
    }


}
