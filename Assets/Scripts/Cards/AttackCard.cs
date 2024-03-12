using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Card", menuName = "Deck/Create Cards/Attack Card")]

public class AttackCard : CardObject
{
    /*
    [SerializeField] int attackDamage;

    private new void Awake()
    {
        if (id == -1)
        {
            id = GameManager.cardIdCount;
            GameManager.cardIdCount++;
        }
        if (range > 1)
        {
            //ability = CardAbility.Ranged;
        }
        else
        {
            //ability = CardAbility.Melee;
        }
    }

    public void SetDamage(int damage) { attackDamage = damage; }

    public int GetDamage() { return attackDamage;}

    new public void PlayCardEffect(UnitObject activator, UnitObject target)
    {
        Debug.Log("Activating card effect - Defence Card");
        EncounterManager.encounterManager.DamageUnit(target, attackDamage);

    }
    */
}
