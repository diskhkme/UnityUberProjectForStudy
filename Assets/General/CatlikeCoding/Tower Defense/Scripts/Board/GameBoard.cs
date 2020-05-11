using UnityEngine;
using System.Collections.Generic;

namespace Defense
{
    public class GameBoard : MonoBehaviour
    {
        [SerializeField] Transform ground = default;
        [SerializeField] GameObject tilePrefab = default;
        [SerializeField] Texture2D gridTexture = default;

        Vector2Int size;
        GameTile[] tiles;
        //path finding하면서 담을 정보
        Queue<GameTile> searchFrontier = new Queue<GameTile>();
        //enemy spawn point들을 담은 list
        List<GameTile> spawnPoints = new List<GameTile>();
        GameTileContentFactory contentFactory;
        bool showPaths, showGrid;
        List<GameTileContent> updatingContent = new List<GameTileContent>();


        public int spawnPointCount => spawnPoints.Count;
                       
        public bool ShowPaths
        {
            get => showPaths;
            set
            {
                showPaths = value;
                if (showPaths)
                {
                    foreach (GameTile tile in tiles)
                    {
                        tile.ShowPath();
                    }
                }
                else
                {
                    foreach (GameTile tile in tiles)
                    {
                        tile.HidePath();
                    }
                }
            }
        }

        public bool ShowGrid
        {
            get => showGrid;
            set
            {
                showGrid = value;
                Material m = ground.GetComponent<MeshRenderer>().material;
                if (showGrid)
                {
                    m.mainTexture = gridTexture;
                    m.SetTextureScale("_MainTex", size);
                }
                else
                {
                    m.mainTexture = null;
                }
            }
        }

        public void Initialize(Vector2Int size, GameTileContentFactory contentFactory)
        {
            this.size = size;
            this.contentFactory = contentFactory;
            ground.localScale = new Vector3(size.x, size.y, 1f);
            tiles = new GameTile[size.x * size.y];

            Vector2 offset = new Vector2((size.x - 1) * 0.5f, (size.y - 1) * 0.5f);
            for (int i = 0, y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++, i++)
                {
                    GameTile tile = tiles[i] = Instantiate(tilePrefab).GetComponent<GameTile>(); ;
                    tile.transform.SetParent(this.transform, false);
                    tile.transform.localPosition = new Vector3(x - offset.x, 0f, y - offset.y);

                    if (x > 0)
                        GameTile.MakeEastWestNeighbor(tile, tiles[i - 1]);
                    if (y > 0)
                        GameTile.MakeNorthSouthNeighbors(tile, tiles[i - size.x]);

                    tile.IsAlternative = (x & 1) == 0; //짝수 번째는 alternative
                    if ((y & 1) == 0)
                    {
                        tile.IsAlternative = !tile.IsAlternative;
                    }
                    tile.Content = contentFactory.Get(GameTileContentType.Empty); //초기에 타일들을 empty로 채움
                }
            }

            ToggleDestination(tiles[tiles.Length / 2]);
            ToggleSpawnPoint(tiles[0]);
        }

