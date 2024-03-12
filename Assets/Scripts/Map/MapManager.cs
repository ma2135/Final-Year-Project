using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;
using Utils;
using static UnityEditor.Progress;

public class MapManager : MonoBehaviour
{
    private static MapManager map;
    public static MapManager mapManager { get { return map; } }
    private void Awake()
    {
        if (map != null && map != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            map = this;
        }
    }

    private GameTile[,] gameTiles;
    [SerializeField] private TileBase originTile = null;

    [Header("Tilemap")]
    [SerializeField] Tilemap tilemap;
    [SerializeField] TilemapCollider2D collider;
    Vector2Int matrixOffset = Vector2Int.zero;

    [Header("Tiles")]
    [SerializeField] GameObject overlayTilePrefab;
    [SerializeField] GameObject overlayContainer;
    [SerializeField] public GameTile selectedTile = null;

    [SerializeField] public UnitObject selectedUnit = null;
    private List<GameTile> highlightedTiles = new List<GameTile>();
    [SerializeField] const int TILE_Z = -1;
    private GameTile[,] playerSpawn = null;
    private int playerSpawnWidth;

    [Header("Units")]
    [SerializeField] GameObject unitPrefab;
    [SerializeField] GameObject unitContainer;

    public bool unitsMovable = true;
    private bool unitSelectionFailed;
    private bool unitSelectionInProgress;



    /*
    public void TileClicked(GameTile inputTile, bool leftClicked)
    {
        UnhighlightTiles(highlightedTiles);
        if (leftClicked)
        {
            if (inputTile != selectedTile)
            {
                if (selectedTile != null)
                {
                    selectedTile.UnHighlightTile(EncounterManager.encounterManager.setupPhase);
                }
                selectedTile = inputTile;
                selectedTile.HighlightTile(EncounterManager.encounterManager.setupPhase);
                selectedUnit = selectedTile.GetUnit();
            }
            else
            {
                if (selectedTile != null)
                {
                    selectedTile.UnHighlightTile(EncounterManager.encounterManager.setupPhase);;
                }
                selectedUnit = null; 
                selectedTile = null;
            }
            if (selectedUnit != null)
            {
                if (selectedUnit.playerUnit == false)
                {
                    selectedUnit = null;
                }
            }
            if (selectedUnit != null)
            {
                HighlightTiles(GetTilesInRange(selectedTile, selectedUnit.GetMovementRange()));
            }
        }
        else
        {
            if (selectedUnit != null)
            {
                MoveUnit(selectedTile, inputTile);
            }
        }
        selectedCoords = selectedTile.GetMatrixCoords();

    }
    */


    public void TileLeftClicked(Vector2Int tileCoords)
    {
        Debug.Log("Tile Clicked");
        selectedTile = gameTiles[tileCoords.x, tileCoords.y];
        UnhighlightTiles(highlightedTiles);
        if (selectedTile.GetUnit() != null && selectedTile.GetUnit().playerUnit && unitsMovable)
        {
            selectedUnit = selectedTile.GetUnit();
            HighlightTiles(GetTilesInRange(selectedTile, selectedUnit.GetMovementRange()));
        }
        else
        {
            selectedUnit = null;
            HighlightTiles(new List<GameTile> {selectedTile});
        }
    }

    public void TileRightClicked(Vector2Int tileCoords)
    {
        GameTile selectedTile = gameTiles[tileCoords.x, tileCoords.y];
        if (selectedUnit != null)
        {
            Vector2Int unitCoords = selectedUnit.GetCoords();
            MoveUnit(gameTiles[unitCoords.x, unitCoords.y], selectedTile);
        }
        UnhighlightTiles(highlightedTiles);
    }

    public void SetPlayerSpawnWidth(int width)
    {
        playerSpawnWidth = width;
    }

