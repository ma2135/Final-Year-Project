using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    private GameTile[,] gameTiles; 
    public static EncounterManager encounterManager;
    [SerializeField] private Party playerParty;
    [SerializeField] private Party defenders;

    [SerializeField] private DeckObject playersDeck;
    [SerializeField] private DeckObject defendersDeck;
    [SerializeField] private List<CardObject> playersDiscardDeck = new List<CardObject>();
    [SerializeField] private List<CardObject> defendersDiscardDeck = new List<CardObject>();
    [SerializeField] private CardObject[] playersHand;// = new CardObject[];
    [SerializeField] private CardObject[] defendersHand;// = new CardObject[];

    private GameTile[,] playerSpawnZone;
    private GameTile[,] defendersSpawnZone;


    [SerializeField] public bool setupPhase = true;
    private bool firstTurn = true;
    private bool endTurn = false;
    [SerializeField] private GameObject hand;
    private GameState currentGameState;

    public UnitObject cardActivator = null;
    public UnitObject cardTarget = null;
    public bool cardFailed = false;


    public bool playersTurn = true;
    private bool encounter = true;

    private void Start()
    {
        encounterManager = this;
        playersHand = new CardObject[UIManager.uiManager.maxHandSize];
        defendersHand = new CardObject[UIManager.uiManager.maxHandSize];
    }

    public void CreateEncounter(Party player, Party defenders)
    {
        Debug.Log("========== STARTING ENCOUNTER ==========");
        //spawn player on the left
        //spawn defenders on the right
        gameTiles = MapManager.mapManager.GetGameTiles();
        this.playerParty = player;
        this.defenders = defenders;
        playersDeck = player.GetDeck();
        defendersDeck = defenders.GetDeck();

        foreach(UnitObject unit in playerParty.GetAllUnits())
        {
            unit.playerUnit = true;
        }

        // Updates the Decks for the parties
        // playerParty.UpdateParty();
        // defenders.UpdateParty();
        Debug.LogFormat("PlayerDeck.Count = {0}", defendersDeck.GetDeckSize());
        Debug.LogFormat("DefendersDeck.Count = {0}", defendersDeck.GetDeckSize());

        //Place board
        int spawnWidth = Mathf.RoundToInt(gameTiles.GetLength(0) / 4);
        int spawnHeight = gameTiles.GetLength(1);
        int defendersIndent = gameTiles.GetLength(0) - spawnWidth - 1;
        GameTile[,] attackersSpawn = new GameTile[spawnWidth, spawnHeight];
        GameTile[,] defendersSpawn = new GameTile[spawnWidth, spawnHeight];

        Debug.LogFormat("spawn width : {0}", spawnWidth);
        for (int i = 0; i < spawnWidth; i++)
        {
            for (int j = 0; j < spawnHeight; j++)
            {
                attackersSpawn[i, j] = gameTiles[i, j];
                defendersSpawn[i, j] = gameTiles[i + defendersIndent, j];
            }
        }
        /*
        foreach (GameTile tile in attackersSpawn)
        {
            if (tile != null)
            {
                tile.GetComponent<SpriteRenderer>().color = Color.blue.WithAlpha(0.2f);
            }
        }
        foreach (GameTile tile in defendersSpawn)
        {
            if (tile != null)
            {
                tile.GetComponent<SpriteRenderer>().color = Color.red.WithAlpha(0.2f);
            }
        }
        */
        playerSpawnZone = attackersSpawn;
        defendersSpawnZone = defendersSpawn;
        MapManager.mapManager.SetTilesAsSpawn(attackersSpawn, Color.blue);
        MapManager.mapManager.SetTilesAsSpawn(defendersSpawn, Color.red);
        MapManager.mapManager.SetPlayerSpawnWidth(spawnWidth);

        int unitDeployOrigin = Mathf.RoundToInt(spawnHeight / 2);
        int yStep = 0;
        int xStep = 0;
        bool firstPlaced = true;

        List<Vector2Int> positions = new List<Vector2Int>();
        List<UnitObject> units = player.GetAllUnits();
        //Debug.LogAssertionFormat("unit length: {0}", board.Count);
        for (int i = 0; i < units.Count; i++)
        {
            //Debug.LogAssertionFormat("1 - x = {0}", x);
            positions.Add(new Vector2Int(attackersSpawn.GetLength(0) - 1 - xStep, unitDeployOrigin + yStep));
            //Debug.LogAssertionFormat("2 - Adding position");
            if (!firstPlaced && i <= units.Count)
            {
                positions.Add(new Vector2Int(attackersSpawn.GetLength(0) - 1 - xStep, unitDeployOrigin - yStep));
                //Debug.LogAssertionFormat("3 - Adding position");
                i++;
                //Debug.LogAssertionFormat("4 - Adding to x = {0}", x);
            }
            yStep++;
            firstPlaced = false;
            if (unitDeployOrigin - yStep <= 0)
            {
                yStep = 0;
                xStep++;
                firstPlaced = true;
            }
        }
        if (positions.Count != units.Count)
        {
            Debug.LogErrorFormat("Position.Count ({0}) != Units.Count ({1})", positions.Count, units.Count);
        } 
        if (positions.Count >= units.Count)
        {
            for (int i = 0; i < units.Count; i++)
            {
                Debug.LogFormat("Placing Units[{0}] = ({2}) at Position[{0}] = {1}", i, positions[i].ToString(), units[i].name);
                MapManager.mapManager.CreateUnit(positions[i], units[i]);
            }
        }

        yStep = 0;
        xStep = 0;
        firstPlaced = true;
        positions.Clear();
        units = defenders.GetAllUnits();
        for (int i = 0; i < units.Count; i++)
        {
            positions.Add(new Vector2Int(defendersIndent + xStep, unitDeployOrigin + yStep));
            if (!firstPlaced && i <= units.Count)
            {
                positions.Add(new Vector2Int(defendersIndent + xStep, unitDeployOrigin - yStep));
                i++;
            }
            yStep++;
            firstPlaced = false;
            if (unitDeployOrigin - yStep <= 0)
            {
                yStep = 0;
                xStep++;
                firstPlaced = true;
            }
        }
        if (positions.Count != units.Count)
        {
            Debug.LogErrorFormat("Position.Count ({0}) != Units.Count ({1})", positions.Count, units.Count);
        }
        else
        {
            for (int i = 0; i < units.Count; i++)
            {
                Debug.LogFormat("Placing Units[{0}] = ({2}) at Position[{0}] = {1}", i, positions[i].ToString(), units[i].name);
                MapManager.mapManager.CreateUnit(positions[i], units[i]);
            }
        }

        if (hand != null)
        {
            hand.gameObject.SetActive(true);
        }

    }

    private void DrawCard(DeckObject deck, int drawAmount)
    {
        for (int i = 0; i <= drawAmount; i++)
        {
            if (deck.GetDeckSize() == 0)
            {
                DiscardToDeck(playersTurn);
                if (deck.GetDeckSize() == 0)
                {
                    Debug.LogErrorFormat("Did not add discard pile to deck");
                }
            }

            StartCoroutine(UIManager.uiManager.DrawCard(deck));
        }
    }

    public void DiscardCard(CardObject card)
    {

        if (playersTurn)
        {
            playersDiscardDeck.Add(card);
        }
        else
        {
            defendersDiscardDeck.Add(card);
        }
    }

    private void DiscardToDeck(bool player)
    {
        if (player)
        {
            playersDeck.EnqueueFromDiscard(playersDiscardDeck);
            playersDiscardDeck.Clear();
            // playersDeck.Shuffle();
            // shuffle discard deck before adding back to hand queue
        }
        else
        {
            defendersDeck.EnqueueFromDiscard(defendersDiscardDeck);
            defendersDiscardDeck.Clear();
            // defendersDeck.Shuffle();
            // shuffle discard deck before adding back to hand queue
        }
    }


    public IEnumerator StartEncounter()
    {
        setupPhase = false;
        MapManager.mapManager.HideSpawnZone(playerSpawnZone);
        MapManager.mapManager.HideSpawnZone(defendersSpawnZone);
        // Draw starting hand
        if (playersDeck == null)
        {
            Debug.LogErrorFormat("playersDeck == null");
        }
        Debug.LogFormat("Player's deck size: {0}", playersDeck.GetDeckSize());
        // FIX -> playersDeck.Shuffle();
        StartCoroutine(UIManager.uiManager.DrawStartingHand(playersDeck, GameManager.gameManager.playerDrawSize));
        //MapManager.mapManager.UnhighlightTiles();
        // Play turns

        int count = 0;

        while (encounter)
        {
            if (!playersTurn)
            {
                if (!firstTurn)
                {
                    DrawCard(defendersDeck, 2);
                }
                Debug.LogAssertionFormat("Starting AI turn");
                StartCoroutine(StartTurn(playersTurn));
                // AI turn

                StartCoroutine(EndTurn());

            }
            else
            {
                if (!firstTurn)
                {
                    DrawCard(playersDeck, 2);
                }
                Debug.LogAssertionFormat("Starting players turn");
                StartCoroutine(StartTurn(playersTurn));
                while (playersTurn)
                {
                    yield return null;
                }
            }
            firstTurn = false;
            count++;
            if (count == 3)
            {
                encounter = false;
            }
        }
        EndEncounter();

    }

    public void EndEncounter()
    {
        Debug.LogAssertionFormat("End of encounter");
        hand.gameObject.SetActive(false);
    }

    public IEnumerator StartTurn(bool playerTurn)
    {
        // Any start of turn events happen here
        endTurn = false;
        Debug.Log("========== START OF TURN ==========");
        UIManager.uiManager.DisplayCurrentTurn(playersTurn);
        while (endTurn == false)
        {
            yield return null;
        }
        yield break;
    }

    public IEnumerator EndTurn()
    {
        // Any end of turn events happen here
        endTurn = true;
        playersTurn = !playersTurn;

        yield break;
    }


    //https://www.youtube.com/watch?v=kUP6OK36nrM&ab_channel=GameDevBeginner
    public IEnumerator PlayCard(int cardIndex)
    {
        // yield return null  ->  suspends coroutine until next frame, can be used to make update loops - loops can persist over several frames
        // yield return new WaitForSeconds()
        // yield return new WaitForEndOfFrame() - screenshots
        // yield return new WaitUntil()
        // yield return new WaitWhile()

        // yield return StartCoroutine()  ->  Wait until the next coroutine is complete

        // yield break - exit out of the coroutine

        Debug.LogAssertionFormat("card Index ({0}) selected, card: {1}", cardIndex, playersHand[cardIndex].name);

        CardObject card = playersHand[cardIndex];

        cardFailed = false;
        List<UnitObject> activators;
        List<UnitObject> targets;
        if (card.GetAbility() == CardAbility.DrawCard)
        {
            if (playersTurn)
            {
                UIManager.uiManager.DrawCard(playersDeck);
            }
            else
            {
                UIManager.uiManager.DrawCard(defenders.GetDeck());
            }
        }
        else 
        {
            activators = GetPossibleActivatorUnits(card, playersTurn);
            Debug.LogFormat("({0}) units can activate the card", activators.Count);
            if (activators.Count == 0)
            {
                Debug.LogErrorFormat("No Unit can be selected to play the selected card");
            }
            yield return StartCoroutine(MapManager.mapManager.SelectFromUnits(activators, false));
            if (cardFailed)
            {
                Debug.LogFormat("No activator unit was selected");
                yield break;
            }

            targets = GetPossibleTargetUnits(cardActivator.GetCoords(), card, playersTurn);
            Debug.LogFormat("({0}) units can be targeted by the card", targets.Count);
            if (targets.Count == 0)
            {
                Debug.LogErrorFormat("No Unit can be selected as a target for the card");
            }
            yield return StartCoroutine(MapManager.mapManager.SelectFromUnits(targets, true));
            if (cardFailed)
            {
                Debug.LogFormat("No target unit was selected");
                yield break;
            }

        }

        // call play card with these board

        card.PlayCardEffect(cardActivator, cardTarget);

        UIManager.uiManager.RemoveCardFromHand(cardIndex);
        DiscardCard(card);
        // remove card from hand
    
        yield break;
    }

    /// <summary>
    /// Returns a list of all board that can activate the card used
    /// </summary>
    /// <param name="card"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    public List<UnitObject> GetPossibleActivatorUnits(CardObject card, bool player)
    {
        List<UnitObject> possibleUnits = new List<UnitObject>();
        if (player)
        {
            foreach (UnitObject unit in playerParty.GetAllUnits())
            {
                EquipmentObject[] equipment = unit.GetEquipment();
                for (int i = 0; i < equipment.Length; i++)
                {
                    if (equipment[i] != null && equipment[i].GetCards().Contains(card))
                    {
                        possibleUnits.Add(unit);
                    }
                }
            }
        }
        else
        {
            foreach (UnitObject unit in defenders.GetAllUnits())
            {
                EquipmentObject[] equipment = unit.GetEquipment();
                for (int i = 0; i < equipment.Length; i++)
                {
                    if (equipment[i] != null && equipment[i].GetCards().Contains(card))
                    {
                        possibleUnits.Add(unit);
                    }
                }
            }
        }
        return possibleUnits;
    }

    /// <summary>
    /// Returns a list of all board that the card can target
    /// </summary>
    /// <param name="centerTile"></param>
    /// <param name="card"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    public List<UnitObject> GetPossibleTargetUnits(Vector2Int centerTile, CardObject card, bool player)
    {
        List<UnitObject> possibleTargets = new List<UnitObject>();
        Party targetParty = null;
        if (card.GetAbility() != CardAbility.DrawCard)
        {
            if (player)
            {
                targetParty = defenders;
            }
            else
            {
                targetParty = playerParty;
            }
            foreach (UnitObject unit in targetParty.GetAllUnits())
            {
                if (Mathf.RoundToInt(MapManager.mapManager.CubicDistance(MapManager.mapManager.MatrixToCubic(centerTile), MapManager.mapManager.MatrixToCubic(unit.GetCoords()))) <= card.range)
                {
                    possibleTargets.Add(unit);
                }
            }
        }

        return possibleTargets;
    }

    public void DamageUnit(UnitObject unit, int damage)
    {
        unit.health = unit.health - damage;

        if (unit.shield > 0)
        {
            if (damage > unit.shield)
            {
                damage -= unit.shield;
                unit.shield = 0;
            }
            else if (damage < unit.shield)
            {
                unit.shield -= damage;
                damage = 0;
            }
        }
        unit.health -= damage;

        if (unit.health < 0)
        {
            if (unit.playerUnit)
            {
                playerParty.KillUnit(unit);
            }
            else
            {
                defenders.KillUnit(unit);
            }
        }
        // Change current game state int[,]s ro reflect the change in unit health
    }

    public void ShieldUnit(UnitObject unit, int shield)
    {
        unit.shield += shield;
    }

    public void HealUnit(UnitObject unit, int healAmount)
    {
        unit.health += healAmount;
        if (unit.health > unit.maxHealth)
        {
            unit.health = unit.maxHealth;
        }
    }


    private void UpdateGameState()
    {
        int[,] unitArray = new int[gameTiles.GetLength(0), gameTiles.GetLength(1)];
        int[,] movementArray = new int[gameTiles.GetLength(0), gameTiles.GetLength(1)];
        List<Vector2Int> unitPositions = new List<Vector2Int>();
        foreach (UnitObject unit in playerParty.GetAllUnits())
        {
            Vector2Int unitTile = unit.GetCoords();
            unitArray[unitTile.x, unitTile.y] = -unit.health;
            movementArray[unitTile.x, unitTile.y] = unit.movement;
            unitPositions.Add(unit.GetCoords());
        }
        foreach (UnitObject unit in defenders.GetAllUnits())
        {
            Vector2Int unitTile = unit.GetCoords();
            unitArray[unitTile.x, unitTile.y] = unit.health;
            movementArray[unitTile.x, unitTile.y] = unit.movement;
            unitPositions.Add(unit.GetCoords());
        }

        GameState initialState = new GameState(unitArray, movementArray, unitPositions);

        initialState.GetAvailableGameStates(playersTurn);
        currentGameState = initialState;
    }

    private void AITurn(EnemyType enemy)
    {
        // Requires:
        //  - board positions
        //  - hand
        //  - possible moves
        //      PathToTile().count <= unit movement movement


        // +ve = AI, -ve = player
        int[,] unitArray = new int[gameTiles.GetLength(0), gameTiles.GetLength(1)];
        int[,] movementArray = new int[gameTiles.GetLength(0), gameTiles.GetLength(1)];
        List<Vector2Int> unitPositions = new List<Vector2Int>();
        foreach (UnitObject unit in playerParty.GetAllUnits())
        {
            Vector2Int unitTile = unit.GetCoords();
            unitArray[unitTile.x, unitTile.y] = -unit.health;
            movementArray[unitTile.x, unitTile.y] = unit.movement;
            unitPositions.Add(unit.GetCoords());
        }
        foreach (UnitObject unit in defenders.GetAllUnits())
        {
            Vector2Int unitTile = unit.GetCoords();
            unitArray[unitTile.x, unitTile.y] = unit.health;
            movementArray[unitTile.x, unitTile.y] = unit.movement;
            unitPositions.Add(unit.GetCoords());
        }

        GameState initialState = new GameState(unitArray, movementArray, unitPositions);

        initialState.GetAvailableGameStates(playersTurn);
        List<GameStateAndScore> scoresList = new List<GameStateAndScore>();
        foreach (GameState state in initialState.availableGameStates)
        {
            scoresList.Add(new GameStateAndScore(ScoreGameState(enemy), state));
        }
        initialState.ScoreStates(scoresList);

    }


    private void Minimax(int depth, int player, int alpha, int beta)
    {
        int bestScore = 0;



    }


    public int ScoreGameState(EnemyType enemyType)
    {
        int score = 0;
        if (enemyType == EnemyType.Agressive)
        {
            foreach (UnitObject unit in playerParty.GetAllUnits())
            {
                score = score - unit.health;
            }
        }
        if (enemyType == EnemyType.Defensive)
        {
            foreach (UnitObject unit in defenders.GetAllUnits() )
            {
                score = score + unit.health;
            }
        }
        return score;
    }

}


