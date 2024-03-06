using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Defence Card", menuName = "Deck/Create Cards/Defence Card")]

public class DefenceCard : CardObject
{
    [SerializeField] private int shieldAmount;

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
    new public void PlayCardEffect(UnitObject activator, UnitObject target)
    {
        EncounterManager.encounterManager.ShieldUnit(target, shieldAmount);
        // select unit within movement
        // that unit gains armour (temporary health)
    }
}
