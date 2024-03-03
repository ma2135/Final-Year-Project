using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Deck/Create Deck")]
public class DeckObject : ScriptableObject
{
    [SerializeField] private List<CardObject> cardList = new List<CardObject>();
    [SerializeField] private Queue<CardObject> queue = new Queue<CardObject>();
    [SerializeField] private int queueSize = 0;



    public void SetCards(List<CardObject> cardList)
    {
        queue.Clear();
        AddCards(cardList);
    }
    public void AddCards(List<CardObject> cardListInput)
    {
        if (cardListInput == null)
        {
            Debug.LogAssertionFormat("cardList = {0}", null);
        }
        foreach(CardObject card in cardListInput)
        {
            Debug.LogFormat("Adding card ({0}) to deck ({1})", card.name, this.name);
            this.cardList.Add(card);
            queue.Enqueue(card);
            queueSize++;
        }
    }

    public int GetDeckSize()
    {
        return queue.Count;
    }

    private void OnEnable()
    {
        LoadDeck();
    }
    private void OnValidate()
    {
        LoadDeck();
    }

    public void LoadDeck()
    {
        SetCards(cardList);
        queueSize = queue.Count;
    }

    /*
    public void SaveDeck()
    {
        cardListInput = new List<CardObject>();
        while (queue.Count > 0)
        {
            CardObject card = queue.Dequeue();
            if (card != null)
            {
                cardListInput.Add(card);
            }
        }
        queueSize = queue.Count;
    }
    */

    public List<CardObject> GetXCards(int x) 
    {
        Debug.LogFormat("Queue count: {0}", queue.Count);
        List<CardObject> cards = new List<CardObject>();
        if (x == -1)
        {
            x = queue.Count - 1;
        }
        if (x >= queue.Count)
        {
            x = queue.Count;
        }
        for (int i = 0; i < x; i++)
        {
            Debug.LogFormat("Adding card to XCards: {0}", queue.Peek());
            cards.Add(queue.Dequeue());
        }
        return cards;
    }

    public CardObject RemoveCard(CardObject card)
    {
        Queue<CardObject> tempQueue = queue;
        bool flag = false;


        tempQueue.Clear();

        while (queue.Count > 0)
        {
            CardObject tempCard = queue.Dequeue();
            if (tempCard != card)
            {
                tempQueue.Enqueue(tempCard);
            }
            else
            {
                flag = true;
            }
        }
        queue = tempQueue;
        if (flag)
        {
            return card;
        } 
        else
        {
            Debug.LogFormat("Card [{0}] not found", card);
        }
        return null;
    }

    public void ClearDeck()
    {
        cardList.Clear();
        queue.Clear();
        queueSize = 0;
    }

    public void Shuffle()
    {
        CardObject[] array = new CardObject[queue.Count];
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = queue.Dequeue();
        }

        while (array.Length > 0)
        {
            queue.Clear();
            int index = Random.Range(0, array.Length);
            queue.Enqueue(array[index]);
            array[index] = null;
            CardObject[] array2 = new CardObject[array.Length - 1]; 
            for (int i = 0; i < array2.Length; i++)
            {
                if (array[i] == null)
                {
                    i--;
                }
                else
                {
                    array2[i] = array[i];
                }
            }
        }
    }

    public string DeckToString()
    {
        string output = "Deck:\n";
        foreach (CardObject card in cardList)
        {
            output += "[" + card.cardName + "]\n";
        }
        return output;
    }

}

