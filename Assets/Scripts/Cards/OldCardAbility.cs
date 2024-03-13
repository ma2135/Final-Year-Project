using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]


public class OldCardAbility : ScriptableObject
{
    // ScriptableObjects-based Enum  - name comparison works same as enums
    // Value stored for attribute

    [SerializeField] private CardType ability;
    [SerializeField] private int value;
    [SerializeField] private int range;

    public CardType Ability { get { return ability; } }
    public int Value { get { return value; } set { this.value = value; } }
    public int Range { get { return range; } set {  range = value; } }

}
