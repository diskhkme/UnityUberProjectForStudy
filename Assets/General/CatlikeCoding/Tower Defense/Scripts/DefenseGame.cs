using System;
using UnityEngine;

public class DefenseGame : MonoBehaviour
{
    [SerializeField] Vector2Int boardSize = new Vector2Int(11, 11);
    [SerializeField] GameBoard board = default;
    [SerializeField] GameTileContentFactory tileContentFactory = default;

    Ray TouchRay => Camera.main.ScreenPointToRay(Input.mousePosition);

    private void Awake()
    {
        board.Initialize(boardSize,tileContentFactory);
        board.ShowGrid = true;
    }

    private void OnValidate()
    {
        if (boardSize.x < 2)
            boardSize.x = 2;
        if (boardSize.y < 2)
            boardSize.y = 2;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            HandleAlternativeTouch();
            
        }
        else if(Input.GetMouseButtonDown(0))
        {
            HandleTouch();
        }

        if(Input.GetKeyDown(KeyCode.V))
        {
            board.ShowPaths = !board.ShowPaths;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            board.ShowGrid = !board.ShowGrid;
        }
    }

    private void HandleAlternativeTouch()
    {
        GameTile tile = board.GetTile(TouchRay);
        if(tile != null)
        {
            if(Input.GetKey(KeyCode.LeftShift))
            {
                board.ToggleDestination(tile);
            }
            else
            {
                board.ToggleSpawnPoint(tile);
            }
        }
    }

    void HandleTouch()
    {
        GameTile tile = board.GetTile(TouchRay);
        if(tile != null)
        {
            board.ToggleWall(tile);
        }
    }
}