        bool FindPaths()
        {
            foreach (GameTile tile in tiles)
            {
                if (tile.Content.Type == GameTileContentType.Destination)
                {
                    tile.BecomeDestination();
                    searchFrontier.Enqueue(tile); //destination으로 지정된 타일을 enqueue
                }
                else
                {
                    tile.ClearPath();
                }
            }

            if (searchFrontier.Count == 0) //목적지 타일이 없을 때
            {
                return false;
            }

            while (searchFrontier.Count > 0)
            {
                GameTile tile = searchFrontier.Dequeue(); //queue 타일 하나 제거
                if (tile != null)
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

            foreach (GameTile tile in tiles)
            {
                if (!tile.HasPath)
                {
                    return false;
                }
            }

            if (showPaths)
            {
                foreach (GameTile tile in tiles)
                {
                    tile.ShowPath();
                }
            }


            return true;
        }

        //ray casting을 통한 타일 셀렉션 기능
        public GameTile GetTile(Ray ray)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, 1)) //default layer만 check하도록 변경(enemy는 tile 놓을 때 간섭하지 않도록)
            {
                int x = (int)(hit.point.x + size.x * 0.5f); //hit 위치를 보드의 타일 위치로 변환
                int y = (int)(hit.point.z + size.y * 0.5f); //hit 위치를 보드의 타일 위치로 변환
                if (x >= 0 && x < size.x && y > 0 && y < size.y)
                {
                    return tiles[x + y * size.x];
                }

            }
            return null;
        }

        public void GameUpdate()
        {
            for(int i=0;i<updatingContent.Count;i++)
            {
                updatingContent[i].GameUpdate();
            }
        }

        public void ToggleDestination(GameTile tile)
        {
            //목적지 타일이 클릭되면, empty로 바꾸고 path 갱신
            if (tile.Content.Type == GameTileContentType.Destination)
            {
                tile.Content = contentFactory.Get(GameTileContentType.Empty);
                if (!FindPaths()) //empty로 바꾸어 목적지가 없어질 경우의 처리. 이미 findpath에서 bool return 하도록 수정 함
                {
                    tile.Content = contentFactory.Get(GameTileContentType.Destination);
                    FindPaths();
                }
            }
            else if (tile.Content.Type == GameTileContentType.Empty)
            {
                tile.Content = contentFactory.Get(GameTileContentType.Destination);
                FindPaths();
            }
        }

        public void ToggleWall(GameTile tile)
        {
            if (tile.Content.Type == GameTileContentType.Wall)
            {
                tile.Content = contentFactory.Get(GameTileContentType.Empty);
                FindPaths();
            }
            else if (tile.Content.Type == GameTileContentType.Empty)
            {
                tile.Content = contentFactory.Get(GameTileContentType.Wall);
                if (!FindPaths())
                {
                    tile.Content = contentFactory.Get(GameTileContentType.Empty);
                    FindPaths();
                }
            }
        }

        //지금까지처럼, 클릭으로 spawn point 할당 기능 추가. 단, spawn point는 path에 영향을 주지 않음
        public void ToggleSpawnPoint(GameTile tile)
        {
            if (tile.Content.Type == GameTileContentType.SpawnPoint)
            {
                if (spawnPoints.Count > 1) //적어도 하나의 spawn point는 남아 있도록 함
                {
                    spawnPoints.Remove(tile);
                    tile.Content = contentFactory.Get(GameTileContentType.Empty);
                }
            }
            else if (tile.Content.Type == GameTileContentType.Empty)
            {
                tile.Content = contentFactory.Get(GameTileContentType.SpawnPoint);
                spawnPoints.Add(tile);
            }
        }

        public GameTile GetSpawnPoint(int index)
        {
            return spawnPoints[index];
        }

        public void ToggleTower(GameTile tile)
        {
            if (tile.Content.Type == GameTileContentType.Tower)
            {
                updatingContent.Remove(tile.Content);
                tile.Content = contentFactory.Get(GameTileContentType.Empty);
                FindPaths();
            }
            else if (tile.Content.Type == GameTileContentType.Empty)
            {
                tile.Content = contentFactory.Get(GameTileContentType.Tower);
                if (FindPaths())
                {
                    updatingContent.Add(tile.Content);
                }
                else
                {
                    tile.Content = contentFactory.Get(GameTileContentType.Empty);
                    FindPaths();
                }
            }
            else if (tile.Content.Type == GameTileContentType.Wall) //wall에서 바로 tower로 바꾸는 것을 지원
            {
                tile.Content = contentFactory.Get(GameTileContentType.Tower);//이때는 findPath 갱신 불필요
                updatingContent.Add(tile.Content);
            }
        }
    }

}
