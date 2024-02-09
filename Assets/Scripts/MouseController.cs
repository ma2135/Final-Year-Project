using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

//https://www.youtube.com/watch?v=riLtglHwoYw&ab_channel=LawlessGames

public class MouseController : MonoBehaviour
{
    [SerializeField] Camera _mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var tileHit = GetMousedOver();
        if (tileHit.HasValue && tileHit.Value.collider != null)
        {
            RaycastHit2D hit = tileHit.Value;
            Collider2D collider = hit.collider;
            if (collider == null)
            {
                Debug.LogAssertion(string.Format("collider is null"));
            }
            else
            {
                if (collider.gameObject == null)
                {
                    Debug.LogAssertion(string.Format("gameObject is null"));
                }

            }
            GameObject gameobject = collider.gameObject;

            var overlayTile = hit.collider.gameObject;
            this.transform.position = overlayTile.transform.position;
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = overlayTile.GetComponent<SpriteRenderer>().sortingOrder;
        }
    }

    /// <summary>
    /// Get objects moused over
    /// 
    /// Can extend to get list of objects hit and return the top one
    /// </summary>
    /// <returns></returns>
    public RaycastHit2D? GetMousedOver()
    {
        Vector3 mousePos = _mainCamera.ScreenToWorldPoint( Input.mousePosition );
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        return hit; 
    }

    //https://www.youtube.com/watch?v=mRkFj8J7y_I&ab_channel=agoodboygames
    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }
        var tileHit = GetMousedOver();
        if (tileHit.HasValue && tileHit.Value.collider != null)
        {
            //Debug.Log(tileHit.Value.collider.gameObject.name);
            MapManager.mapManager.TileClicked(tileHit.Value.collider.gameObject.GetComponent<GameTile>(), true);
        }
        
    }


    public void OnRightClick(InputAction.CallbackContext context)
    {

        if (!context.started)
        {
            return;
        }
        var tileHit = GetMousedOver();
        if (tileHit.HasValue && tileHit.Value.collider != null)
        {
            //Debug.Log(tileHit.Value.collider.gameObject.name);
            MapManager.mapManager.TileClicked(tileHit.Value.collider.gameObject.GetComponent<GameTile>(), false);
        }

    }
}
