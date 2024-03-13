using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;

    [SerializeField] public static int cardIdCount;
    //[SerializeField] GameObject deckContainer;
    //[SerializeField] GameObject deckPrefab;
    [SerializeField] GameObject cardPrefab;

    //https://www.youtube.com/watch?v=u3YdlUW1nx0&ab_channel=SasquatchBStudios

    private const string ATTRIBUTEPATH = "ScriptableObjects/Attributes";
    private const string CARDABILITIESPATH = "ScriptableObjects/CardAbilities";
    private const string CARDSPATH = "ScriptableObjects/Cards";
    //[SerializeField] List<Attribute> allAtributes = new List<Attribute>();

    [Header("General")]
    [SerializeField] List<Party> enemyParties = new List<Party>();
    public const float AGGRESSIVE_TILE_DISTANCE = 1;
    public const float DEFENSIVE_TILE_DISTANCE = 1;

    [Header("Player")]
    [SerializeField] Party playerParty = null;
    public int playerDrawSize = 5;

    [Header("Items")]
    [SerializeField] private LootTableObject startArmourObjects;
    [SerializeField] private LootTableObject startHelmetObjects;
    [SerializeField] private LootTableObject startShieldObjects;
    [SerializeField] private LootTableObject startWeaponObjects;
    [SerializeField] private LootTableObject startUtilityObjects;

    // Start is called before the first frame update
    private void Start()
    {
        gameManager = this;// transform.GetComponent<GameManager>();

        if (playerParty == null)
        {
            Debug.LogErrorFormat("Player party is null");
        }
        else
        {
            foreach (UnitObject unit in playerParty.GetAllUnits())
            {
                unit.playerUnit = true; ;
            }
            playerParty.SetUp();
        }
        if (enemyParties.Count == 0)
        {
            Debug.LogErrorFormat("enemyParties is empty: {0}", enemyParties.Count);
        }
        foreach (Party party in enemyParties)
        {
            party.SetUp();
        }


    }

    public Party GetPlayerParty()
    {
        return playerParty;
    }

    public Party GetRandomParty()
    {
        int randEnemy = (int)Mathf.Round(Random.Range(0, enemyParties.Count));
        Debug.LogFormat("enemy Parties size: {0},  {1} selected", enemyParties.Count, randEnemy);
        return enemyParties[randEnemy];
    }


    public void EquipItemToUnit(EquipmentObject item, UnitObject unit, EquipmentSlot itemLocation)
    {
        unit.EquipItem(item, itemLocation);
    }


    public void EquipUnitStart(UnitObject unit)
    {
        Debug.Log("Getting starting equipment");
        EquipmentObject[] equipment = new EquipmentObject[5];
        for (int i = 0; i < equipment.Length; i++)
        {
            switch (i)
            {
                case 0:
                    equipment[i] = (EquipmentObject)startHelmetObjects.RollTable();
                    break;
                case 1:
                    equipment[i] = (EquipmentObject)startArmourObjects.RollTable();
                    break;
                case 2:
                    equipment[i] = (EquipmentObject)startShieldObjects.RollTable();
                    break;
                case 3:
                    equipment[i] = (EquipmentObject)startWeaponObjects.RollTable();
                    break;
                case 4:
                    equipment[i] = (EquipmentObject)startUtilityObjects.RollTable();
                    break;
            }
        }
        unit.EquipUnit(equipment);
    }


    public EquipmentObject[] CreateLoadout()
    {
        EquipmentObject[] equipment = new EquipmentObject[5];
        return null;
    }

    public CardObject GetTopDeck(DeckObject deck)
    {
        if (deck == null)
        {
            deck = playerParty.GetDeck();
        }
        List<CardObject> list = playerParty.GetDeck().GetXCards(1);
        if (list.Count != 1)
        {
            Debug.LogFormat("Top Decked card amount != 1: {0}", list.Count);
            foreach (CardObject card in list)
            {
                Debug.LogFormat("card: {0}", card.cardName);
            }
        }
        return list[0];
    }


}
