using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;

    [SerializeField] private Canvas HUDCanvas;
    [SerializeField] private GameObject handObj;
    [SerializeField] private GameObject cardPrefab;
    [Header("Player's Hand")]
    [SerializeField] private GameObject leftMarker;
    [SerializeField] private GameObject rightMarker;

    [SerializeField] private int maxHandSize = 5;
    private int currHandSize = 0;

    List<GameObject> hand = new List<GameObject>();
    [SerializeField] private GameObject[] cardSlots;
    [SerializeField] private bool[] freeCardSlots;

    [Header("Inventory")]
    [SerializeField] private Item[] equipSlots;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Item itemPrefab;


    private void Start()
    {
        freeCardSlots = new bool[maxHandSize];
        for (int i = 0; i < freeCardSlots.Length; i++) 
        { 
            freeCardSlots[i] = true;
            cardSlots[i].SetActive(false);
        }
    }


    public void DrawCard()
    {
        if (gameManager == null)
        {
            return;
        }
        if (gameManager.GetPlayerDeckSize() < 1)
        {
            Debug.LogAssertionFormat("Deck size: {0}", gameManager.GetPlayerDeckSize());
            return;
        }
        if (currHandSize < maxHandSize - 1)
        {
            //should only ever draw 1 card
            CardObject card = gameManager.GetTopDeck(null);

            bool flag = false;
            for (int i = 0; i < cardSlots.Length; i++)
            {
                if (freeCardSlots[i])
                {
                    cardSlots[i].transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>().text = card.name;
                    cardSlots[i].transform.GetChild(0).transform.GetChild(1).GetComponent<TMP_Text>().text = card.cost.ToString();
                    freeCardSlots[i] = false;
                    flag = true;
                    cardSlots[i].SetActive(true);
                    break;
                }
            }
            if (flag)
            {
                Debug.LogAssertionFormat("All card slots used");
            }


            /*
            float left = leftMarker.transform.position.x;
            float right = rightMarker.transform.position.x;
            float middle = (rightMarker.transform.position.x - leftMarker.transform.position.x) / 2;

            float step = middle / maxHandSize;
            left = middle - (step * currHandSize)/2;

            Debug.LogFormat("left: {0}", leftMarker.transform.position.x);
            Debug.LogFormat("right: {0}", right);
            Debug.LogFormat("middle: {0}", middle);
            Debug.LogFormat("step: {0}", step);

            for (int i = 1; i <= currHandSize - 1; i++)
            {
                Debug.Log(left + (step * i));
                hand[i].transform.position = new Vector3(left + (step * i), hand[i].transform.position.y);
            }
            */


        }
    }

    public void DisplayUnit(UnitObject unit)
    {
        for (int i = 0; i <= equipSlots.Length; i++ )
        {
            equipSlots[i].SetItemObject(unit.GetEquipment()[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
