using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldTile : MonoBehaviour
{
    public List<WorldConnector> prevConnectors = new List<WorldConnector>();
    public List<WorldConnector> nextConnectors = new List<WorldConnector>();
    public WorldTile newPrevTile = null;
    public WorldTile newNextTile = null;
    public WorldEvent eventType;
    [SerializeField] private Sprite image;

    /*
    public WorldTile(List<WorldConnector> nextConnectors, WorldEvent eventType)
    {
        this.nextConnectors = nextConnectors;
        this.eventType = eventType;
    }
    */

    public void SetEventType(WorldEvent eventType)
    {
        this.eventType = eventType;
    }

    public void OnValidate()
    {
        transform.GetChild(0).GetComponent<Image>().sprite = image;
        WorldConnector worldConnector;
            bool flag = false;
        if (newNextTile != null)
        {
            foreach (WorldConnector connector in nextConnectors)
            {
                if (connector.GetConnected(this) == newNextTile)
                {
                    flag = true;
                }
            }
            if (!flag)
            {
                worldConnector = new WorldConnector(this, newNextTile);
                nextConnectors.Add(worldConnector);
                newNextTile.prevConnectors.Add(worldConnector);
                newNextTile = null;
            }
        }
        if (newPrevTile != null)
        {
            foreach (WorldConnector connector in prevConnectors)
            {
                if (connector.GetConnected(this) == newPrevTile)
                {
                    flag = true;
                }
            }
            if (!flag)
            {
                worldConnector = new WorldConnector(this, newPrevTile);
                prevConnectors.Add(worldConnector);
                newPrevTile.nextConnectors.Add(worldConnector);
                newPrevTile = null;
            }
        }
    }

    public void LoadEvent()
    {
        switch (eventType)
        {
            case WorldEvent.Encounter:
                //EncounterManager.encounterManager.CreateEncounter(GameManager.gameManager.GetPlayerParty(), )
                break;
            case WorldEvent.Shop:
                //EncounterManager.encounterManager.CreateEncounter(GameManager.gameManager.GetPlayerParty(), )
                break;
            case WorldEvent.Event:
                //EncounterManager.encounterManager.CreateEncounter(GameManager.gameManager.GetPlayerParty(), )
                break;
            case WorldEvent.BossFight:
                //EncounterManager.encounterManager.CreateEncounter(GameManager.gameManager.GetPlayerParty(), )
                break;
        }
    }


}

public class WorldConnector
{
    public WorldTile connection1;
    public WorldTile connection2;

    public WorldConnector(WorldTile connection1, WorldTile connection2)
    {
        this.connection1 = connection1;
        this.connection2 = connection2;
    }

    public WorldTile GetConnected(WorldTile inputTile)
    {
        if (connection1 == inputTile)
        {
            return connection2;
        }
        else if (connection2 == inputTile)
        {
            return connection1;
        }
        else
        {
            //Debug.LogError("No world event was connected");
            return null;
        }
    }

}