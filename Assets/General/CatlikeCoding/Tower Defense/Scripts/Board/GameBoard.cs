using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField] Transform ground = default;
    [SerializeField] GameObject tilePrefab = default;
    Vector2Int size;
    GameTile[] tiles;

    public void Initialize(Vector2Int size)
    {
        this.size = size;
        ground.localScale = new Vector3(size.x, size.y, 1f);
        tiles = new GameTile[size.x * size.y];

        Vector2 offset = new Vector2((size.x - 1) * 0.5f, (size.y - 1) * 0.5f);
        for(int i=0,y=0;y<size.y;y++)
        {
            for(int x=0;x<size.x;x++,i++)
            {
                GameTile tile = tiles[i] = Instantiate(tilePrefab).GetComponent<GameTile>();;
                tile.transform.SetParent(this.transform, false);
                tile.transform.localPosition = new Vector3(x - offset.x, 0f, y - offset.y);
            }
        }
    }
}