    /// <summary>
    /// Creates a unitGame on the input coordinates
    /// </summary>
    /// <param name="matrixCoord"></param>
    public void CreateNewUnit(Vector2Int matrixCoord)
    {
        if (matrixCoord == null)
        {
            Debug.LogAssertion("Input coordinates are null");
        }
        Vector2Int tileCoord = matrixCoord;
        if (gameTiles[tileCoord.x, tileCoord.y] == null)
        {
            Debug.LogAssertion(string.Format("Could not create unit at {0} as no tile exists", tileCoord));
            return;
        } 
        if (gameTiles[tileCoord.x, tileCoord.y].GetUnit() != null)
        {
            Debug.LogAssertion(string.Format("Tile {0} already occupied", matrixCoord));
        }
        GameObject unitGame = Instantiate(unitPrefab, unitContainer.transform);
        Unit unit = unitGame.GetComponent<Unit>();
        UnitObject unitObj = ScriptableObject.CreateInstance<UnitObject>();
        unitObj.SetUnit(unitGame.GetComponent<Unit>());
        unit.SetUnitObject(unitObj);
        unitObj.MoveUnit(matrixCoord, gameTiles[matrixCoord.x, matrixCoord.y].transform.position);
        //unitGame.transform.position = gameTiles[tileCoord.x, tileCoord.y].transform.position;
        gameTiles[matrixCoord.x, matrixCoord.y].SetUnit(unitObj);
        if (unitObj == null)
        {
            Debug.LogAssertion("unitObj = null");
        }
        if (GameManager.gameManager == null)
        {
            Debug.LogAssertion("GameManager.gameManager is null");
        }
        GameManager.gameManager.EquipUnitStart(unitObj);
    }

    public void CreateUnit(Vector2Int matrixCoords, UnitObject unitData)
    {
        if (matrixCoords == null)
        {
            Debug.LogAssertion("Input coordinates are null");
        }
        if (gameTiles[matrixCoords.x, matrixCoords.y] == null)
        {
            Debug.LogAssertion(string.Format("Could not create unit at {0} as no tile exists", matrixCoords));
            return;
        }
        if (gameTiles[matrixCoords.x, matrixCoords.y].GetUnit() != null)
        {
            Debug.LogAssertion(string.Format("Tile {0} already occupied by {1}", matrixCoords, gameTiles[matrixCoords.x, matrixCoords.y].GetUnit().name));
        }
        GameObject unitGame = Instantiate(unitPrefab, unitContainer.transform);
        Unit unit = unitGame.GetComponent<Unit>();
        unit.SetUnitObject(unitData);
        unitData.SetUnit(unitGame.GetComponent<Unit>());
        unitData.MoveUnit(matrixCoords, gameTiles[matrixCoords.x, matrixCoords.y].transform.position);
        //unitGame.transform.position = gameTiles[tileCoord.x, tileCoord.y].transform.position;
        gameTiles[matrixCoords.x, matrixCoords.y].SetUnit(unitData);
        if (unitData == null)
        {
            Debug.LogAssertion("unitObj = null");
        }
        if (GameManager.gameManager == null)
        {
            Debug.LogAssertion("GameManager.gameManager is null");
        }
    }

    #region Unit Movement
    public bool MoveUnit(GameTile unitTile, GameTile destinationTile)
    {
        if (unitTile == null || destinationTile == null)
        {
            Debug.LogError("MoveUnit - Null input");
        }
        if (unitTile.GetUnit() == null)
        {
            Debug.LogError(string.Format("MoveUnit - No unit on tile {0} to move", unitTile.GetMatrixCoords()));
            return false;
        }
        if (destinationTile.GetUnit() != null)
        {
            Debug.LogError(string.Format("MoveUnit - Cannot move unit to destination. Tile {0} is already occupied", destinationTile.GetMatrixCoords()));
            return false;
        }

        if (!highlightedTiles.Contains(destinationTile))
        {
            Debug.Log("Trying to move to unhighlighted tile");
            return false;
        }

        //get the path to the destination tile
        //move the board movement movement number of tiles from the stack

        Stack<GameTile> path = GetPathToTile(unitTile, destinationTile);
        Debug.LogFormat("Move - path size: {0}", path.Count);
        GameTile previousTile;
        GameTile currentTile = unitTile;

        int count = Mathf.Min(path.Count, unitTile.GetUnit().GetMovementRange());
        Debug.LogFormat("Max movement range: {0}", count);
        while (count > 0)
        {
            previousTile = currentTile;
            currentTile = path.Pop();
            Debug.LogFormat("Removing {0} from stack", currentTile.GetMatrixCoords());

            currentTile.SetUnit(previousTile.GetUnit());
            previousTile.SetUnit(null);
            currentTile.GetUnit().MoveUnit(currentTile.GetMatrixCoords(), currentTile.transform.position);
            Debug.LogFormat("Moving tile from {0} to {1}", previousTile.GetMatrixCoords(), currentTile.GetMatrixCoords());
            count--;
        }
        return true;
    }

