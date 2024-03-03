using System.Collections;
using System.Collections.Generic;
using Utils;
using UnityEngine;


[CreateAssetMenu(fileName = "New Equipment Loot Table", menuName = "Create Equipment Loot Table")]

public class LootTableObject : ScriptableObject
{

    [SerializeField] public ItemType itemType;
    [SerializeField] protected float[] probabilities = new float[0];
    [SerializeField] protected ItemObject[] items = new ItemObject[0];
    [SerializeField] protected float totalProbability;

    protected void Awake()
    {
        if (probabilities.Length != items.Length)
        {
            Debug.LogAssertionFormat("Probabilities and Items lengths do not match\nprobabilities length: {0}\nitems length: {1}", probabilities.Length, items.Length);
        }
        if (items.Length! > 0)
        {
            Debug.LogError("Not items in the loot table");
        }
        itemType = items[0].GetItemType();
        totalProbability = 0;
        foreach (float probability in probabilities)
        {
            totalProbability += probability;
        }
    }

    protected void OnValidate()
    {
        if (items != null && items.Length != probabilities.Length)
        {
            float[] temp = new float[items.Length];
            for (int i = 0; i < probabilities.Length; i++)
            {
                temp[i] = probabilities[i];
            }
            probabilities = temp;
        }
        totalProbability = 0;
        foreach (float probability in probabilities)
        {
            totalProbability += probability;
        }
    }

    public ItemObject RollTable()
    {
        float randFloat = Random.Range(0, totalProbability);
        float runningTotal = 0;
        for (int i = 0; i < probabilities.Length; i++)
        {
            runningTotal += probabilities[i];
            if (randFloat <= runningTotal)
            {
                if (items[i] == null)
                {
                    Debug.LogAssertion("Item is null");
                } 
                return items[i];
            }
        }
        Debug.LogAssertion("No item was created");
        return null;
    }
}

