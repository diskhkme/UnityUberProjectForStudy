using UnityEngine;

//게임의 추가 상태 저장을 위해 돌아가는 spawn zone를 추가해 봄
public class RotatingObject : GameLevelObject
{
    [SerializeField] Vector3 angularVelocity;

    public override void GameUpdate()
    {
        this.transform.Rotate(angularVelocity * Time.deltaTime);
    }
    
}