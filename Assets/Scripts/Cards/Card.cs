using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    // Game object interaction and logic for the cards
    [SerializeField] private CardObject cardData;
    [SerializeField] private int dataId;
    [SerializeField] private new string name;

    public void SetUp(int id, CardObject cardData)
    {
        dataId = id;
        this.cardData = cardData;
        name = cardData.name;
        cardData.SetCard(this);
    }

    public void SetCardData(CardObject cardData)
    {
        this.cardData = cardData;
        dataId = cardData.id;
    }

    public int GetID() {  return dataId; }

    public string GetName() {  return cardData.name; }
    public string GetDescription() {  return cardData.description;}

    public int GetCost() {  return cardData.cost; }
    public void SetCost(int cost) {  cardData.cost = cost; }

    /*
    public int GetRange() {  return cardData.range; }
    public void SetRange(int range) {  cardData.range = range; }
    */

    public void SetAbility(CardAbility ability) { cardData.SetAbility(ability);}
    public CardAbility GetAbility() { return cardData.GetAbility(); }

    /*
    public void AddAbility(OldCardAbility ability) {  cardData.ability.Add(ability); }
    public void RemoveAbility(OldCardAbility ability) {  cardData.ability.Remove(ability); }
    public List<OldCardAbility> GetAbilities() {  return cardData.ability; }
    */
    public void PlayCard()
    {
        //move to discard pile
        //activate effect

    }
}
