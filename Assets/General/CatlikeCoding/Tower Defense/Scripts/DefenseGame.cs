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
        if(Input.GetMouseButtonDown(0))
        {
            HandleTouch();
        }
    }

    void HandleTouch()
    {
        GameTile tile = board.GetTile(TouchRay);
        if(tile != null)
        {
            board.ToggleDestination(tile);
        }
    }
}
