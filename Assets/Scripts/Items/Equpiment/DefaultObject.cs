using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Default Object", menuName = "Inventory/Items/Create Default Item")]

public class DefaultObject : ItemObject
{
    public void Awake()
    {
        itemType = ItemType.Default;
    }


}
