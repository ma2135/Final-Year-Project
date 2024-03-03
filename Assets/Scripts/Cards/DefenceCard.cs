using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Defence Card", menuName = "Deck/Create Cards/Defence Card")]

public class DefenceCard : CardObject
{
    [SerializeField] private int defence;

    private new void Awake()
    {
        {
            if (id == -1)
            {
                id = GameManager.cardIdCount;
                GameManager.cardIdCount++;
            }
        }
        ability = CardAbility.Defence;
    }
    new public void PlayCardEffect()
    {
        // select unit within range
        // that unit gains armour (temporary health)
    }
}
