using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //represents an item in an inventory
    //For physical item interactions
    //To be draggend-and-dropped and clicked to move

    [SerializeField] private ItemSlot slot;
    [HideInInspector] public Transform parentAfterDrag;
    public Image image;

    public void OnValidate()
    {
        image = transform.GetComponent<Image>();
        //slot = transform.parent.GetComponent<ItemSlot>();
        if (slot != null )
        {
            GetComponent<Image>().sprite = slot.GetSprite();
        }
        
    }

    public ItemSlot GetItemSlot()
    {
        return slot;
    }

    public void SetItemSlot(ItemSlot slot)
    {
        this.slot = slot;
        transform.parent = slot.transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Start drag");
        parentAfterDrag = slot.transform;
        transform.parent = transform.root;
        transform.SetAsLastSibling();
        image.raycastTarget = false;

    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
        if (slot.GetItemObject() != null)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End drag");
        transform.SetParent(parentAfterDrag);
        transform.position = slot.transform.position;
        image.raycastTarget = true;
    }
}
