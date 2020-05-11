using UnityEngine;

namespace Defense
{
    public enum GameTileContentType
    {
        Empty, Destination, Wall, SpawnPoint, Tower
    }

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

        public void Recycle()
        {
            originFactory.Reclaim(this);
        }
    }

}
