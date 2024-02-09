/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Create Attribute")]
public class OldAttribute : ScriptableObject
{
    [SerializeField] private Level level;
    [SerializeField] private List<OldCardAbility> abilities;

    [SerializeField] private new string name;
    [SerializeField] private DeckObject level1 = null;
    [SerializeField] private DeckObject level2 = null;
    [SerializeField] private DeckObject level3 = null;

    public DeckObject GetCards(int level)
    {
        if (level1 == null)
        {
            return null;
        }
        DeckObject newDeckScriptable = level1;
        switch (level)
        {
            case 2:
                newDeckScriptable.AddCards(level2.GetCards());
                return newDeckScriptable;
            case 3:
                newDeckScriptable.AddCards(level2.GetCards());
                newDeckScriptable.AddCards(level3.GetCards());
                return newDeckScriptable;
            default: return newDeckScriptable;
        }
    }
}
*/