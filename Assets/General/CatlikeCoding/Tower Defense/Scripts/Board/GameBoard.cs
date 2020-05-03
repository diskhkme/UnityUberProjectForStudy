using UnityEngine;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour
{
    [SerializeField] Transform ground = default;
    [SerializeField] GameObject tilePrefab = default;
    Vector2Int size;
    GameTile[] tiles;

    //path finding하면서 담을 정보
    Queue<GameTile> searchFrontier = new Queue<GameTile>();

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

                if (x > 0)
                    GameTile.MakeEastWestNeighbor(tile, tiles[i - 1]);
                if (y > 0)
                    GameTile.MakeNorthSouthNeighbors(tile, tiles[i - size.x]);

                tile.IsAlternative = (x & 1) == 0; //짝수 번째는 alternative
                if((y&1) == 0)
                {
                    tile.IsAlternative = !tile.IsAlternative;
                }
            }
        }

        FindPaths();
    }

    void FindPaths()
    {
        foreach(GameTile tile in tiles)
        {
            tile.ClearPath();
        }
        tiles[tiles.Length/2].BecomeDestination(); //정가운데 타일을 목적지로
        searchFrontier.Enqueue(tiles[tiles.Length/2]);
        
        while(searchFrontier.Count>0)
        {
            GameTile tile = searchFrontier.Dequeue(); //queue 타일 하나 제거
            if(tile != null)
            {
                if (tile.IsAlternative)
                {
                    searchFrontier.Enqueue(tile.GrowPathNorth()); //그 타일의 사방 인접 정보 탐색하여 업데이트 후 큐에 삽입
                    searchFrontier.Enqueue(tile.GrowPathSouth());
                    searchFrontier.Enqueue(tile.GrowPathEast());
                    searchFrontier.Enqueue(tile.GrowPathWest());
                }
                else
                {
                    searchFrontier.Enqueue(tile.GrowPathWest());
                    searchFrontier.Enqueue(tile.GrowPathEast());
                    searchFrontier.Enqueue(tile.GrowPathSouth());
                    searchFrontier.Enqueue(tile.GrowPathNorth()); //그 타일의 사방 인접 정보 탐색하여 업데이트 후 큐에 삽입
                }

                
            }
        }

        foreach(GameTile tile in tiles)
        {
            tile.ShowPath();
        }
    }
}
