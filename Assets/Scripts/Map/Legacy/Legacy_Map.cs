using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legacy_Map : MonoBehaviour
{
    Legacy_Tile[,] tileMap = null;
    [SerializeField] GameObject tilePrefab;

    public void CreateMap(Vector2Int size)
    {
        Legacy_Tile[,] map = new Legacy_Tile[size.x, size.y];
        for (int i = 0; i < map.GetLength(0); i++)
        {
            int indent = 0;
            for (int j = 0; j < map.GetLength(1); j++)
            { 
                if (j > 0 && j % 2 == 0)
                {
                    indent++;
                }
                else
                {
                    indent--;
                }
                GameObject tileO = Instantiate(tilePrefab, this.transform);
                Legacy_Tile TileT = tileO.GetComponent<Legacy_Tile>();
                map[i, j] = TileT;
                TileT.SetUp(new Vector2Int(i, j));
                TileT.ShiftIndentTile(new Vector2Int(indent, 0));
            }
        }
        tileMap = map;
    }

    public void CenterMap()
    {
        if (tileMap[0, 0] == null)
        {
            Debug.LogAssertion(string.Format("Tilemap contains nothing at [0, 0]"));
        }
        float width = tileMap.GetLength(0) * tileMap[0, 0].GetTileWidth();
        float height = tileMap.GetLength(1) * tileMap[0, 0].GetTileHeight();
        if (tileMap.GetLength(1) > 1)
        {
            width += 0.5f * tileMap[0, 0].GetTileWidth();
        }

        Vector2 shift = new Vector2(-width/2, height/2);
        foreach (Legacy_Tile tile in tileMap)
        {
            tile.ShiftVectorTile(shift);
        }
    }





    private void Start()
    {
        CreateMap(new Vector2Int(8, 6));
        CenterMap();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
