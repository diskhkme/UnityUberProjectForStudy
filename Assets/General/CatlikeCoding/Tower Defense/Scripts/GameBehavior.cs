using UnityEngine;

namespace Defense
{
    //enemy와 non-enemy의 behavior를 통합해서 관리하기 위한 클래스
    public abstract class GameBehavior : MonoBehaviour
    {

        public virtual bool GameUpdate() => true;
    }
}
