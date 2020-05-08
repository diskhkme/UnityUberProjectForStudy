using System.Collections.Generic;

namespace Defense
{
    //factory는 관리 역할로 남겨두고, 게임 내에서 살아있는 적들을 관리할 collection class를 만들어줌. 객체는 defence game에 들어있음
    [System.Serializable]
    public class EnemyCollection
    {
        List<Enemy> enemies = new List<Enemy>();

        public void Add(Enemy enemy)
        {
            enemies.Add(enemy);
        }

        public void GameUpdate()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (!enemies[i].GameUpdate())
                {
                    int lastIndex = enemies.Count - 1;
                    enemies[i] = enemies[lastIndex];
                    enemies.RemoveAt(lastIndex);
                    i -= 1;
                }
            }
        }
    }
}
