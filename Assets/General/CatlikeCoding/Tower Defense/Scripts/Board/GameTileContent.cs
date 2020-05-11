using UnityEngine;

namespace Defense
{
    public enum GameTileContentType
    {
        Empty, Destination, Wall, SpawnPoint, Tower
    }

    [SelectionBase] //scene view에서 객체를 선택할 때 root object가 선택되도록 하는 attrubute
    public class GameTileContent : MonoBehaviour
    {
        [SerializeField] GameTileContentType type = default;
        GameTileContentFactory originFactory;

        public bool BlocksPath => Type == GameTileContentType.Wall || Type == GameTileContentType.Tower; //tower와 wall은 모두 길을 막음
        public GameTileContentType Type => type;

        public GameTileContentFactory OriginFactory
        {
            get => originFactory;
            set
            {
                Debug.Assert(originFactory == null, "Redefined origin factory!");
                originFactory = value;
            }
        }

        public virtual void GameUpdate() {}

        public void Recycle()
        {
            originFactory.Reclaim(this);
        }
    }

}