    // https://www.redblobgames.com/pathfinding/a-star/introduction.html
    public Stack<GameTile> GetPathToTile(GameTile startTile, GameTile destinationTile)
    {
        if (startTile == null)
        {
            Debug.LogError("GetPathToTile - No start tile");
            return null;
        }
        if (destinationTile == null)
        {
            Debug.LogError("GetPathToTile - No destination tile");
            return null;
        }

        // Change to true A*
        // Give each node a value (path newCost) on the way through
        // On way back, next node is the one with the lowest value


        PriorityQueue<GameTile, float> fringe = new PriorityQueue<GameTile, float>();
        bool found = false;
        GameTile current = startTile;
        Dictionary<GameTile, int> pathCost = new Dictionary<GameTile, int>();
        Dictionary<GameTile, GameTile> path = new Dictionary<GameTile, GameTile>();
        pathCost.Add(startTile, 0);
        path.Add(startTile, null);
        fringe.Enqueue(current, CubicDistance(MatrixToCubic(current.GetMatrixCoords()), MatrixToCubic(destinationTile.GetMatrixCoords())));

        while (!found && fringe.Count > 0)
        {
            current = fringe.Dequeue();
            if (current == destinationTile)
            {
                found = true;
            }
            else
            {
                foreach (GameTile neighbour in GetTilesInRange(current, 1))
                {
                    int newCost = pathCost[current] + 1;
                    if (!pathCost.ContainsKey(neighbour) || pathCost[neighbour] > newCost)
                    {
                        pathCost[neighbour] = newCost;
                        fringe.Enqueue(neighbour, newCost + CubicDistance(MatrixToCubic(current.GetMatrixCoords()), MatrixToCubic(destinationTile.GetMatrixCoords())));
                        path[neighbour] = current;
                    }
                }
            }
        }

        Stack<GameTile> pathStack = new Stack<GameTile>();
        while (current != startTile)
        {
            Debug.LogFormat("Adding {0} to stack", current.GetMatrixCoords());
            pathStack.Push(current);
            current = path[current];
        }

        Debug.Log(TileArrayToString(TileStackToArray(pathStack)));
        return pathStack;

        /*
        PriorityQueue<GameTile, float> fringe = new PriorityQueue<GameTile, float>();
        Stack<GameTile> path = new Stack<GameTile>();
        bool found = false;
        GameTile current = startTile;
        fringe.Enqueue(current, CubicDistance(MatrixToCubic(current.GetMatrixCoords()), MatrixToCubic(destinationTile.GetMatrixCoords())));

        while (!found && fringe.Count > 0)
        {
            current = fringe.Dequeue();
            if (path.Contains(current))
            {
                while (path.Peek() == current)
                {
                    path.Pop();
                }
            }
            else
            {
                path.Push(current);
            }
            
            if (current == destinationTile)
            {
                found = true;
            }
            else
            {
                foreach (GameTile neighbour in GetTilesInRange(current, 1))
                {
                    if (!path.Contains(neighbour)  && !neighbour.IsOccupied())
                    {
                        fringe.Enqueue(neighbour, CubicDistance(MatrixToCubic(neighbour.GetMatrixCoords()), MatrixToCubic(destinationTile.GetMatrixCoords())));
                    }
                }
            }
        }

        //&& !QueueContains(fringe, neighbour)          //Check if the tile is already in the stack - may not be needed - stack may already checka dn update priority

        Stack<GameTile> toTravel = new Stack<GameTile>();
        while (path.Count > 0)
        {
            toTravel.Push(path.Pop());
        }
        Debug.Log(TileArrayToString(TileStackToArray(toTravel)));
        return toTravel;
        */
    }

