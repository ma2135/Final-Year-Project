using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//What is interacted with by the user
public class Item : MonoBehaviour
{
    [SerializeField] ItemObject itemData;

    public ItemObject GetItemObject() { return itemData; }
}
