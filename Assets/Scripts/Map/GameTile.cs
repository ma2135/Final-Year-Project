using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using Color = UnityEngine.Color;

public class GameTile : MonoBehaviour, IPointerClickHandler
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

    public void SelectTile()
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.LogFormat("Pointer Clicked at ({0})", matrixCoords.ToString());
        /*
        Debug.Log(eventData);

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.LogFormat("Left clicked at ({0})", matrixCoords.ToString());
            MapManager.mapManager.TileLeftClicked(matrixCoords);
        }
        else
        {
            Debug.LogFormat("Right clicked at ({0})", matrixCoords.ToString());
            MapManager.mapManager.TileRightClicked(matrixCoords);
        }*/
    }

    /*
    // https://stackoverflow.com/questions/57010713/unity-ispointerovergameobject-issue
    private bool IsMouseOverUI()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].gameObject.layer == 5) //5 = UI layer
            {
                Debug.LogFormat("UI clicked");
                return true;
            }
        }

        //Debug.LogAssertionFormat("Clicked UI: {0}", false);
        return false;
    }

    public void OnMouseDown()
    {
        MapManager.mapManager.selectedTile = null;
        Debug.LogAssertion("Cleared selectedTile");
        Debug.LogFormat("Mouse down at ({0})", matrixCoords.ToString());
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        if (!IsMouseOverUI())
        {
            Debug.Log(eventData);
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Debug.LogFormat("Left clicked at ({0})", matrixCoords.ToString());
                MapManager.mapManager.TileLeftClicked(matrixCoords);
            }
            else
            {

                Debug.LogFormat("Right clicked at ({0})", matrixCoords.ToString());
                MapManager.mapManager.TileRightClicked(matrixCoords);
            }
        }
    }
    */
}
