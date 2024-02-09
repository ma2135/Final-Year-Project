using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Object", menuName = "Inventory/Items/Equipment/Create Weapon")]

public class WeaponObject : EquipmentObject
{
    [SerializeField] public bool twoHanded = false;

    private void Awake()
    {
        itemType = ItemType.Equipment;
        equipmentSlot = EquipmentSlot.RightHand;
    }

}
