using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Transform navmeshFloor;
    public Transform navmeshMaskPrefab; //화면 밖 부분을 자르기 위해..?
    public Vector2 mapSize;
    public Vector2 maxMapSize;
    public int seed = 10;

    [Range(0,1)] public float outlinePercent;
    [Range(0, 1)] public float obstclePercent;
    public float tileSize;


    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;
    Coord mapCenter;

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        allTileCoords = new List<Coord>();
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), seed));
        mapCenter = new Coord((int)(mapSize.x / 2), (int)(mapSize.y / 2));

        string holderName = "Generated Map";
        if(transform.Find(holderName))
        {
            //Editor에서 호출할때는 Immediate 사용
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for(int x=0;x<mapSize.x;x++)
        {
            for(int y=0;y<mapSize.y;y++)
            {
                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90f)) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                newTile.parent = mapHolder;
            }
        }

        bool[,] obstacleMap = new bool[(int)mapSize.x, (int)mapSize.y];

        int obstacleCount = (int)((mapSize.x*mapSize.y) * obstclePercent);
        int currentObstacleCount = 0;
        for(int i=0;i<obstacleCount;i++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;

            //맵의 가운데는 막히지 않게
            if (randomCoord != mapCenter && MapIsFullyAccessible(obstacleMap,currentObstacleCount))
            {
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * 0.5f, Quaternion.identity) as Transform;
                newObstacle.parent = mapHolder;
                newObstacle.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
            }
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }

        //Navmesh mask gen
        Transform maskLeft = Instantiate(navmeshMaskPrefab, Vector3.left * ((mapSize.x + maxMapSize.x) / 4) * tileSize, Quaternion.identity) as Transform;
        maskLeft.parent = mapHolder;
        maskLeft.localScale = new Vector3((maxMapSize.x - mapSize.x) / 2, 1, mapSize.y) * tileSize;
        Transform maskRight = Instantiate(navmeshMaskPrefab, Vector3.right * ((mapSize.x + maxMapSize.x) / 4) * tileSize, Quaternion.identity) as Transform;
        maskRight.parent = mapHolder;
        maskRight.localScale = new Vector3((maxMapSize.x - mapSize.x) / 2, 1, mapSize.y) * tileSize;
        Transform maskTop = Instantiate(navmeshMaskPrefab, Vector3.forward * ((mapSize.y + maxMapSize.y) / 4) * tileSize, Quaternion.identity) as Transform;
        maskTop.parent = mapHolder;
        maskTop.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - mapSize.y) / 2) * tileSize;
        Transform maskBottom = Instantiate(navmeshMaskPrefab, Vector3.back * ((mapSize.y + maxMapSize.y) / 4) * tileSize, Quaternion.identity) as Transform;
        maskBottom.parent = mapHolder;
        maskBottom.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - mapSize.y) / 2) * tileSize;

        navmeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;
    }

    //Map이 완전히 닫히지 않도록 하기 위해
    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        //floodfill algorithm
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0),obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();

        queue.Enqueue(mapCenter);
        mapFlags[mapCenter.y, mapCenter.y] = true;

        int accessibleTileCount = 1;

        while(queue.Count > 0)
        {
            Coord tile = queue.Dequeue();

            for(int x=-1;x<=1;x++)
            {
                for(int y=-1;y<=1;y++)
                {
                    int neighborX = tile.x + x;
                    int neighborY = tile.y + y;
                    if(x == 0 || y == 0) //diagonal은 체크안함
                    {
                        //outside는 제외
                        if(neighborX >= 0 && neighborX < obstacleMap.GetLength(0) && neighborY >= 0 && neighborY < obstacleMap.GetLength(1))
                        {
                            //아직 체크하지 않았고, obstacle이 없으면
                            if(!mapFlags[neighborX,neighborY] && !obstacleMap[neighborX,neighborY])
                            {
                                mapFlags[neighborX, neighborY] = true; //체크 했음
                                queue.Enqueue(new Coord(neighborX, neighborY)); //큐에 넣음
                                accessibleTileCount++; //중심에서 시작해서 접근가능한 타일의 개수
                            }

                        }
                    }
                }
            }
        }

        int targetAccessibleTileCount = (int)(mapSize.x * mapSize.y - currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;
    }

    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-mapSize.x / 2 + 0.5f + x, 0f, -mapSize.y / 2 + 0.5f + y) * tileSize;
    }

    public Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue(); //앞에서 빼고
        shuffledTileCoords.Enqueue(randomCoord); //뒤에 다시 집어넣음
        return randomCoord;
    }

    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            x = _x; y = _y;
        }

        public static bool operator ==(Coord lhs, Coord rhs)
        {
            return Equals(lhs, rhs);
        }
        public static bool operator !=(Coord lhs, Coord rhs)
        {
            return !Equals(lhs, rhs);
        }

        public override bool Equals(object obj)
        {
            if(obj is Coord coord)
            {
                if(this.x == coord.x && this.y == coord.y)
                {
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}
