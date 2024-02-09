using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Draw Card", menuName = "Deck/Create Cards/Draw Card")]

public class DrawCard : CardObject
{
    [SerializeField] int drawAmount;

    private new void Awake()
    {
        {
            if (id == -1)
            {
                id = GameManager.cardIdCount;
                GameManager.cardIdCount++;
            }
        }
        ability = CardAbility.DrawCard;
    }

    public int GetDrawAmount() { return drawAmount; }
}
