using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class CardObject : ScriptableObject
{
    // Data storage for the cards
    public int id = -1;
    private Card card;

    [SerializeField] protected CardAbility ability;

    public string cardName;
    public string description;
    public int cost;

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

}
