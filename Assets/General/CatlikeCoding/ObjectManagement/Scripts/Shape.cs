using System.Collections.Generic;
using UnityEngine;

public class Shape : PersistableObject
{
    [SerializeField] MeshRenderer[] meshRenderers; //composite shape들은 child object도 있음

    int shapeId = int.MinValue;
    public int ShapeId
    {
        get
        {
            return shapeId;
        }
        set
        {
            if(shapeId == int.MinValue && value != int.MinValue)
            {
                shapeId = value;
            }
        }
    }

    public int MaterialId { get; private set; }

    public void SetMaterial(Material material, int materialId)
    {
        for(int i=0;i<meshRenderers.Length;i++)
        {
            meshRenderers[i].material = material;
        }
        
        MaterialId = materialId;
    }

    Color[] colors;
    public int ColorCount
    {
        get
        {
            return colors.Length;
        }
    }
    static int colorPropertyId = Shader.PropertyToID("_Color");
    static MaterialPropertyBlock sharedPropertyBlock;

    public void SetColor(Color color)
    {
        if(sharedPropertyBlock == null)
        {
            sharedPropertyBlock = new MaterialPropertyBlock();
        }
        sharedPropertyBlock.SetColor(colorPropertyId, color);
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            colors[i] = color;
            meshRenderers[i].SetPropertyBlock(sharedPropertyBlock);
        }
    }

    public void SetColor(Color color, int index)
    {
        if (sharedPropertyBlock == null)
        {
            sharedPropertyBlock = new MaterialPropertyBlock();
        }
        sharedPropertyBlock.SetColor(colorPropertyId, color);
        colors[index] = color;
        meshRenderers[index].SetPropertyBlock(sharedPropertyBlock);
        
    }

    
    //어느 factory에서 생긴 shape인지를 추적. 이 정보를 가지고 있지 않으면, reclaim할 때 다른 factory에서 찾으려 할 수 있음.
    ShapeFactory originFactory;
    List<ShapeBehavior> behaviorList = new List<ShapeBehavior>(); //shape의 behavior를 업데이트 시키기 위한 리스트를 매뉴얼하게 관리
    public float Age { get; private set; } //shape이 생긴 시간을 저장. oscillation의 variation과 그 저장에 필요함
    public int InstanceId { get; private set; } 
    public int SaveIndex { get; set; }

    //shape에 behavior 추가하는 제네릭 메소드 구현, 뒤쪽의 new()는 기본 생성자가 있다고 알려주는 것.
    public T AddBehavior<T> () where T : ShapeBehavior, new()
    {
        //shapebehavior가 더이상 mono가 아니므로, addcomponent 불가능
        //T behavior = this.gameObject.AddComponent<T>(); 
        T behavior = ShapeBehaviorPool<T>.Get();
        behaviorList.Add(behavior);
        return behavior;
    }

    public ShapeFactory OriginFactory
    {
        get
        {
            return originFactory;
        }
        set
        {
            if(originFactory == null)
            {
                originFactory = value;
            }
            else
            {
                Debug.LogError("Not allowed to chage origin factory.");
            }
        }
    }

    public void Recycle()
    {
        Age = 0f;
        InstanceId++;
        //pool에서 재사용될 때 shape behavior가 계속 생성되므로, pool에 반납할 때 component 제거. 당연히 좀 비효율적이므로, 나중에 바꿀 것.
        for (int i=0;i<behaviorList.Count;i++)
        {
            //Destroy(behaviorList[i]);
            behaviorList[i].Recycle();
        }
        behaviorList.Clear();
        OriginFactory.Reclaim(this);
    }

    public void ResolveShapeInstances()
    {
        for(int i=0;i<behaviorList.Count;i++)
        {
            behaviorList[i].ResolveShapeInstances();
        }
    }

    public void Die()
    {
        Game.Instance.Kill(this);
    }

    private void Awake()
    {
        colors = new Color[meshRenderers.Length];
    }

    public void GameUpdate()
    {
        Age += Time.deltaTime;
        //이제 각 shape의 update는 개별 behavior component의 역할
        for(int i=0;i<behaviorList.Count;i++)
        {
            if(!behaviorList[i].GameUpdate(this)) //update가 필요없는 behavior는 반환(satellite, focus가 없는 경우)
            {
                behaviorList[i].Recycle();
                behaviorList.RemoveAt(i--);
            }
        }
    }

    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
        writer.Write(colors.Length);
        for(int i=0;i<colors.Length;i++)
        {
            writer.Write(colors[i]);
        }
        writer.Write(Age);
        writer.Write(behaviorList.Count);
        for(int i=0;i<behaviorList.Count;i++)
        {
            writer.Write((int)behaviorList[i].BehaviorType);
            behaviorList[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        base.Load(reader);
        if(reader.Version >= 5)
        {
            LoadColors(reader);
        }
        else
        {
            SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);
        }

        if (reader.Version >= 6)
        {
            Age = reader.ReadFloat();
            int behaviorCount = reader.ReadInt();
            for (int i = 0; i < behaviorCount; i++)
            {
                ShapeBehavior behavior = ((ShapeBehaviorType)reader.ReadInt()).GetInstance();
                behaviorList.Add(behavior);
                behavior.Load(reader);
            }
        }
        else if(reader.Version >= 4) //backward compatibility
        {
            AddBehavior<RotationShapeBehavior>().AngularVelocity = reader.ReadVector3();
            AddBehavior<MovementShapeBehavior>().Velocity = reader.ReadVector3();
        }

        
    }

    private void LoadColors(GameDataReader reader)
    {
        int count = reader.ReadInt();
        int max = count <= colors.Length ? count : colors.Length;
        int i = 0;
        for (; i < max; i++)
        {
            SetColor(reader.ReadColor(), i);
        }
        //남은 데이터를 읽어서 buffer read 위치를 옮겨 주어야 함!
        if(count > colors.Length)
        {
            for(;i<count;i++)
            {
                reader.ReadColor();
            }
        }
        else if(count < colors.Length) //그렇지 않은 경우 색 할당이 더 필요하단 이야기
        {
            for(;i<colors.Length;i++)
            {
                SetColor(Color.white, i); //흰색으로 채움.
            }
        }
    }
}