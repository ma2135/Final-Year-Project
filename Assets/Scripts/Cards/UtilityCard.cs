using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Healing Card", menuName = "Deck/Create Cards/Healing Card")]

public class UtilityCard : CardObject
{
    /*
    [SerializeField] int healingAmount;

    private new void Awake()
    {
        {
            if (id == -1)
            {
                id = GameManager.cardIdCount;
                GameManager.cardIdCount++;
            }
        }
        //cardType = CardType.Utility;
    }

    public void SetHealingAmount(int healingAmount) { this.healingAmount = healingAmount; }

    public int GetHealingAmount() {  return this.healingAmount; }

    new public void PlayCardEffect(UnitObject activator, UnitObject target)
    {
        Debug.Log("Activating card effect - Utility Card");
        EncounterManager.encounterManager.HealUnit(target, healingAmount);
        //select unit
        //that unit regains health
    }
    */
}
