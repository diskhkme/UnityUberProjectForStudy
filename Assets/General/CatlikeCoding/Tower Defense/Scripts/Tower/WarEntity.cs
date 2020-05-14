using UnityEngine;

namespace Defense
{
    //발사체를 관리하기 위한 클래스
    public abstract class WarEntity : GameBehavior
    {

        WarFactory originFactory;

        public WarFactory OriginFactory
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
