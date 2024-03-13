using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Draw Card", menuName = "Deck/Create Cards/Draw Card")]

public class DrawCard : CardObject
{
    /*
    public int drawAmount;

    private new void Awake()
    {
        {
            if (id == -1)
            {
                id = GameManager.cardIdCount;
                GameManager.cardIdCount++;
            }
        }
        //cardType = CardType.DrawCard;
    }

    /*
    new public void PlayCardEffect()
    {
        for (int i = 0; i < drawAmount; i++)
        {
            EncounterManager.encounterManager.PlayCard(this);
        }
    }
    */
}
