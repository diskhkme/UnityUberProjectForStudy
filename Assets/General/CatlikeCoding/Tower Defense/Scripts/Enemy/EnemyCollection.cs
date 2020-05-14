using System.Collections.Generic;

namespace Defense
{
    //factory는 관리 역할로 남겨두고, 게임 내에서 살아있는 적들을 관리할 collection class를 만들어줌. 객체는 defence game에 들어있음
    [System.Serializable]
    public class GameBehaviorCollection
    {
        List<GameBehavior> behaviors = new List<GameBehavior>();

        public void Add(GameBehavior behavior)
        {
            behaviors.Add(behavior);
        }

        public void GameUpdate()
        {
            for (int i = 0; i < behaviors.Count; i++)
            {
                if (!behaviors[i].GameUpdate())
                {
                    int lastIndex = behaviors.Count - 1;
                    behaviors[i] = behaviors[lastIndex];
                    behaviors.RemoveAt(lastIndex);
                    i -= 1;
                }
            }
        }
    }
}