    #endregion

    /// <summary>
    /// Returns a list of tiles about the center neighbour in a specified movement
    /// </summary>
    /// <param name="centerTile"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public List<GameTile> GetTilesInRange(GameTile centerTile, int range)
    {
        Vector2Int centerMatrix = centerTile.GetMatrixCoords();
        Vector3Int centerCubic = MatrixToCubic(centerMatrix);
        List<GameTile> tiles = new List<GameTile>();
        int indent;
        for (int y = Mathf.Max(centerMatrix.y - range, 0); y <= Mathf.Min(centerMatrix.y + range, gameTiles.GetLength(1) - 1); y++)
        {
            if (centerTile.GetMatrixCoords().y % 2 == 0 && y % 2 != 0)
            {
                indent = -1;
            }
            else if (centerTile.GetMatrixCoords().y % 2 != 0 && y % 2 == 0)
            {
                indent = 1;
            }
            else
            {
                indent = 0;
            }
            for (int x = Mathf.Max(centerMatrix.x - range, 0) + indent; x <= Mathf.Min(centerMatrix.x + range, gameTiles.GetLength(0) - 1); x++)
            {
                if (gameTiles[x, y] != null)
                {
                    Vector3Int tileCubic = MatrixToCubic(new Vector2Int(x, y));
                    if (Mathf.Abs(tileCubic.x - centerCubic.x) <= range && Mathf.Abs(tileCubic.y - centerCubic.y) <= range && Mathf.Abs(tileCubic.z - centerCubic.z) <= range)
                    {
                        tiles.Add(gameTiles[x, y]);
                    }
                }
            }
        }
        return tiles;
    }
    public List<Vector2Int> GetTilesInRange(Vector2Int centerTile, int range)
    {
        Vector2Int centerMatrix = centerTile;
        Vector3Int centerCubic = MatrixToCubic(centerMatrix);
        List<Vector2Int> tiles = new List<Vector2Int>();
        int indent;
        for (int y = Mathf.Max(centerMatrix.y - range, 0); y <= Mathf.Min(centerMatrix.y + range, gameTiles.GetLength(1) - 1); y++)
        {
            if (centerTile.y % 2 == 0 && y % 2 != 0)
            {
                indent = -1;
            }
            else if (centerTile.y % 2 != 0 && y % 2 == 0)
            {
                indent = 1;
            }
            else
            {
                indent = 0;
            }
            for (int x = Mathf.Max(centerMatrix.x - range, 0) + indent; x <= Mathf.Min(centerMatrix.x + range, gameTiles.GetLength(0) - 1); x++)
            {
                if (gameTiles[x, y] != null)
                {
                    Vector3Int tileCubic = MatrixToCubic(new Vector2Int(x, y));
                    if (Mathf.Abs(tileCubic.x - centerCubic.x) <= range && Mathf.Abs(tileCubic.y - centerCubic.y) <= range && Mathf.Abs(tileCubic.z - centerCubic.z) <= range)
                    {
                        tiles.Add(new Vector2Int(x, y));
                    }
                }
            }
        }
        return tiles;
    }

