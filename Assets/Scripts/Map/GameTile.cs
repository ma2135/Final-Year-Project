using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Color = UnityEngine.Color;

public class GameTile : MonoBehaviour
{

    [SerializeField] Vector2Int matrixCoords;
    [SerializeField] Color highlightColour = Color.grey;
    private Color spawnColour;
    [SerializeField] private float highlightStrength = 0.3f;
    private UnitObject unit = null;
    private bool occupied = false;

    public void SetUp(Vector2Int matrixCoords)
    {
        this.matrixCoords = matrixCoords;
    }

    // Start is called before the first frame update
    void Start()
    {
        spawnColour = Color.clear;
        gameObject.GetComponent<SpriteRenderer>().color = Color.clear;
    }

    public void SetHighlightColour(Color highlightColour)
    {
        this.highlightColour = highlightColour;
    }

    public void HighlightTile(bool spawn)
    {
        //Color colour = gameObject.GetComponent<SpriteRenderer>().color;
        Color highlight;
        if (spawn)
        {
            if (spawnColour == Color.clear)
            {
                highlight = new Color(highlightColour.r, highlightColour.g, highlightColour.b, highlightStrength);

            }
            else
            {
                highlight = new Color(highlightColour.r + spawnColour.r, highlightColour.g + spawnColour.g, highlightColour.b + spawnColour.b, highlightStrength);
            }
        }
        else
        {
            highlight = new Color(highlightColour.r, highlightColour.g, highlightColour.b, highlightStrength);
        }
        gameObject.GetComponent<SpriteRenderer>().color = highlight;
    }

    public void UnHighlightTile(bool spawn)
    {
        if (spawn)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(spawnColour.r, spawnColour.g, spawnColour.b, highlightStrength);
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.clear;
        }
    }

    public void SetSpawnColour(Color colour)
    {
        spawnColour = colour;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(colour.r, colour.g, colour.b, 0.3f);
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
