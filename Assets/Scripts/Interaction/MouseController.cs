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
    /// https://www.youtube.com/watch?v=ptmum1FXiLE&ab_channel=CodeMonkey
    /// </summary>
    /// <returns></returns>
    public RaycastHit2D? GetMousedOver()
    {
        Vector3 mousePos = _mainCamera.ScreenToWorldPoint( Input.mousePosition );
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
        return hit; 
    }

    // https://stackoverflow.com/questions/57010713/unity-ispointerovergameobject-issue
    private bool IsMouseOverUI()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].gameObject.layer == 5) //5 = UI layer
            {
                Debug.LogFormat("UI clicked");
                return true;
            }
        }

        //Debug.LogAssertionFormat("Clicked UI: {0}", false);
        return false;

    }

    //https://www.youtube.com/watch?v=mRkFj8J7y_I&ab_channel=agoodboygames
    public void OnLeftClick(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }
        var tileHit = GetMousedOver();
        if (tileHit.HasValue && tileHit.Value.collider != null && !IsMouseOverUI())
        {
            //Debug.Log(tileHit.Value.collider.gameObject.name);

            Vector2Int matrixCoords = tileHit.Value.collider.gameObject.GetComponent<GameTile>().GetMatrixCoords();
            Debug.LogFormat("Tile left clicked at {0}", matrixCoords.ToString());
            MapManager.mapManager.TileLeftClicked(matrixCoords);

        }
        
    }


    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }
        var tileHit = GetMousedOver();
        if (tileHit.HasValue && tileHit.Value.collider != null && !IsMouseOverUI())
        {
            Vector2Int matrixCoords = tileHit.Value.collider.gameObject.GetComponent<GameTile>().GetMatrixCoords();
            Debug.LogFormat("Tile right clicked at {0}", matrixCoords.ToString());
            MapManager.mapManager.TileRightClicked(matrixCoords);
        }

    }
}
