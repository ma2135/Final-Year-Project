using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Card : MonoBehaviour
{
    // Game object interaction and logic for the cards
    [SerializeField] private CardObject cardData;
    [SerializeField] private int dataId;
    [SerializeField] private new string name;

    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private TMP_Text costTxt;
    [SerializeField] private TMP_Text rangeTxt;
    [SerializeField] private TMP_Text descriptionTxt;

    public void SetUp(int id, CardObject cardData)
    {
        dataId = id;
        this.cardData = cardData;
        name = cardData.cardName;
        cardData.SetCard(this);
        nameTxt.text = name;
        costTxt.text = cardData.cost.ToString();
        rangeTxt.text = cardData.range.ToString();
        descriptionTxt.text = cardData.description.ToString();
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
    public int GetMovementRange() {  return cardData.movement; }
    public void SetRange(int movement) {  cardData.movement = movement; }
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
        //activate effect

    }
}
