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

    public Vector3 AngularVelocity { get; set; }
    public Vector3 Velocity { get; set; }

    private void Awake()
    {
        colors = new Color[meshRenderers.Length];
    }

    // void FixedUpdate() //개별적으로 FixedUpdate 호출은 성능에 매우 악영향
    public void GameUpdate()
    {
        this.transform.Rotate(AngularVelocity * Time.deltaTime);
        this.transform.localPosition += Velocity * Time.deltaTime;
    }

    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
        writer.Write(colors.Length);
        for(int i=0;i<colors.Length;i++)
        {
            writer.Write(colors[i]);
        }
        writer.Write(AngularVelocity);
        writer.Write(Velocity);
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
        
        AngularVelocity = reader.Version >= 4 ? reader.ReadVector3() : Vector3.zero;
        Velocity = reader.Version >= 4 ? reader.ReadVector3() : Vector3.zero;
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