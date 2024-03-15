using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class EncounterManager : MonoBehaviour
{
    private GameTile[,] gameTiles; 
    public static EncounterManager encounterManager;
    [SerializeField] private Party playerParty;
    [SerializeField] private Party defenders;

    [Header("Cards")]
    [SerializeField] private DeckObject playersDeck;
    [SerializeField] private DeckObject defendersDeck;
    [SerializeField] private List<CardObject> playersDiscardDeck = new List<CardObject>();
    [SerializeField] private List<CardObject> defendersDiscardDeck = new List<CardObject>();
    [SerializeField] public CardObject[] playersHand;// = new CardObject[];
    [SerializeField] private CardObject[] defendersHand;// = new CardObject[];

    [SerializeField] private int[] defensiveCardValues = new int[Enum.GetNames(typeof(CardType)).Length];
    [SerializeField] private int[] aggressiveCardValues = new int[Enum.GetNames(typeof(CardType)).Length];

    private int[] baseCardValues = new int[Enum.GetNames(typeof(CardType)).Length];

    private GameTile[,] playerSpawnZone;
    private GameTile[,] defendersSpawnZone;


    [SerializeField] public bool setupPhase = true;
    private bool firstTurn = true;
    private bool endTurn = false;
    [SerializeField] private GameObject hand;
    private EnemyType enemyType;
    private int enemyActions = 4;
    public float aggressiveDistanceConstant = 1;
    public float defensiveDistanceConstant = 1;
    private const float LARGE_DISTANCE = 9999;

    public UnitObject cardActivator = null;
    public UnitObject cardTarget = null;
    public bool cardFailed = false;


    public bool playersTurn = true;
    private bool encounter = true;

    private void Start()
    {
        encounterManager = this;
        aggressiveDistanceConstant = GameManager.AGGRESSIVE_TILE_DISTANCE;
        defensiveDistanceConstant = GameManager.DEFENSIVE_TILE_DISTANCE;
    }

    /// <summary>
    /// Create an encounter between the player and an input enemy party
    /// </summary>
    /// <param name="player"></param>
    /// <param name="defenders"></param>
    /// <param name="enemyType"></param>
    public void CreateEncounter(Party player, Party defenders, EnemyType enemyType)
    {
        Debug.Log("========== STARTING ENCOUNTER ==========");
        MapManager.mapManager.SetBoardActive(true);
        playersHand = new CardObject[GameManager.gameManager.playerDrawSize];
        defendersHand = new CardObject[GameManager.gameManager.playerDrawSize];
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
        this.enemyType = enemyType;
        baseCardValues = GetBaseCardValues();

    }

    #region Deck Management
    /// <summary>
    /// Discard a card
    /// </summary>
    /// <param name="card"></param>
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

    /// <summary>
    /// Reshuffle the discard pile to the deck
    /// </summary>
    /// <param name="player"></param>
    private void DiscardToDeck(bool player)
    {
        if (player)
        {
            playersDeck.EnqueueFromDiscard(playersDiscardDeck);
            playersDiscardDeck.Clear();
             playersDeck.Shuffle();
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

    public void DrawCardCurrent(int amount)
    {
        Debug.LogFormat("Drawing ({0}) cards", amount);
        DeckObject deck;
        if (playersTurn)
        {
            deck = playersDeck;
        }
        else
        {
            deck = defendersDeck;
        }
        DrawCard(deck, amount);
    }

    public void DrawCard(DeckObject deck, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            DrawCard(deck);
        }
    }

    public CardObject DrawCard(DeckObject deck)
    {
        CardObject[] hand;
        int nextCard = -1;
        if (deck == playersDeck)
        {
            hand = playersHand;
        }
        else
        {
            hand = defendersHand;
        }
        for (int i = 0; i < hand.Length; i++)
        {
            if (hand[i] == null)
            {
                nextCard = i;
                break;
            }
        }
        if (nextCard == -1)
        {
            Debug.LogErrorFormat("All hand positions are full. No card can be drawn");
            return null;
        }
        CardObject card = deck.GetXCards(1)[0];
        hand[nextCard] = card;
        StartCoroutine(UIManager.uiManager.SetCardToPosition(card, nextCard));
        return card;
        //Set emptyCards to hand positions
        //UiManager.uiManager.SetCardToPosition() 

    }

    private void DrawStartingHand(bool player)
    {
        int handSize;
        DeckObject deck;
        if (player)
        {
            handSize = playersHand.Length;
            deck = playersDeck;
        }
        else
        {
            handSize = defendersHand.Length;
            deck = defendersDeck;
        }
        DrawCard(deck, handSize);
    }
    #endregion

    /// <summary>
    /// The start of an encounter. Controls the turns etc.
    /// </summary>
    /// <returns></returns>
    public IEnumerator StartEncounter()
    {
        setupPhase = false;
        MapManager.mapManager.HideSpawnZone(playerSpawnZone);
        MapManager.mapManager.HideSpawnZone(defendersSpawnZone);
        // Draw starting hand
        if (playersDeck == null)
        {
            Debug.LogErrorFormat("playersDeck == null");
            yield break;
        }
        Debug.LogFormat("Player's deck size: {0}", playersDeck.GetDeckSize());
        // FIX -> playersDeck.Shuffle();
        DrawStartingHand(true);
        DrawStartingHand(false);
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
                StartCoroutine(StartTurn(playersTurn));
                // AI turn
                yield return StartCoroutine(AITurn());
                StartCoroutine(EndTurn());

            }
            else
            {
                if (!firstTurn)
                {
                    DrawCard(playersDeck, 2);
                }
                StartCoroutine(StartTurn(playersTurn));
                Debug.LogAssertionFormat("Starting players turn");
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

    /// <summary>
    /// The end of the encounter.
    /// Any end of encounter events happen here
    /// </summary>
    public void EndEncounter()
    {
        Debug.LogAssertionFormat("End of encounter");

        //Player gets loot
        //Players units heal
        foreach(UnitObject unit in playerParty.GetAllUnits())
        {
            unit.health = unit.maxHealth;
        }
        hand.gameObject.SetActive(false);
    }

    /// <summary>
    /// The start of a turn. Any start of turn events happen here
    /// </summary>
    /// <param name="playerTurn"></param>
    /// <returns></returns>
    public IEnumerator StartTurn(bool playerTurn)
    {
        endTurn = false;
        Debug.Log("========== START OF TURN ==========");
        UIManager.uiManager.DisplayCurrentTurn(playerTurn);
        while (endTurn == false)
        {
            yield return null;
        }
        yield break;
    }

    /// <summary>
    /// The end of a turn
    /// Any end of turn events happen here
    /// </summary>
    /// <returns></returns>
    public IEnumerator EndTurn()
    {
        endTurn = true;
        playersTurn = !playersTurn;

        yield break;
    }


    //https://www.youtube.com/watch?v=kUP6OK36nrM&ab_channel=GameDevBeginner
    /// <summary>
    /// Play a card. A user and target will need to be specified
    /// </summary>
    /// <param name="cardIndex"></param>
    /// <returns></returns>
    public IEnumerator PlayCard(int cardIndex)
    {
        // yield return null  ->  suspends coroutine until next frame, can be used to make update loops - loops can persist over several frames
        // yield return new WaitForSeconds()
        // yield return new WaitForEndOfFrame() - screenshots
        // yield return new WaitUntil()
        // yield return new WaitWhile()

        // yield return StartCoroutine()  ->  Wait until the next coroutine is complete

        // yield break - exit out of the coroutine


        CardObject card = playersHand[cardIndex];

        Debug.LogFormat("Playing card {0}", card.name);

        MapManager.mapManager.unitsMovable = false;
        MapManager.mapManager.selectedUnit = null;
        DeckObject deck;
        if (playersTurn)
        {
            deck = playersDeck;
        }
        else
        {
            deck = defendersDeck;
        }
        cardFailed = false;
        List<UnitObject> activators;
        List<UnitObject> targets;
        activators = GetPossibleActivatorUnits(card, playersTurn);
        Debug.LogFormat("({0}) units can activate the card", activators.Count);
        if (activators.Count == 0)
        {
            Debug.LogErrorFormat("No Unit can be selected to play the selected card");
        }
        yield return StartCoroutine(MapManager.mapManager.SelectFromUnits(activators, false));
        Debug.Log("SelectFromUnits ended");
        if (cardFailed)
        {
            Debug.LogFormat("No unit was selected");
            yield break;
        }
        Debug.LogFormat("cardActivator: {0}", cardActivator.name.ToString());
        targets = GetPossibleTargetUnits(MapManager.mapManager.GetTilesInRange(gameTiles[cardActivator.GetCoords().x, cardActivator.GetCoords().y], card.range), card, playersTurn);
        Debug.LogFormat("({0}) units can be targeted by the card", targets.Count);
        if (targets.Count == 0)
        {
            Debug.LogFormat("No Unit can be selected as a target for the card");
        }
        yield return StartCoroutine(MapManager.mapManager.SelectFromUnits(targets, true));
        if (cardFailed)
        {
            Debug.LogFormat("No target was selected");
            yield break;
        }
        card.PlayCardEffect(cardActivator, cardTarget);
        UIManager.uiManager.RemoveCardFromHand(cardIndex);
        DiscardCard(card);
        // tile shading back to normal
        yield break;
    }

    /// <summary>
    /// Returns a list of all units that can activate the card
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
    /// Returns a list of all units that the card can target in the tiles given
    /// </summary>
    /// <param name="centerTile"></param>
    /// <param name="card"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    private List<UnitObject> GetPossibleTargetUnits(List<GameTile> tiles, CardObject card, bool player)
    {

        // get enemies within card range of centerTile
        if (tiles == null || card == null || tiles.Count == 0)
        {
            Debug.LogError("An input to possible targets is null or 0");
        }
        List<UnitObject> possibleTargets = new List<UnitObject>();
        Party targetParty;
        if (player && card.GetCardType() == CardType.Attack || !player && (card.GetCardType() == CardType.Utility || card.GetCardType() == CardType.Defence))
        {
            targetParty = defenders;
        }
        else
        {
            targetParty = playerParty;
        }
        foreach (GameTile tile in tiles)
        {
            if (tile.GetUnit() != null && targetParty.GetAllUnits().Contains(tile.GetUnit()))
            {
                possibleTargets.Add(tile.GetUnit());
            }
        }
        return possibleTargets;
    }

    #region Card Abilities
    /// <summary>
    /// Applied damage to a unit
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="damageAmount"></param>
    public void DamageUnit(UnitObject unit, int damageAmount)
    {
        Debug.LogFormat("Applying ({0}) damage to unit ({1})", damageAmount, unit.name);
        unit.health = unit.health - damageAmount;

        if (unit.shield > 0)
        {
            if (damageAmount > unit.shield)
            {
                damageAmount -= unit.shield;
                unit.shield = 0;
            }
            else if (damageAmount < unit.shield)
            {
                unit.shield -= damageAmount;
                damageAmount = 0;
            }
        }
        unit.health -= damageAmount;

        if (unit.health < 0)
        {
            Debug.LogFormat("({0}) was killed | player's party: {1}", unit.name, unit.playerUnit);
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

    /// <summary>
    /// Applied a shield value to a unit
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="shieldAmount"></param>
    public void ShieldUnit(UnitObject unit, int shieldAmount)
    {
        Debug.LogFormat("Applying {(0)} shield to unit {(1)}", shieldAmount, unit.name.ToString());
        unit.shield += shieldAmount;
    }

    /// <summary>
    /// Heals a units 
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="healAmount"></param>
    public void HealUnit(UnitObject unit, int healAmount)
    {
        Debug.LogFormat("Applying {(0)} healing to unit {(1)}", healAmount, unit.name.ToString());
        unit.health += healAmount;
        if (unit.health > unit.maxHealth)
        {
            unit.health = unit.maxHealth;
        }
    }

    #endregion

    /// <summary>
    /// Creates a graph of all possible card plays and unit moves (unit no more emptyCards can be played)
    /// </summary>
    /// <param name="previousMove"></param>
    /// <param name="cardIndex"></param>
    /// <param name="hand"></param>
    /// <param name="player"></param>
    /// <param name="actions"></param>
    /// <returns></returns>
    public void BuildPossibleMoveGraph(PossibleMove previousMove, int cardIndex, CardObject[] hand, bool player, int actions)
    {
        Debug.LogFormat("Actions left: {0} | Card playing: {1}", actions, hand[cardIndex].cardName);

        //Connect new move to previous move

        //Look at possible future moves


        CardObject card = hand[cardIndex];
        if (card.cost > actions)
        {
            return;
        }
        List<PossibleMove> possibleMoves = new List<PossibleMove>();

        //get possibleActivators that can be targeted with the card taking movement into account
        List<UnitObject> possibleActivators = GetPossibleActivatorUnits(card, player);
        Debug.LogAssertionFormat("{0} units can use the ({1}) card", possibleActivators.Count, card.cardName);
        PossibleMove newMove;
        int unitCount = 0;
        foreach(UnitObject unit in possibleActivators)
        {
            Debug.LogAssertionFormat("Checking unit {0}", unitCount);
            //List of all coordinates that the unit can move to
            List<Vector2Int> movesList = MapManager.mapManager.GetTilesInRange(unit.GetCoords(), unit.GetMovementRange());
            //List of all tiles that can be targetted
            List <Vector2Int> targetList = MapManager.mapManager.GetTilesInRange(unit.GetCoords(), unit.GetMovementRange() + card.range);
            List<GameTile> tileList = new List<GameTile>();
            foreach (Vector2Int vector in targetList)
            {
                tileList.Add(gameTiles[vector.x, vector.y]);
            }
            //Get targets from list of targettable tiles
            List<UnitObject> possibleTargets = GetPossibleTargetUnits(tileList, card, player);
            //Finds where the unit can move to to activate the card on the target
            foreach (UnitObject target in possibleTargets)
            {
                //If target is out of range of unit, do nothing
                if (!targetList.Contains(target.GetCoords()))
                {
                    break;
                }
                //List of tiles around enemy
                List<Vector2Int> targetRangeVector = MapManager.mapManager.GetTilesInRange(target.GetCoords(), card.range);
                List<Vector2Int> possibleMoveVector = new List<Vector2Int>();
                //Creates list of all tiles the unit can move to that allow it to use the card's affect on the target
                foreach (Vector2Int vector in targetRangeVector)
                {
                    if (movesList.Contains(vector))
                    {
                        possibleMoveVector.Add(vector);
                    }
                }
                //The coords and value for the best tile to move to
                float[] temp = GetBestCardUseTile(possibleMoveVector, gameTiles[unit.GetCoords().x, unit.GetCoords().y], gameTiles[target.GetCoords().x, target.GetCoords().y]);
                newMove = new PossibleMove(previousMove, unit, target, card, hand, gameTiles[(int)temp[0], (int)temp[1]], GetCardPriority(card) + temp[2]);
                Debug.LogFormat("Adding new move to move graph");
                newMove.DisplayMove();
                // possibleMoveVector is a list of moves against all possible targets. These moves have the best move locations for targetting that target
                possibleMoves.Add(newMove);
                previousMove.connectedMoves.Add(new MoveVertex(previousMove, newMove));
            }
                // best tile for that target
            //if enemy on tile, add possible moves
        }
        CardObject[] newHand = new CardObject[hand.Length];
        hand.CopyTo(newHand, 0);
        newHand[cardIndex] = null;
        for (int i = 0; i < newHand.Length; i++)
        {
            if (newHand[i] != null)
            {
                // Create graph verticies for each terget the ability was used on
                foreach (PossibleMove move in possibleMoves)
                {
                    BuildPossibleMoveGraph(move, i, newHand, player, actions - card.cost);
                }
            }
        }
    }

    /// <summary>
    /// Get the best tile to move to to use the card (returns float[xCoord, yCoord, priority value])
    /// </summary>
    /// <param name="possibleTileVector"></param>
    /// <param name="unitTile"></param>
    /// <param name="targetTile"></param>
    /// <returns></returns>
    private float[] GetBestCardUseTile(List<Vector2Int> possibleTileVector, GameTile unitTile, GameTile targetTile)
    {
        PriorityQueue<Vector2Int, float> possibleTiles = new PriorityQueue<Vector2Int, float>();
        float priority = 0;
        foreach (Vector2Int vector in possibleTileVector)
        {
            float targetDistance = MapManager.mapManager.CubicDistance(MapManager.mapManager.MatrixToCubic(unitTile.GetMatrixCoords()), MapManager.mapManager.MatrixToCubic(targetTile.GetMatrixCoords()));
            //Disatnce to closest friendly
            float closestUnit = LARGE_DISTANCE;
            foreach (UnitObject unit in defenders.GetAllUnits())
            {
                if (unit != unitTile.GetUnit())
                {
                    closestUnit = MathF.Min(MapManager.mapManager.CubicDistance(MapManager.mapManager.MatrixToCubic(unitTile.GetMatrixCoords()), MapManager.mapManager.MatrixToCubic(unit.GetCoords())), closestUnit);
                }
            }
            switch (enemyType)
            {
                case EnemyType.Agressive:
                    // minimise distance between unit and target and other friendly possibleActivators
                    //Distance to target
                    priority = (1 + GameManager.AGGRESSIVE_TILE_DISTANCE / closestUnit) + (1 + GameManager.AGGRESSIVE_TILE_DISTANCE / targetDistance);
                    break;
                case EnemyType.Defensive:
                    // minimise distance to other friendly possibleActivators while maximising distance to target
                    priority = (1 + GameManager.DEFENSIVE_TILE_DISTANCE / closestUnit) + (1 - GameManager.DEFENSIVE_TILE_DISTANCE / targetDistance);
                    break;
            }
            possibleTiles.Enqueue(vector, priority);
        }
        Vector2Int tileVector = possibleTiles.Dequeue();
        return new float[] { tileVector.x, tileVector.y, priority };
    }

    /// <summary>
    /// Get the priority (how much the AI wants to play the card) of the input card
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    private float GetCardPriority(CardObject card)
    {
        // base card priority + amount of that affect (e.g. damageAmount)
        float output = baseCardValues[(int)card.GetCardType()];
        float cardEffect = 0;
        switch (card.GetCardType())
        {
            case CardType.Attack:
                cardEffect = card.attackDamage;
                break;
            case CardType.Utility:
                cardEffect = card.healingAmount;
                break;
            case CardType.Defence:
                cardEffect = card.shieldAmount;
                break;
            case CardType.DrawCard:
                int emptyCards = 0;
                foreach (CardObject playableCard in defendersHand)
                {
                    if (playableCard != null)
                    {
                        emptyCards++;
                    }
                }
                cardEffect = 1.25f * emptyCards;
                break;
        }
        output += 0.5f * cardEffect;
        return output;
    }

    /// <summary>
    /// Sets how important the AI thinks different emptyCards are. Changes with different personalities
    /// </summary>
    /// <returns></returns>
    private int[] GetBaseCardValues()
    {
        // switch statement allows expansion
        switch (enemyType)
        {
            case EnemyType.Agressive:
                return aggressiveCardValues;
            case EnemyType.Defensive:
                return defensiveCardValues;
        }
        return null;
    }

    private IEnumerator AITurn()
    {
        Debug.LogAssertionFormat("Starting AI turn");
        // Get values for all emptyCards in hand
        // Order emptyCards (ordered list?, Priority queue?)
        // 
        // Get possible possibleActivators for each card
        // order emptyCards based on distances to player and AI possibleActivators

        // Reorder after each card played (unit movement may mean some emptyCards cannot be played)
        /*
        PriorityQueue<CardObject, float> baseQueue = new PriorityQueue<CardObject, float>();
        foreach (CardObject card in defendersHand)
        {
            baseQueue.Enqueue(card, baseCardValues[((int)card.GetCardType())]);
        }
        */
        PossibleMove rootNode = new PossibleMove(null, null, null, null, defendersHand, null, 0);
        rootNode.rootNode = true;
        CardObject[] tempHand = new CardObject[defendersHand.Length];
        defendersHand.CopyTo(tempHand, 0);
        Debug.LogFormat("Building possible moves graph");
        for (int i = 0; i < defendersHand.Length; i++)
        {
            BuildPossibleMoveGraph(rootNode, i, rootNode.hand, false, enemyActions);
        }
        Debug.LogAssertionFormat("Nodes connected to root: {0}", rootNode.connectedMoves.Count);
        Debug.LogFormat("Searching possible moves graph");
        Queue<PossibleMove> possibleMoves = ExpandMove(rootNode);
        PossibleMove move;

        Debug.LogFormat("Moving");
        while (possibleMoves.Count > 0)
        {
            move = possibleMoves.Dequeue();
            move.DisplayMove();
            move.PlayMove();
            yield return new WaitForSecondsRealtime(0.5f);
        }
        Debug.LogAssertionFormat("Ending AI turn");
    }

    /// <summary>
    /// Fully expands a node of possible moves
    /// </summary>
    /// <param name="move"></param>
    /// <returns></returns>
    private Queue<PossibleMove> ExpandMove(PossibleMove move)
    {
        List<PossibleMove> fringe = new List<PossibleMove>();
        Queue<PossibleMove> moveQueue = new Queue<PossibleMove>();
        moveQueue.Enqueue(move);
        if (move.connectedMoves.Count == 0)
        {
            Debug.LogAssertion("No more nodes to expand");
        }
        foreach(MoveVertex vertex in move.connectedMoves)   
        {
            if (vertex != null)
            {
                fringe.Add(vertex.GetNextMove());
            }
        }
        Queue<PossibleMove> tempQueue;
        Queue<PossibleMove> bestQueue = new Queue<PossibleMove>();
        float bestValue = 0;
        float tempValue;
        foreach(PossibleMove possible in fringe)
        {
            tempQueue = ExpandMove(possible);
            tempValue = GetMovePriority(tempQueue);
            if (tempValue > bestValue)
            {
                bestValue = tempValue;
                bestQueue = tempQueue;
            }
        }
        while (bestQueue.Count > 0)
        {
            moveQueue.Enqueue(bestQueue.Dequeue());
        }
        return moveQueue;
    }

    /// <summary>
    /// Gets the priority values for all moves in a movement queue
    /// </summary>
    /// <param name="moveQueue"></param>
    /// <returns></returns>
    private float GetMovePriority(Queue<PossibleMove> moveQueue)
    {
        float value = 0;
        int moveQueueCount = moveQueue.Count;
        for (int i = 0; i < moveQueueCount; i++)
        {
            PossibleMove move = moveQueue.Dequeue();
            value += move.priority;
            moveQueue.Enqueue(move);
        }
        return value;
    }
}


public class PossibleMove
{
    public bool rootNode = false;
    public UnitObject unit;
    public UnitObject target;
    public CardObject card;
    //The hand after the move
    public CardObject[] hand;
    public GameTile destinationTile;

    public float priority;

    public MoveVertex previousMoveVertex;
    public List<MoveVertex> connectedMoves = new List<MoveVertex>();

    public PossibleMove(PossibleMove previousMove, UnitObject unit, UnitObject target, CardObject card, CardObject[] hand, GameTile destinationTile, float value)
    {
        this.unit = unit;
        this.target = target;
        this.card = card;
        this.hand = hand;
        this.destinationTile = destinationTile;
        if (previousMove != null)
        {
            this.previousMoveVertex = new MoveVertex(previousMove, this);
        }
        this.priority = value;
    }

    public void PlayMove()
    {
        if (rootNode)
        {
            Debug.Log("Root node reached");
            return;
        }
        if (unit != null || destinationTile != null || card != null)
        {
            //Move unit to tile
            MapManager.mapManager.MoveUnit(MapManager.mapManager.GetGameTiles()[unit.GetCoords().x, unit.GetCoords().y], destinationTile);

            //Play card
            card.PlayCardEffect(unit, target);

            //Discard the card
            EncounterManager.encounterManager.DiscardCard(card);
        } 
        else
        {
            Debug.Log("Cannot playt move. No unit, target or card");
        }
    }


    public void DisplayMove()
    {
        if (rootNode)
        {
            Debug.Log("Root node reached");
            return;
        }

        if (this.hand != null)
        {
            string hand = "[";
            for (int i = 0; i < this.hand.Length; i++)
            {
                if (this.hand[i] != null)
                {
                    hand += this.hand[i].cardName;
                }
                else
                {
                    hand += "null";
                }
                if (i != this.hand.Length - 1)
                {
                    hand += ", ";
                }
            }
            hand += "]";
        }
        if (card == null)
        {
            Debug.Log("Cannot display move, card is null");
        }
        else if (unit == null)
        {
            Debug.Log("Cannot display move, card is null");
        }
        else if (target == null)
        {
            Debug.Log("Cannot display move, card is null");
        }
        else
        {
            Debug.LogFormat("Displaying possible move\nCard: {0} || Hand after card: {1}\nUnit: {2} || Target: {3}", card.name.ToString(), hand, unit.name, target.name);
        }
    }
}

public class MoveVertex
{
    public PossibleMove previousMove;
    public PossibleMove newMove;

    public MoveVertex(PossibleMove previousMove, PossibleMove newMove)
    {
        this.previousMove = previousMove;
        this.newMove = newMove;
        newMove.previousMoveVertex = this;
        previousMove.connectedMoves.Add(this);
    }

    public PossibleMove GetNextMove()
    {
        return newMove;
    }

    public PossibleMove GetMove(PossibleMove currentMove)
    {
        if (currentMove == previousMove)
        {
            Debug.Log("Getting previous move");
            return newMove;
        }
        else if (currentMove == newMove)
        {
            Debug.Log("Getting the next move");
            return previousMove;
        }
        else
        {
            Debug.Log("Move could not be found");
            return null;
        }
    }
}