public class GameStateAndScore
{
    public int score;
    public GameState state;
    public GameStateAndScore(int score, GameState state)
    {
        this.score = score;
        this.state = state;
    }
}

public class GameState
{
    public int[,] board;
    public List<Vector2Int> unitPositions;
    public int[,] movement;
    public CardObject[] hand;
    public List<GameState> availableGameStates = new List<GameState>();
    public List<GameStateAndScore> scoredGameStates = new List<GameStateAndScore>();

    MapManager mapManager;

    public GameState(int[,] board, int[,] movement, List<Vector2Int> unitPositions)
    {
        this.board = board;
        this.movement = movement;
        this.unitPositions = unitPositions;
        mapManager = MapManager.mapManager;
    }

    public void ScoreStates(List<GameStateAndScore> scores)
    {
        scoredGameStates = scores;
    }

    public bool UniqueBoardCheck(int[,] board1, int[,] board2)
    {
        for (int x = 0; x < board1.Length; x++)
        {
            for (int y = 0; y < board1.Length; y++)
            {
                if (board1[x, y] != board2[x, y])
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void GetAvailableGameStates(bool player)
    {
        foreach (Vector2Int unitPos in unitPositions)
        {
            if ((player && board[unitPos.x, unitPos.y] < 0) || (!player && board[unitPos.x, unitPos.y] > 0))
            {
                if (movement[unitPos.x, unitPos.y] > 0)
                {
                    // Adds new 
                    List<GameTile> tiles = mapManager.GetTilesInRange(mapManager.GetGameTiles()[unitPos.x, unitPos.y], movement[unitPos.x, unitPos.y]);
                    foreach (GameTile tile in tiles)
                    {
                        availableGameStates.Add(MoveUnit(board, movement, unitPos, tile.GetMatrixCoords()));
                    }
                }
            }
        }
    }

    private GameState MoveUnit(int[,] array, int[,] movement, Vector2Int startCell, Vector2Int endCell)
    {
        int[,] newArray = new int[array.GetLength(0), array.GetLength(1)];
        array.CopyTo(newArray, 0);
        int[,] newMovement = new int[movement.GetLength(0), movement.GetLength(1)];
        array.CopyTo(newMovement, 0);
        // Move array and movement values from cell to cell
        newArray[endCell.x, endCell.y] = newArray[startCell.x, startCell.y];
        newArray[startCell.x, startCell.y] = 0;
        newMovement[endCell.x, endCell.y] = newMovement[startCell.x, startCell.y] - Mathf.RoundToInt(mapManager.CubicDistance(mapManager.MatrixToCubic(startCell), mapManager.MatrixToCubic(endCell)));
        newMovement[startCell.x, startCell.y] = 0;
        if (newMovement[endCell.x, endCell.y] < 0)
        {
            Debug.LogErrorFormat("Movement cell in new GameState < 0 ({0})", newMovement[endCell.x, endCell.y]);
        }
        List<Vector2Int> unitList = new List<Vector2Int>();
        foreach (Vector2Int pos in unitPositions)
        {
            if (pos == startCell)
            {
                unitList.Add(endCell);
            }
            else
            {
                unitList.Add(pos);
            }
        }
        return new GameState(newArray, newMovement, unitList);
    }

}