using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] public static int cardIdCount;
    [SerializeField] GameObject deckContainer;
    [SerializeField] GameObject deckPrefab;
    [SerializeField] GameObject cardPrefab;

    //https://www.youtube.com/watch?v=u3YdlUW1nx0&ab_channel=SasquatchBStudios

    private const string ATTRIBUTEPATH = "ScriptableObjects/Attributes";
    private const string CARDABILITIESPATH = "ScriptableObjects/CardAbilities";
    private const string CARDSPATH = "ScriptableObjects/Cards";
    //[SerializeField] List<Attribute> allAtributes = new List<Attribute>();

    [Header("General")]
    [SerializeField] DeckObject allCards;
    [SerializeField] GameObject allCardsDeckObj;
    [SerializeField] List<Party> allParty = new List<Party>();

    [Header("Player")]
    [SerializeField] DeckObject playerDeck;
    [SerializeField] Party playerParty = null;

    [Header("Items")]
    [SerializeField] private List<EquipmentObject> armourObjects;
    [SerializeField] private List<EquipmentObject> helmetObjects;
    [SerializeField] private List<EquipmentObject> shieldObjects;
    [SerializeField] private List<EquipmentObject> weaponObjects;
    [SerializeField] private List<EquipmentObject> utilityObjects;

    [SerializeField] private static List<EquipmentObject>[] allEquipment;

    // Start is called before the first frame update
    private void Start()
    {
        allEquipment = new List<EquipmentObject>[] { helmetObjects, armourObjects, shieldObjects, weaponObjects, utilityObjects };

        if (playerParty == null)
        {
            playerParty = allParty[0];
        }
        if (!allParty.Contains(playerParty))
        {
            allParty.Add(playerParty);
        }
        /*
        foreach (DeckObject deck in allDecks)
        {
            deck.LoadDeck();
        }*/

        /*
        Attribute[] allAtributes = Resources.LoadAll<Attribute>(ATTRIBUTEPATH);
        Debug.LogFormat("allAttributes.length(): {0}", allAtributes.Length);
        foreach (Attribute attribute in allAtributes)
        {
            this.allAtributes.Add(attribute);
        }

        OldCardAbility[] allCardAbilities = Resources.LoadAll<OldCardAbility>(CARDABILITIESPATH);
        Debug.LogFormat("allAttributes.length(): {0}", allAtributes.Length);
        foreach (OldCardAbility ability in allCardAbilities)
        {
            this.allCardAbilities.Add(ability);
        }
        */

        CardObject[] allScriptableCards = Resources.LoadAll<CardObject>(CARDSPATH);
        Debug.LogFormat("allAttributes.length(): {0}", allScriptableCards.Length);
        foreach (CardObject cardData in allScriptableCards)
        {
            allCards.AddCards(new List<CardObject> { cardData });
        }

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
            deck = playerDeck;
        }
        List<CardObject> list = playerDeck.GetXCards(1);
        if (list.Count != 1)
        {
            Debug.LogAssertionFormat("Top Decked card amount != 1: {0}", list.Count);
            foreach (CardObject card in list)
            {
                Debug.LogAssertionFormat("card: {0}", card.cardName);
            }
        }
        return list[0];
    }

    public int GetPlayerDeckSize()
    {
        return playerDeck.GetDeckSize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }



}