    public GameTile[,] GetGameTiles()
    {
        return gameTiles;
    }
    /*
    public bool QueueContains(PriorityQueue<GameTile, float> stack, GameTile tile)
    {
        PriorityQueue<GameTile, float> queueCopy = stack;
        int target = stack.Count;
        while (queueCopy.Count < target)
        {

            stack

            GameTile temp = stack.Dequeue();
            queueCopy.Enqueue(temp);
            stack.Enqueue(temp);
        }



        while (queueCopy.Count > 0)
        {
            GameTile temp = queueCopy.Dequeue();
            if (temp = tile)
            {
                return true;
            }
        }
        return false;
    }
    */


    #region Highlighting
    /// <summary>
    /// Highlights the tiles in the input list
    /// </summary>
    /// <param name="tileList"></param>
    private void HighlightTiles(List<GameTile> tileList)
    {
        bool setup = EncounterManager.encounterManager.setupPhase;
        foreach (GameTile tile in tileList)
        {
            if (setup && tile.GetMatrixCoords().x < playerSpawnWidth)
            {
                if (!highlightedTiles.Contains(tile))
                {
                    highlightedTiles.Add(tile);
                }
                tile.HighlightTile(setup);
            }
            else if (!setup)
            {
                tile.HighlightTile(setup);
                highlightedTiles.Add(tile);
            }
        }
    }

    /// <summary>
    /// Unhighlights the tiles in the input list if they are highlighted
    /// </summary>
    /// <param name="tileList"></param>
    private void UnhighlightTiles(List<GameTile> tileList)
    {
        //Debug.LogFormat("Unhighlighting tilelist length: ({0})", spawnZone.Count);
        bool spawn = EncounterManager.encounterManager.setupPhase;
        if (tileList == null)
        {
            Debug.Log("UnhighlightTiles - input list is null");
            return;
        }

        for (int i = 0; i < tileList.Count; i++)
        {
            if (tileList[i] != null && highlightedTiles.Contains(tileList[i]))
            {
                //Debug.LogFormat("Unhighlighting tile ({0})", tileList[i].GetMatrixCoords().ToString());
                tileList[i].UnHighlightTile(spawn);
            }
        }
        while (tileList.Count > 0)
        {
            tileList.RemoveAt(0);
        }
    }

    public void SetTilesAsSpawn(GameTile[,] spawnZone, Color colour)
    {
        foreach (GameTile tile in spawnZone)
        {
            if (tile != null)
            {
                tile.SetSpawnColour(colour);
            }
        }
    }

    public void HideSpawnZone(GameTile[,] spawnZone)
    {
        foreach (GameTile tile in spawnZone)
        {
            if (tile != null)
            {
                tile.UnHighlightTile(false);
            }
        }
    }
    #endregion

    #region Coordinate Conversion

    public Vector3Int MatrixToCubic(Vector2Int matrixCoords)
    {
        // "&1" to get odd or even as works with -ve numbers
        int q = matrixCoords.x - (matrixCoords.y - (matrixCoords.y & 1)) / 2;
        int r = matrixCoords.y;
        return new Vector3Int(q, r, -q - r);
    }

    public Vector2Int CubicToMatrix(Vector3Int cubicCoords)
    {
        int x = cubicCoords.x + (cubicCoords.y - (cubicCoords.y & 1)) / 2;
        int y = cubicCoords.y;
        return new Vector2Int(x, y);
    }

    public float CubicDistance(Vector3Int start, Vector3Int end)
    {
        Vector3Int difference = CubicSubtraction(start, end);
        return (Mathf.Abs(difference.x) + Mathf.Abs(difference.y) + Mathf.Abs(difference.z)) / 2;
    }

