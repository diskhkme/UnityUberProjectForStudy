using System;
using UnityEngine;

namespace Defense
{
    public class DefenseGame : MonoBehaviour
    {
        [SerializeField] Vector2Int boardSize = new Vector2Int(11, 11);
        [SerializeField] GameBoard board = default;
        [SerializeField] GameTileContentFactory tileContentFactory = default;

        [SerializeField] EnemyFactory enemyFactory = default; //enemy 생성 factory에 대한 참조
        [SerializeField, Range(0.1f, 10f)] float spawnSpeed = 1f; //enemy 생성 속도 파라메터
        float spawnProgress;

        EnemyCollection enemies = new EnemyCollection();

        Ray TouchRay => Camera.main.ScreenPointToRay(Input.mousePosition);

        private void Awake()
        {
            board.Initialize(boardSize, tileContentFactory);
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
            if (Input.GetMouseButtonDown(1))
            {
                HandleAlternativeTouch();

            }
            else if (Input.GetMouseButtonDown(0))
            {
                HandleTouch();
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                board.ShowPaths = !board.ShowPaths;
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                board.ShowGrid = !board.ShowGrid;
            }

            spawnProgress += spawnSpeed * Time.deltaTime;
            while (spawnProgress >= 1f)
            {
                spawnProgress -= 1f;
                SpawnEnemy();
            }

            enemies.GameUpdate();
            Physics.SyncTransforms(); 
            //적이 생성 후에  spawn 위치로 가기 때문에, physics engine과 sync되지 않으면 targeting system이 제대로 동작하지 않음
            //따라서 적 업데이트 이후에 synchronization을 한 뒤에 targeting system을 invoke하는 board의 gameupdate를 수행해 주어야 함

            board.GameUpdate();
        }

        private void HandleAlternativeTouch()
        {
            GameTile tile = board.GetTile(TouchRay);
            if (tile != null)
            {
                if (Input.GetKey(KeyCode.LeftShift))
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
            if (tile != null)
            {
                if(Input.GetKey(KeyCode.LeftShift))
                {
                    board.ToggleTower(tile);
                }
                else
                {
                    board.ToggleWall(tile);
                }
                
            }
        }

        void SpawnEnemy()
        {
            GameTile spawnPoint = board.GetSpawnPoint(UnityEngine.Random.Range(0, board.spawnPointCount));
            Enemy enemy = enemyFactory.Get();
            enemy.SpawnOn(spawnPoint);
            enemies.Add(enemy);
        }
    }

}
