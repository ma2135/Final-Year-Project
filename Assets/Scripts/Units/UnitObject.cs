using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class UnitObject : ScriptableObject
{
    private Unit unit;

    [SerializeField] EquipmentObject[] equipedItems = new EquipmentObject[5];
    [SerializeField] DeckObject unitDeck;
    Vector2Int matrixCoords;
    int range = 3;

    private void Start()
    {
        unitDeck = new DeckObject();
        UpdateCards();
    }

    public void EquipUnit(EquipmentObject[] equipment)
    {
        if (equipment == null || equipment.Length != 5)
        {
            Debug.LogErrorFormat("New unit's equipment array length is incorrect: {0}", equipment.Length);
            return;
        }
        equipedItems = new EquipmentObject[] { equipment[0], equipment[1], equipment[2], equipment[3], equipment[4] };
        unitDeck = new DeckObject();
        UpdateCards();

    }

    public EquipmentObject EquipItem(EquipmentObject equipment, int itemLocation)
    {
        EquipmentObject oldItem = equipedItems[itemLocation];
        equipedItems[itemLocation] = equipment;
        return oldItem;
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
        return equipedItems;
    }

    private void UpdateCards()
    {
        if (unitDeck == null)
        {
            unitDeck = new DeckObject();
        }
        unitDeck.ClearDeck();
        foreach (var item in equipedItems)
        {
            if (item != null)
            {
                unitDeck.AddCards(item.GetCards());
            }
        }
    }

    public DeckObject GetUnitDeck()
    {
        return unitDeck;
    }

    public void SetUnit(Unit unit) { this.unit = unit; }
    public Unit GetUnit() { return unit; }

    public void Awake()
    {
        UpdateCards();
    }


}