    private Vector3Int CubicSubtraction(Vector3Int a, Vector3Int b)
    {
        return new Vector3Int(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    #endregion

    #region StringAdmin

    private string TileArrayToString(GameTile[] tileArray)
    {
        if (tileArray == null || tileArray.Length == 0)
        {
            Debug.LogAssertion("TileArrayToString - input array is null or has a length of 0");
        }
        string output = string.Empty;
        for (int i = 0; i < tileArray.Length; i++)
        {
            output += tileArray[i].GetMatrixCoords();
            if (i != tileArray.Length - 1)
            {
                output += ", ";
            }
        }
        return output;
    }

    private GameTile[] TileStackToArray(Stack<GameTile> tileStack)
    {

        if (tileStack == null || tileStack.Count == 0)
        {
            Debug.LogAssertion("TileStackToArray - input stack is null or has a length of 0");
        }
        GameTile[] output = new GameTile[tileStack.Count];
        Stack<GameTile> stack = new Stack<GameTile>();
        int count = tileStack.Count;
        for (int i = 0; i < count; i++)
        {
            GameTile temp = tileStack.Pop();
            output[i] = temp;
            stack.Push(temp);
        }
        while (stack.Count > 0)
        {
            tileStack.Push(stack.Pop());
        }
        Debug.LogFormat("Tile Stack to Array: {0}", output);
        /*
        for (int i = 0; i < output.Length; i++)
        {
            Debug.LogFormat("Output {0}: {1}", i, output[i].GetMatrixCoords());
        }*/
        return output;
    }



    #endregion


    // Start is called before the first frame update
    void Start()
    {
        BoundsInt bounds = tilemap.cellBounds;


        //tilemap.SetTile(new Vector3Int(0, 0, 0), originTile);
        //gameTiles = new GameTile[Mathf.Abs(bounds.min.x) + bounds.max.x, Mathf.Abs(bounds.min.y) + bounds.max.y];
        /*
        gameTiles = new GameTile[Mathf.Abs(bounds.max.x) + Mathf.Abs(bounds.min.x), Mathf.Abs(bounds.max.y) + Mathf.Abs(bounds.min.y)];
        Debug.LogFormat("gameTiles shape: [{0}, {1}]", gameTiles.GetLength(0), gameTiles.GetLength(1));
        Debug.LogFormat("gameTiles min and max: [{0}, {1}]", bounds.min, bounds.max);
        */
        //Debug.LogFormat("Tilemap layout: {0}", tilemap.cellLayout);

        /*
        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                if (minX > x) { minX = x; }
                if (minY > y) { minY = y; }
                if (maxX < x) { maxX = x; }
                if (maxY < y) { maxY = y; }
            }
        }
        */

        Vector2Int offsetVector = new Vector2Int(-bounds.min.x, -bounds.min.y);
        int indent;
        // +ve => tile map to tile array
        // -ve => tile array to tile map
        Debug.LogFormat("vector Offset: {0}", offsetVector);
        gameTiles = new GameTile[bounds.size.x, bounds.size.y];
        Debug.LogFormat("Tilemap bounds: {0}", bounds);

        Debug.LogFormat("gameTiles shape: [{0}, {1}]", gameTiles.GetLength(0), gameTiles.GetLength(1));
        for (int x = 0; x < gameTiles.GetLength(0); x++)
        {
            for (int y = 0; y < gameTiles.GetLength(1); y++)
            {
                //Debug.LogFormat("Position: [{0}, {1}]", x, y);
                /*
                if (y%2 == minY%2)
                {
                    indent = -1;
                }
                else
                {
                    indent = 0;
                }
                */
                Vector3Int tilePos = new Vector3Int(x - offsetVector.x, y - offsetVector.y, 0);
                if (tilemap.HasTile(tilePos))
                {
                    //Debug.LogFormat("Tile at {0}", new Vector2Int(tilePos.x + offsetVector.x, tilePos.y + offsetVector.y));
                    GameObject overlayTileObj = Instantiate(overlayTilePrefab, overlayContainer.transform);
                    GameTile overlayTile = overlayTileObj.GetComponent<GameTile>();
                    var cellPosition = tilemap.GetCellCenterWorld(tilePos);
                    overlayTile.transform.position = new Vector3(cellPosition.x, cellPosition.y, TILE_Z);
                    //overlayTile.GetComponent<SpriteRenderer>().sortingOrder = tilemap.GetComponent<SpriteRenderer>().sortingOrder;
                    gameTiles[x, y] = overlayTile;
                    overlayTile.SetUp(new Vector2Int(x, y));
                }
                else
                {
                    gameTiles[x, y] = null;
                }
            }
        }

        //CreateNewUnit(new Vector2Int(Random.Range(0, Mathf.Abs(bounds.min.x) + bounds.max.x - 1), Random.Range(0, Mathf.Abs(bounds.min.y) + bounds.max.y - 1)));
        //CreateNewUnit(new Vector2Int(Random.Range(0, Mathf.Abs(bounds.min.x) + bounds.max.x - 1), Random.Range(0, Mathf.Abs(bounds.min.y) + bounds.max.y - 1)));
        //CreateNewUnit(new Vector2Int(Random.Range(0, Mathf.Abs(bounds.min.x) + bounds.max.x - 1), Random.Range(0, Mathf.Abs(bounds.min.y) + bounds.max.y - 1)));
    }


    public void CreateRandomUnit()
    {
        //Debug.LogAssertionFormat("gameTiles == null: {0}", gameTiles == null);
        GameTile tile = null;
        if (gameTiles == null)
        {
            Debug.LogErrorFormat("Gametiles null");
        }
        
        while (tile == null)
        {
            int randx = Random.Range(0, gameTiles.GetLength(0));
            int randy = Random.Range(0, gameTiles.GetLength(1));
            if (gameTiles[randx, randy] != null)
            {
                tile = gameTiles[randx, randy];
            }
            
        }
        Debug.LogFormat("tile: {0}", gameTiles[tile.GetMatrixCoords().x, tile.GetMatrixCoords().y]);

        //if def

        Debug.LogFormat("tile: {0}", tile.GetMatrixCoords());
        CreateNewUnit(tile.GetMatrixCoords());
    }

    public void StartTestEncounter()
    {
        EncounterManager.encounterManager.CreateEncounter(GameManager.gameManager.GetPlayerParty(), GameManager.gameManager.GetRandomParty());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator SelectFromUnits(List<UnitObject> unitList, bool target)
    {
        string text = "";
        UnhighlightTiles(highlightedTiles);
        if (highlightedTiles.Count > 0)
        {
            Debug.LogFormat("More than 0 tiles highlighted: {0}", highlightedTiles.Count);
        }
        List<GameTile> tiles = new List<GameTile>();
        foreach (UnitObject unit in unitList)
        {
            Vector2Int coords = unit.GetCoords();
            tiles.Add(gameTiles[coords.x, coords.y]);
        }
        if (target)
        {
            Debug.Log("searching for target");
            text = "target";
            foreach (GameTile tile in tiles)
            {
                tile.SetHighlightColour(Color.red);
            }
            HighlightTiles(tiles);
        }
        else
        {
            Debug.Log("searching for user");
            text = "user";
            foreach (GameTile tile in tiles)
            {
                tile.SetHighlightColour(Color.blue);
            }
            HighlightTiles(tiles);
        }
        unitSelectionInProgress = true;
        unitSelectionFailed = false;

        selectedTile = null;
        selectedUnit = null;
        yield return new WaitUntil(() => selectedTile != null);
        selectedUnit = selectedTile.GetUnit();
        if (!unitList.Contains(selectedUnit))
        {
            Debug.Log("Clicked off of unit");
            unitSelectionInProgress = false;
            unitSelectionFailed = true;
            Debug.Log("unit selection failed to find a unit");
            EncounterManager.encounterManager.cardFailed = true;
            unitsMovable = true;
            yield break;
        }
        else
        {
            unitSelectionInProgress = false;
            Debug.Log("unit selected");
        }
        Debug.LogFormat("selectedUnit : {0}", selectedUnit.name.ToString());
        if (target)
        {
            EncounterManager.encounterManager.cardTarget = selectedUnit;
        }
        else
        {
            EncounterManager.encounterManager.cardActivator = selectedUnit;
        }
        foreach (GameTile tile in tiles)
        {
            tile.SetHighlightColour(Color.grey);
        }
        yield break;
    }

}
