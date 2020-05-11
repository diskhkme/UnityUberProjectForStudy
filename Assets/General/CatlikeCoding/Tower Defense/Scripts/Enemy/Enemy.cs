using System;
using UnityEngine;

namespace Defense
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] Transform model = default;

        EnemyFactory originFactory;

        //enemy 이동을 위한 정보들, spawnOn에서 초기화
        GameTile tileFrom, tileTo;
        Vector3 positionFrom, positionTo;
        float progress, progressFactor;

        Direction direction;
        DirectionChange directionChange;
        float directionAngleFrom, directionAngleTo;

        float pathOffset; //enemy의 좌우 이동경로 offset
        float speed;

        float Health { get; set; }
        public float Scale { get; private set; }

        public EnemyFactory OriginFactory
        {
            get => originFactory;
            set
            {
                Debug.Assert(originFactory == null, "Redefined origin factory!");
                originFactory = value;
            }
        }

        public void SpawnOn(GameTile spawnPoint)
        {
            Debug.Assert(spawnPoint.NextTileOnPath != null, "Nowhere to go!", this);
            tileFrom = spawnPoint;
            tileTo = spawnPoint.NextTileOnPath;
            progress = 0f;
            PrepareIntro();
        }

        public void Initialize(float scale, float pathOffset, float speed)
        {
            model.localScale = new Vector3(scale, scale, scale);
            this.Scale = scale;
            this.pathOffset = pathOffset;
            this.speed = speed;
            Health = 100f * scale;
        }

        public bool GameUpdate()
        {
            if(Health <= 0f) //health가 0이 되는 시점에서 반환하는 것이 아니라, 다음 frame update 초반에 함으로써, 여러 타워가 동시에 같은 대상을 보고있을때 제거하는 처리를 간단히 함!
            {
                OriginFactory.Reclaim(this);
                return false;
            }

            progress += Time.deltaTime * progressFactor; //상태에 따라 이동거리가 다르므로 progressFactor로 보전
            while (progress >= 1f)
            {

                if (tileTo == null) //destination 도달
                {
                    OriginFactory.Reclaim(this);
                    return false;
                }
                progress = (progress - 1f) / progressFactor;
                PrepareNextState();
                progress *= progressFactor;
            }

            if (directionChange == DirectionChange.None)
            {
                transform.localPosition = Vector3.LerpUnclamped(positionFrom, positionTo, progress);
            }
            else
            {
                float angle = Mathf.LerpUnclamped(directionAngleFrom, directionAngleTo, progress);
                transform.localRotation = Quaternion.Euler(0f, angle, 0f);
            }
            return true;
        }

        public void ApplyDamage(float damage)
        {
            Debug.Assert(damage >= 0f, "Nagative damage applied");
            Health -= damage;
        }

        void PrepareIntro()
        {
            positionFrom = tileFrom.transform.localPosition;
            positionTo = tileFrom.ExitPoint;
            direction = tileFrom.PathDirection;
            directionChange = DirectionChange.None;
            directionAngleFrom = directionAngleTo = direction.GetAngle();
            model.localPosition = new Vector3(pathOffset, 0f);
            transform.localRotation = tileFrom.PathDirection.GetRotation();
            progressFactor = 2f * speed;
        }

        void PrepareNextState()
        {
            //경로 갱신
            tileFrom = tileTo;
            tileTo = tileFrom.NextTileOnPath;
            positionFrom = positionTo;
            if (tileTo == null)
            {
                PrepareOutro();
                return;
            }
            positionTo = tileFrom.ExitPoint;
            directionChange = direction.GetDirectionChangeTo(tileFrom.PathDirection);
            direction = tileFrom.PathDirection;
            directionAngleFrom = directionAngleTo;

            switch (directionChange)
            {
                case DirectionChange.None: PrepareForward(); break;
                case DirectionChange.TurnRight: PrepareTurnRight(); break;
                case DirectionChange.TurnLeft: PrepareTurnLeft(); break;
                default: PrepareTurnAround(); break;
            }
        }

        void PrepareForward()
        {
            transform.localRotation = direction.GetRotation();
            directionAngleTo = direction.GetAngle();
            model.localPosition = new Vector3(pathOffset, 0f);
            progressFactor = speed;
        }

        void PrepareTurnRight()
        {
            directionAngleTo = directionAngleFrom + 90f;
            model.localPosition = new Vector3(pathOffset-0.5f, 0f); //local position 원점을 기준으로 잠시 이동, model은 enemy의 child transform
            transform.localPosition = positionFrom + direction.GetHalfVector();
            progressFactor = speed / (Mathf.PI * 0.5f * (0.5f - pathOffset));
        }

        void PrepareTurnLeft()
        {
            directionAngleTo = directionAngleFrom - 90f;
            model.localPosition = new Vector3(pathOffset+0.5f, 0f);
            transform.localPosition = positionFrom + direction.GetHalfVector();
            progressFactor = speed / (Mathf.PI * 0.5f * (0.5f + pathOffset));
        }

        void PrepareTurnAround()
        {
            directionAngleTo = directionAngleFrom + (pathOffset < 0f ? 180f : -180f);
            model.localPosition = new Vector3(pathOffset, 0f);
            transform.localPosition = positionFrom;
            progressFactor = speed / (Mathf.PI * Mathf.Max(Mathf.Abs(pathOffset), 0.2f));
        }

        void PrepareOutro() //destination 도착했을 때 보정
        {
            positionTo = tileFrom.transform.localPosition;
            directionChange = DirectionChange.None;
            directionAngleTo = direction.GetAngle();
            model.localPosition = new Vector3(pathOffset, 0f);
            transform.localRotation = direction.GetRotation();
            progressFactor = speed * 2f;
        }
    }
}
