using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

//https://www.youtube.com/watch?v=kWRyZ3hb1Vc&ab_channel=CocoCode
//What is interacted with by the user
public class ItemSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] ItemObject itemData = null;
    [SerializeField] Sprite itemSprite;
    [SerializeField] TMP_Text itemCount;
    public bool equipmentSlot = false;
    public EquipmentSlot equipSlot;

    public ItemObject GetItemObject() { return itemData; }

    public void SetItemObject(ItemObject item) 
    {
        if (item == null)
        {
            //itemSprite.gameObject.SetActive(false);
            itemCount.gameObject.SetActive(false);
            itemData = null;
        }
        else
        {
            //itemSprite.gameObject.SetActive(true);
            itemCount.gameObject.SetActive(true);
            itemData = item;
            itemSprite = item.sprite;
            itemCount.text = item.amount.ToString();
            if (item.amount == 1 || item.amount == 0)
            {
                itemCount.gameObject.SetActive(false);
            }
            else
            {
                itemCount.gameObject.SetActive(true);
            }
            if (equipmentSlot)
            {
                GameManager.gameManager.EquipItemToUnit((EquipmentObject)itemData, UIManager.uiManager.currentUnit, equipSlot);
            }
        }
        
    }

    public Sprite GetSprite() { return itemSprite; }

    public void OnValidate()
    {
        SetItemObject(itemData);
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedItem = eventData.pointerDrag;
        Item item = droppedItem.GetComponent<Item>();
        if (itemData == null)
        {
            item.parentAfterDrag = transform;
            SetItemObject(item.GetItemSlot().GetItemObject());
            item.GetItemSlot().SetItemObject(null);
            item.SetItemSlot(this);
        }
        else
        {

            //droppedItem.GetComponent<Item>()
        }
    }
}
