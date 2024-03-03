using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;


[CreateAssetMenu(fileName = "New units", menuName = "Units/Create Unit")]

public class UnitObject : ScriptableObject
{
    private Unit unit;
    [SerializeField] public bool playerUnit = false;
    [SerializeField] private Sprite unitSprite;
    [SerializeField] private EquipmentObject[] equipedItems = new EquipmentObject[5];
    //[SerializeField] DeckObject unitDeck;
    Vector2Int matrixCoords;
    int range = 3;

    private void Awake()
    {
        //Setup();
    }

    private void OnValidate()
    {
        if (unit != null)
        {
            unit.GetComponent<SpriteRenderer>().sprite = unitSprite;
        }
        
    }

    private void Setup()
    {
        //unitDeck = new DeckObject();
        //UpdateCards();
    }

    public void EquipUnit(EquipmentObject[] equipment)
    {
        if (equipment == null || equipment.Length != 5)
        {
            Debug.LogErrorFormat("New unit's equipment array length is incorrect: {0}", equipment.Length);
            return;
        }
        for (int i = 0; i < equipment.Length; i++)
        {
            Debug.LogFormat("Equipping {0} to unit", equipment[i].name);
            this.equipedItems[i] = equipment[i];
        }
        //unitDeck = new DeckObject();
        //UpdateCards();
    }

    public EquipmentObject EquipItem(EquipmentObject equipment, EquipmentSlot itemLocation)
    {
        EquipmentObject oldItem = equipedItems[(int)itemLocation];
        equipedItems[(int)itemLocation] = equipment;
        return oldItem;
    }

    public Sprite GetUnitSprite()
    {
        return unitSprite;
    }
    public void SetUnitSprite(Sprite sprite)
    {
        unitSprite = sprite;
    }

    public void SetUp(Vector2Int coords)
    {
        matrixCoords = coords;
    }

    public void SetCoords(Vector2Int coords)
    {
        matrixCoords = coords;
    }

    public Vector2Int GetCoords()
    {
        return matrixCoords;
    }

    public void MoveUnit(Vector2Int matrixCoords, Vector3 gameCoords)
    {
        if (unit == null) { return; }
        SetCoords(matrixCoords);
        unit.transform.position = gameCoords;
    }

    public int GetRange()
    {
        return range;
    }

    public EquipmentObject[] GetEquipment()
    {
        if (equipedItems == null)
        {
            Debug.LogAssertion("Trying to get null equipment list from a unit");
        } 
        return equipedItems;
    }
    /*
    private void UpdateCards()
    {
        if (unitDeck == null)
        {
            unitDeck = new DeckObject();
        }
        unitDeck.ClearDeck();
        foreach (EquipmentObject item in equipedItems)
        {
            if (item != null)
            {
                Debug.LogFormat("Adding cards from {0} to deck: {1}", item, item.GetCards().ToString());
                foreach (CardObject card in item.GetCards())
                {
                    Debug.LogFormat("Adding {0} to hand", card.name);
                }
                unitDeck.AddCards(item.GetCards());
            }
        }
        Debug.LogAssertionFormat("unit decksize: {0}", unitDeck.GetDeckSize());
    }
    */
    /*
    public DeckObject GetUnitDeck()
    {
        return unitDeck;
    }
    */

    public void SetUnit(Unit unit) { this.unit = unit; }
    public Unit GetUnit() { return unit; }
}
