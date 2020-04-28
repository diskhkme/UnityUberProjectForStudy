using UnityEngine;

public class Shape : PersistableObject
{
    //read-only라면 좋지만, 어디선가 할당은 필요함
    //inspector에서 설정하게 할 수도 있지만(serialize field) prefab별이 아닌 instance별로 할당이 필요할 수도 있으므로 유연성이 필요
    int shapeId = int.MinValue;

    //프로퍼티로 만듬
    public int ShapeId
    {
        get
        {
            return shapeId;
        }
        set
        {
            //readonly면 값 할당을 default인 0으로밖에 못함. 아니면 생성자에서 해 주어야 하는데, persistableObject는 monobehavior이므로 생성자 사용 불가
            if(shapeId == int.MinValue && value != int.MinValue)
            {
                shapeId = value;
            }
        }
    }

}