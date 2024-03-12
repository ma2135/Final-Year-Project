using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]


[CreateAssetMenu(fileName = "New Card", menuName = "Deck/Create Card")]
public class CardObject : ScriptableObject
{
    // Data storage for the cards
    public int id = -1;
    private Card card;

    [SerializeField] private CardAbility ability;
    public int attackDamage;
    public int shieldAmount;
    public int drawAmount;
    public int healingAmount;

    public string cardName;
    public string description;
    public int cost;
    public int range = 1;

    protected void Awake()
    {
        if (id == -1)
        {
            id = GameManager.cardIdCount;
            GameManager.cardIdCount++;
        }
    }

    public void SetUp(Card card, int id, string name, string description, int cost)
    {
        this.card = card;
        cardName = name;
        this.description = description;
        this.cost = cost;
    }

    public void SetCard(Card card)
    {
        this.card = card;
    }

    public void SetAbility(CardAbility ability) { this.ability = ability; }

    public CardAbility GetAbility() { return this.ability; }

    public void SetId(int id)
    {
        this.id = id;
    }

    public void PlayCardEffect(UnitObject activator, UnitObject target)
    {
        Debug.LogFormat("Playing ({0}) card", this.name);
        if (shieldAmount > 0)
        {
            EncounterManager.encounterManager.ShieldUnit(target, shieldAmount);
        }
        if (drawAmount > 0)
        {
            EncounterManager.encounterManager.DrawCardCurrent(drawAmount);
        }
        if (healingAmount > 0)
        {
            EncounterManager.encounterManager.HealUnit(target, healingAmount);
        }
        if (attackDamage > 0)
        {
            EncounterManager.encounterManager.DamageUnit(target, attackDamage);
        }
    }
}
