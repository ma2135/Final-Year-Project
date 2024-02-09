using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legacy_Tile : MonoBehaviour
{

    [SerializeField] Vector2Int matrixCoordinates;
    [SerializeField] float tileWidth = 2;
    [SerializeField] float tileHeight = 2;
    bool occupied = false;
    bool traversable = true;
    int traverseCost = 1;
    Unit unit = null;

    public void SetUp(Vector2Int mapPos)
    {
        matrixCoordinates = mapPos;
        PositionTile(mapPos);
    }

    public void PositionTile(Vector2Int matrixCoordinates)
    {
        Vector2 pos = new Vector2(matrixCoordinates.x * tileWidth, 0.75f * -matrixCoordinates.y * tileHeight);
        Debug.Log(string.Format("Coords: {0}, Position: {1}", matrixCoordinates, pos));
        transform.position = pos;
    }

    public void ShiftIndentTile(Vector2 tileShift)
    {
        transform.position = new Vector3 (transform.position.x + (0.5f * tileShift.x * tileWidth), transform.position.y - (tileShift.y * tileHeight), 0);
    }

    public void ShiftVectorTile(Vector2 tileShift)
    {
        transform.position += new Vector3(tileShift.x, tileShift.y, 0);
    }


    public Vector2Int GetMatrixCoordinates() { return matrixCoordinates; }
    public float GetTileWidth() {  return tileWidth; }
    public float GetTileHeight() {  return tileHeight; }
    public Unit GetUnit() { return unit; }  
    public void SetUnit(Unit unit) {  this.unit = unit; }
    public bool IsOccupied() { return occupied;}
    public bool IsTraversable() { return traversable;}
    public void SetTraversable(bool traversable) {  this.traversable = traversable; }
    public int GetTraverseCost() { return traverseCost; }



}
