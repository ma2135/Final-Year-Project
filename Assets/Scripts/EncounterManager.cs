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


    [SerializeField] private bool setupPhase = true;
    private bool firstTurn = true;
    [SerializeField] private GameObject hand;

    public UnitObject cardActivator = null;
    public UnitObject cardTarget = null;
    public bool cardFailed = false;


    public bool playersTurn = true;
    private bool encounter = true;

    private void Start()
    {
        encounterManager = this;
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

        // Updates the Decks for the parties
        // playerParty.UpdateParty();
        // defenders.UpdateParty();
        Debug.LogFormat("PlayerDeck.Count = {0}", defendersDeck.GetDeckSize());
        Debug.LogFormat("DefendersDeck.Count = {0}", defendersDeck.GetDeckSize());

        //Place units
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

        int unitDeployOrigin = Mathf.RoundToInt(spawnHeight / 2);
        int yStep = 0;
        int xStep = 0;
        bool firstPlaced = true;

        List<Vector2Int> positions = new List<Vector2Int>();
        List<UnitObject> units = player.GetAllUnits();
        //Debug.LogAssertionFormat("unit length: {0}", units.Count);
        for (int i = 0; i < units.Count; i++)
        {
            //Debug.LogAssertionFormat("1 - i = {0}", i);
            positions.Add(new Vector2Int(attackersSpawn.GetLength(0) - 1 - xStep, unitDeployOrigin + yStep));
            //Debug.LogAssertionFormat("2 - Adding position");
            if (!firstPlaced && i <= units.Count)
            {
                positions.Add(new Vector2Int(attackersSpawn.GetLength(0) - 1 - xStep, unitDeployOrigin - yStep));
                //Debug.LogAssertionFormat("3 - Adding position");
                i++;
                //Debug.LogAssertionFormat("4 - Adding to i = {0}", i);
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
        while (encounter)
        {
            StartCoroutine(StartTurn(playersTurn));
            if (!playersTurn)
            {
                if (!firstTurn)
                {
                    DrawCard(defendersDeck, 2);
                }
                // AI turn
            }
            else
            {
                if (!firstTurn)
                {
                    DrawCard(playersDeck, 2);
                }
                while (playersTurn)
                {
                    yield return null;
                }
            }
            firstTurn = false;

            encounter = false;
            EndEncounter();
        }

    }

    public void EndEncounter()
    {
        hand.gameObject.SetActive(false);
    }

    public IEnumerator StartTurn(bool playerTurn)
    {
        // Any start of turn events happen here
        Debug.Log("========== START OF TURN ==========");
        UIManager.uiManager.DisplayCurrentTurn(playersTurn);
        yield break;
    }

    public IEnumerator EndTurn()
    {
        // Any end of turn events happen here
        playersTurn = !playersTurn;

        yield break;
    }


    //https://www.youtube.com/watch?v=kUP6OK36nrM&ab_channel=GameDevBeginner
    public IEnumerator PlayCard(CardObject card)
    {
        // yield return null  ->  suspends coroutine until next frame, can be used to make update loops - loops can persist over several frames
        // yield return new WaitForSeconds()
        // yield return new WaitForEndOfFrame() - screenshots
        // yield return new WaitUntil()
        // yield return new WaitWhile()

        // yield return StartCoroutine()  ->  Wait until the next coroutine is complete

        // yield break - exit out of the coroutine

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
        
        // call play card with these units

        yield break;
    }

    /// <summary>
    /// Returns a list of all units that can activate the card used
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
    /// Returns a list of all units that the card can target
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


}
