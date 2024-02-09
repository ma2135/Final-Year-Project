using System.Collections;
using System.Collections.Generic;
using Utils;
using UnityEngine;


[CreateAssetMenu(fileName = "New Loot Table", menuName = "Create Loot Table")]

public class LootTableObject : ScriptableObject
{
    ItemType itemType;
    [SerializeField] float[] probabilities = new float[0];
    [SerializeField] ItemObject[] items = new ItemObject[0];
    [SerializeField] float totalProbability;

    private void Awake()
    {
        if (probabilities.Length != items.Length)
        {
            Debug.LogAssertionFormat("Probabilities and Items lengths do not match\nprobabilities length: {0}\nitems length: {1}", probabilities.Length, items.Length);
        }
        if (items.Length !> 0)
        {
            Debug.LogError("Not items in the loot table");
        }
        itemType = items[0].GetItemType();
        totalProbability = 0;
        foreach(float probability in probabilities)
        {
            totalProbability += probability;
        }
    }

    public ItemObject GenerateItem()
    {
        float randFloat = Random.Range(0, totalProbability);
        float runningTotal = 0;
        for (int i = 0; i < probabilities.Length; i++)
        {
            runningTotal += probabilities[i];
            if (randFloat <= runningTotal)
            {
                return items[i];
            }
        }
        Debug.LogAssertion("No item was created");
        return null;
    }

}

