using UnityEngine;

//pool과 관련하여 shape behavior를 재사용하기 위해서 shapebehavior가 monobehavior가 아닌 방향으로 수정
//1) mono인 경우 gameobject와 떼어서 저장할 수 없음
//2) factory가 모든 상황을 알고 저장하는 구현이 어려움
public abstract class ShapeBehavior
#if UNITY_EDITOR
    : ScriptableObject //recompile에서 사라지는 것을 방지하기위해 scriptable object로 만들어줌. 에디터에서만 발생하므로, 전처리 해줌
#endif
{
#if UNITY_EDITOR
    public bool IsReclaimed { get; set; }
    private void OnEnable()
    {
        if (IsReclaimed)
        {
            Recycle();
        }
    }
#endif
    public abstract ShapeBehaviorType BehaviorType { get; }
    public abstract bool GameUpdate(Shape shape); //현재 behavior가 필요한 상태인지 아닌지를 반환
    public abstract void Recycle();

    public abstract void Save(GameDataWriter writer);
    public abstract void Load(GameDataReader reader);



}
