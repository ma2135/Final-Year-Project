using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


//What is interacted with by the user
public class Item : MonoBehaviour
{
    [SerializeField] ItemObject itemData = null;
    [SerializeField] Image itemImage;
    [SerializeField] TMP_Text itemCount;

    public ItemObject GetItemObject() { return itemData; }

    public void SetItemObject(ItemObject item) 
    {
        if (item == null)
        {
            itemImage.gameObject.SetActive(false);
            itemCount.gameObject.SetActive(false);
        }
        else
        {
            itemImage.gameObject.SetActive(true);
            itemCount.gameObject.SetActive(true);
        }
        itemData = item;
        itemImage.sprite = item.sprite;
        itemCount.text = item.amount.ToString();
        if (item.amount == 1 || item.amount == 0)
        {
            itemCount.gameObject.SetActive(false);
        }
        else
        {
            itemCount.gameObject.SetActive(true);
        }
    }

    public void OnValidate()
    {
        SetItemObject(itemData);
    }


}
