using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameTile : MonoBehaviour
{

    [SerializeField] Vector2Int matrixCoords;

    private UnitObject unit = null;
    private bool occupied = false;

    public void SetUp(Vector2Int matrixCoords)
    {
        this.matrixCoords = matrixCoords;
    }

    // Start is called before the first frame update
    void Start()
    {
        HideTile();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HideTile()
    {
        Color colour = gameObject.GetComponent<SpriteRenderer>().color;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(colour.r, colour.g, colour.b, 0);
    }
    public void ShowTile()
    {
        Color colour = gameObject.GetComponent<SpriteRenderer>().color;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(colour.r, colour.g, colour.b, 0.5f);
    }

    public bool IsOccupied()
    {
        return occupied;
    }

    public void SetOccupied(bool occupied)
    {
        this.occupied = occupied;
    }

    public void SetUnit(UnitObject unit)
    {
        this.unit = unit;
        if (unit != null)
        {
            occupied = true;
        }
        else 
        {
            occupied = false;
        }
    }
    public UnitObject GetUnit()
    {
        return this.unit;
    }

    public Vector2Int GetMatrixCoords()
    {
        return matrixCoords;
    }
}
